/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GKCore.Observers.Editors
{
    [CustomPropertyDrawer(typeof(LinkedMethod), true)]
    public class LinkedMethodDrawer : PropertyDrawer
    {
        private MethodInfo[] methods;
        private string[] methodNames;
        private int selectedMethodIndex = -1;
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            SerializedProperty targetProperty = property.FindPropertyRelative("_target");
            SerializedProperty methodNameProperty = property.FindPropertyRelative("_methodName");

            float labelWidth = EditorGUIUtility.labelWidth;
            float contentWidth = position.width - labelWidth;
            float halfContentWidth = contentWidth / 2;
            
            if (methods == null)
            {
                UpdateMethods(targetProperty.objectReferenceValue);
                if (!string.IsNullOrEmpty(methodNameProperty.stringValue))
                {
                    selectedMethodIndex = Array.FindIndex(methods, m => m.Name == methodNameProperty.stringValue);
                }
            }
            
            Rect targetRect = new Rect(position.x, position.y, halfContentWidth + labelWidth, EditorGUIUtility.singleLineHeight);
            Rect methodRect = new Rect(targetRect.xMax, position.y, halfContentWidth, EditorGUIUtility.singleLineHeight);

            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(targetRect, targetProperty, label);

            if (EditorGUI.EndChangeCheck())
            {
                UpdateMethods(targetProperty.objectReferenceValue);
                selectedMethodIndex = -1;
                methodNameProperty.stringValue = string.Empty;
            }
            
            EditorGUI.BeginChangeCheck();
            selectedMethodIndex = EditorGUI.Popup(methodRect, selectedMethodIndex, methodNames);
            if (EditorGUI.EndChangeCheck())
            {
                if (selectedMethodIndex >= 0) methodNameProperty.stringValue = methods[selectedMethodIndex].Name;
                else methodNameProperty.stringValue = string.Empty;
            }
            
            EditorGUI.EndProperty();
        }

        private void UpdateMethods(Object target)
        {
            if (target == null)
            {
                methods = Array.Empty<MethodInfo>();
                methodNames = Array.Empty<string>();
                return;
            }
            
            Type type = target.GetType();
            methods = type.GetMethods(ReflectionHelper.BindingFlags);
            
            Type[] genericArguments = fieldInfo.FieldType.GenericTypeArguments;
            
            methods = methods.Where(delegate(MethodInfo m)
            {
                if (m.ReturnParameter.ParameterType != genericArguments.Last()) return false;
                ParameterInfo[] parameters = m.GetParameters();
                if (parameters.Length != genericArguments.Length - 1) return false;
                return parameters.Select(p => p.ParameterType).SequenceEqual(genericArguments.Take(genericArguments.Length - 1));
            }).ToArray();
            
            methodNames = methods.Select(m => m.Name).ToArray();
        }
    }
}