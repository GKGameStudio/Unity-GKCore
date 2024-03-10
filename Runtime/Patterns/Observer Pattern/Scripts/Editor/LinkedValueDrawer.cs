/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace GKCore.Observers.Editors
{
    [CustomPropertyDrawer(typeof(LinkedValue<>))]
    public class LinkedValueDrawer : PropertyDrawer
    {
        private const float Margin = 2;

        private const int Indent = 15;

        private static GUIContent _helpContent;
        
        private float _changedHeight;
        private UnityEventBase _changedInstance;
        private float _helpHeight;
        private string _helpMessage;
        private bool _initialized;
        private bool _isTargetValuePropertyInitialized;
        private bool _showEvents;
        private object _targetObject;
        private SerializedProperty _targetValueProperty;
        private float _valueHeight;

        private void DrawEvents(Rect position, SerializedProperty changedProperty)
        {
            Rect helpRect = new Rect(position.x, position.y, position.width, _helpHeight);
            EditorGUI.HelpBox(helpRect, _helpMessage, MessageType.Info);
            
            Rect changedRect = new Rect(position.x, helpRect.yMax + Margin, position.width, _changedHeight);
            EditorGUI.PropertyField(changedRect, changedProperty);
        }

        private void DrawValue(Rect position, SerializedProperty property)
        {
            if (_targetValueProperty == null) return;

            EditorGUIUtility.labelWidth -= Indent;
            Rect valueRect = new Rect(
                position.x + Indent,
                position.y + Margin + EditorGUIUtility.singleLineHeight,
                position.width - Indent - 20,
                _valueHeight
            );
            
            Rect buttonRect = new Rect(
                valueRect.xMax,
                valueRect.y,
                20,
                EditorGUIUtility.singleLineHeight
            );
            
            try
            {
                bool wideMode = EditorGUIUtility.wideMode;
                EditorGUIUtility.wideMode = true;
                _targetValueProperty.serializedObject.Update();
                EditorGUI.BeginChangeCheck();
                EditorGUI.PropertyField(valueRect, _targetValueProperty, true);
                if (EditorGUI.EndChangeCheck()) _targetValueProperty.serializedObject.ApplyModifiedProperties();
                EditorGUIUtility.wideMode = wideMode;
            }
            catch (ExitGUIException)
            {
                throw;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            GUIContent content = EventButtonContent.GetContent(_changedInstance);
            if (GUI.Button(buttonRect, content, EditorStyles.toolbarButton)) _showEvents = !_showEvents;
            if (_showEvents)
            {
                Rect eventsRect = new Rect(
                    valueRect.x,
                    valueRect.yMax + Margin,
                    position.width - Indent,
                    _changedHeight
                );
                DrawEvents(eventsRect, property.FindPropertyRelative("_changed"));
            }

            EditorGUIUtility.labelWidth += Indent;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!_isTargetValuePropertyInitialized) UpdateTargetValueProperty(property);
            if (_helpMessage == null)
            {
                _helpMessage = $"Call {property.name}.StartObserving(this) in play mode to start observing and notify listeners.";
                _helpContent = new GUIContent(_helpMessage, EditorGUIUtility.IconContent("console.infoicon").image);
            }

            float height = EditorGUIUtility.singleLineHeight;
            
            if (_targetValueProperty == null) return height;

            _valueHeight = EditorGUI.GetPropertyHeight(_targetValueProperty);
            height += Margin + _valueHeight;

            if (_showEvents)
            {
                _helpHeight = EditorStyles.helpBox.CalcHeight(_helpContent, EditorGUIUtility.currentViewWidth - Indent) + 5;
                height += Margin + _helpHeight;
                
                _changedHeight = EditorGUI.GetPropertyHeight(property.FindPropertyRelative("_changed"));
                height += Margin + _changedHeight;
            }
            
            return height;
        }

        private void Initialize(SerializedProperty property)
        {
            _initialized = true;
            _targetObject = fieldInfo.GetValue(property.serializedObject.targetObject);
            FieldInfo field = fieldInfo.FieldType.GetField("_changed", BindingFlags.Instance | BindingFlags.NonPublic);
            _changedInstance = field.GetValue(_targetObject) as UnityEventBase;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!_initialized) Initialize(property);
            
            EditorGUI.BeginProperty(position, label, property);
            
            if (!_isTargetValuePropertyInitialized) UpdateTargetValueProperty(property);
            
            SerializedProperty targetProperty = property.FindPropertyRelative("_target");
            SerializedProperty propertyPathProperty = property.FindPropertyRelative("_propertyPath");
            SerializedProperty displayNameProperty = property.FindPropertyRelative("_displayName");
            
            float labelWidth = EditorGUIUtility.labelWidth;
            float contentWidth = position.width - labelWidth;
            float targetWidth = contentWidth / 2 + labelWidth;
            float memberWidth = contentWidth / 2;
            
            Rect targetRect = new Rect(position.x, position.y, targetWidth , EditorGUIUtility.singleLineHeight);
            Rect memberRect = new Rect(targetRect.xMax, position.y, memberWidth, EditorGUIUtility.singleLineHeight);

            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(targetRect, targetProperty, label);
            if (EditorGUI.EndChangeCheck())
            {
                _targetValueProperty = null;
                _isTargetValuePropertyInitialized = false;
                propertyPathProperty.stringValue = string.Empty;
                displayNameProperty.stringValue = "None";
            }

            if (EditorGUI.DropdownButton(memberRect, TempContent.Get(displayNameProperty.stringValue), FocusType.Keyboard))
            {
                ShowSelectionMenu(property, targetProperty, propertyPathProperty, displayNameProperty);
            }
            
            DrawValue(position, property);

            EditorGUI.EndProperty();
        }

        private void ShowSelectionMenu(SerializedProperty property, SerializedProperty targetProperty, SerializedProperty propertyPathProperty, SerializedProperty displayNameProperty)
        {
            if (targetProperty.objectReferenceValue == null) return;

            SerializedProperty prop = property.Copy();
            SerializedProperty pathProp = propertyPathProperty.Copy();
            SerializedProperty nameProp = displayNameProperty.Copy();

            Type genericTypeArgument = fieldInfo.FieldType.GenericTypeArguments[0];
            string genericType = TypeHelper.GetTypeName(genericTypeArgument.Name);

            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("None"), string.IsNullOrEmpty(pathProp.stringValue), () =>
            {
                pathProp.stringValue = string.Empty;
                nameProp.stringValue = "None";
                prop.serializedObject.ApplyModifiedProperties();
                _targetValueProperty = null;
            });
            
            Object target = targetProperty.objectReferenceValue;
            SerializedObject serializedObject = new SerializedObject(target);
            SerializedProperty iterator = serializedObject.GetIterator();
            bool enterChildren = true;
            
            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;
                
                string path = iterator.propertyPath;
                FieldInfo info = target.GetType().GetField(path, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                if (info != null)
                {
                    if (info.FieldType != genericTypeArgument) continue;
                }
                else if (iterator.type != genericType) continue;
                
                string displayName = iterator.displayName;
                
                menu.AddItem(new GUIContent(displayName), iterator.name == pathProp.stringValue, () =>
                {
                    pathProp.stringValue = path;
                    nameProp.stringValue = displayName;
                    prop.serializedObject.ApplyModifiedProperties();
                    UpdateTargetValueProperty(prop);
                });
            }
            
            menu.ShowAsContext();
        }
        
        private void UpdateTargetValueProperty(SerializedProperty property)
        {
            _targetValueProperty = null;
            _isTargetValuePropertyInitialized = true;
            
            SerializedProperty targetProperty = property.FindPropertyRelative("_target");
            if (targetProperty.objectReferenceValue == null) return;
            
            SerializedProperty propertyPathProperty = property.FindPropertyRelative("_propertyPath");
            if (string.IsNullOrEmpty(propertyPathProperty.stringValue)) return;
            
            Object target = targetProperty.objectReferenceValue;
            SerializedObject targetSerializedObject = new SerializedObject(target);
            _targetValueProperty = targetSerializedObject.FindProperty(propertyPathProperty.stringValue);
        }
    }
}