/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace GKCore.Observers.Editors
{
	[CustomPropertyDrawer(typeof(ValueObserver<>))]
	public class ValueObserverDrawer : PropertyDrawer
	{
		private bool _initialized;
		
		private UnityEventBase _changedInstance;
		private MethodInfo _setMethod;
		private bool _showEvents;
		private bool _showValidate;
		private object _targetObject;
		private UnityEventBase _validateInstance;
		private PropertyInfo _valueProperty;

        private void DrawEvents(Rect position, SerializedProperty changedProperty, SerializedProperty validateProperty)
		{
			Rect changedRect = new Rect(position.x, position.y, position.width, EditorGUI.GetPropertyHeight(changedProperty));
			EditorGUI.PropertyField(changedRect, changedProperty);

			if (_showValidate)
			{
				Rect validateRect = new Rect(changedRect.x, changedRect.yMax + EditorGUIUtility.standardVerticalSpacing, changedRect.width, EditorGUI.GetPropertyHeight(validateProperty));
				EditorGUI.PropertyField(validateRect, validateProperty);
			}
			else
			{
				Rect buttonRect = new Rect(changedRect.x, changedRect.yMax + EditorGUIUtility.standardVerticalSpacing, changedRect.width, EditorGUIUtility.singleLineHeight);
				if (GUI.Button(buttonRect, "+ Validation", EditorStyles.toolbarButton)) _showValidate = true;
			}
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			float valueHeight = EditorGUI.GetPropertyHeight(property.FindPropertyRelative("_value"));
			if (!_showEvents) return valueHeight;

            float changedHeight = EditorGUI.GetPropertyHeight(property.FindPropertyRelative("_changed"));
			float height = valueHeight + EditorGUIUtility.standardVerticalSpacing * 2 + changedHeight;

			if (_showValidate) height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("_validate"));
			else height += EditorGUIUtility.singleLineHeight;
			
			return height;
		}

		private void Initialize(SerializedProperty property)
		{
			_initialized = true;

			_targetObject = fieldInfo.GetValue(property.serializedObject.targetObject);
			_valueProperty = fieldInfo.FieldType.GetProperty("Value");

			FieldInfo field = fieldInfo.FieldType.GetField("_changed", BindingFlags.Instance | BindingFlags.NonPublic);
			_changedInstance = field.GetValue(_targetObject) as UnityEventBase;
			
			field = fieldInfo.FieldType.GetField("_validate", BindingFlags.Instance | BindingFlags.NonPublic);
			_validateInstance = field.GetValue(_targetObject) as UnityEventBase;
			
			_showValidate = _validateInstance.GetPersistentEventCount() > 0;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (!_initialized) Initialize(property);

			EditorGUI.BeginProperty(position, label, property);

			SerializedProperty valueProperty = property.FindPropertyRelative("_value");
			SerializedProperty changedProperty = property.FindPropertyRelative("_changed");
			SerializedProperty validateProperty = property.FindPropertyRelative("_validate");

			float singleLineHeight = EditorGUIUtility.singleLineHeight;
			Rect valueRect = new Rect(position.x, position.y, position.width - 20, EditorGUI.GetPropertyHeight(valueProperty));
			Rect buttonRect = new Rect(position.xMax - 20, position.y, 20, singleLineHeight);

			EditorGUI.BeginChangeCheck();
            label.text = property.displayName;
			EditorGUI.PropertyField(valueRect, valueProperty, label, true);
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(property.serializedObject.targetObject, "Change Value");
				_valueProperty.SetValue(_targetObject, valueProperty.boxedValue);
				valueProperty.boxedValue = _valueProperty.GetValue(_targetObject);
				EditorUtility.SetDirty(property.serializedObject.targetObject);
			}
			
			GUIContent eventContent = EventButtonContent.GetContent(_changedInstance);
			_showEvents = GUI.Toggle(buttonRect, _showEvents, eventContent, EditorStyles.toolbarButton);

            if (_showEvents)
            {
                position.yMin += valueRect.height + EditorGUIUtility.standardVerticalSpacing;
                DrawEvents(position,  changedProperty, validateProperty);
            }

			EditorGUI.EndProperty();
		}
	}
}