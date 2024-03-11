/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GKCore.Observers.Editors
{
    [CustomPropertyDrawer(typeof(LinkedObserver<>))]
    public class LinkedObserverDrawer : PropertyDrawer
    {
        private const float Margin = 2;
        
        private bool _isTargetValuePropertyInitialized;
        private SerializedProperty _targetValueProperty;
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!_isTargetValuePropertyInitialized) UpdateTargetValueProperty(property);
            
            if (_targetValueProperty != null) return EditorGUIUtility.singleLineHeight + Margin + EditorGUI.GetPropertyHeight(_targetValueProperty);
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            if (!_isTargetValuePropertyInitialized) UpdateTargetValueProperty(property);
            
            SerializedProperty targetProperty = property.FindPropertyRelative("_target");
            SerializedProperty propertyPathProperty = property.FindPropertyRelative("_propertyPath");
            SerializedProperty displayNameProperty = property.FindPropertyRelative("_displayName");
            
            float labelWidth = EditorGUIUtility.labelWidth;
            float contentWidth = position.width - labelWidth;
            float targetWidth = contentWidth / 2 + labelWidth;
            float memberWidth = contentWidth / 2;
            
            Rect targetRect = new Rect(position.x, position.y, targetWidth, EditorGUIUtility.singleLineHeight);
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
            
            if (_targetValueProperty != null)
            {
                const int indent = 15;
                EditorGUIUtility.labelWidth -= indent;
                Rect valueRect = new Rect(
                    position.x + indent, 
                    position.y + Margin + EditorGUIUtility.singleLineHeight, 
                    position.width - indent, 
                    position.height - EditorGUIUtility.singleLineHeight - Margin
                );
                try
                {
                    bool wideMode = EditorGUIUtility.wideMode;
                    EditorGUIUtility.wideMode = true;
                    _targetValueProperty.serializedObject.Update();
                    EditorGUI.BeginChangeCheck();
                    EditorGUI.PropertyField(valueRect, _targetValueProperty);
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
                
                EditorGUIUtility.labelWidth += indent;
            }

            EditorGUI.EndProperty();
        }
        
        private void ShowSelectionMenu(SerializedProperty property, SerializedProperty targetProperty, SerializedProperty propertyPathProperty, SerializedProperty displayNameProperty)
        {
            if (targetProperty.objectReferenceValue == null) return;

            SerializedProperty prop = property.Copy();
            SerializedProperty pathProp = propertyPathProperty.Copy();
            SerializedProperty nameProp = displayNameProperty.Copy();

            Type genericTypeArgument = fieldInfo.FieldType.GenericTypeArguments[0];
            Type genericType = typeof(ObservableVar<>).MakeGenericType(genericTypeArgument);
            string genericTypeStr = genericType.Name;

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
                if (iterator.type != genericTypeStr) continue;
                
                FieldInfo info = target.GetType().GetField(iterator.name);
                if (info == null) continue;
                
                if (info.FieldType != genericType) continue;
                
                string path = iterator.propertyPath;
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