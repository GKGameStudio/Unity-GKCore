/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace GKCore.Observers
{
    /// <summary>
    /// Base class for LinkedValue.
    /// </summary>
    [Serializable]
    public abstract class LinkedValue
    {
        /// <summary>
        /// Should exceptions be thrown when errors occur.
        /// </summary>
        public static bool ThrowExceptions = false;
    }
    
    /// <summary>
    /// Class that allows you to link a value of a field or property of an object.
    /// </summary>
    /// <typeparam name="T">Type of value.</typeparam>
    [Serializable]
    public class LinkedValue<T>: LinkedValue
    {
        
        /// <summary>
        /// Target object.
        /// </summary>
        [SerializeField]
        private Object _target;

        /// <summary>
        /// Path to field or property.
        /// </summary>
        [SerializeField]
        private string _propertyPath;
        
        /// <summary>
        /// Event that occurs when the value changes.
        /// </summary>
        [SerializeField] 
        private UnityEvent<T, T> _changed;
        
        /// <summary>
        /// Display name.
        /// </summary>
        [SerializeField]
#pragma warning disable 0414
        private string _displayName = "None";
#pragma warning restore 0414
        
        /// <summary>
        /// Field info.
        /// </summary>
        [NonSerialized]
        private FieldInfo _fieldInfo;

        /// <summary>
        /// Is member info initialized.
        /// </summary>
        [NonSerialized]
        private bool _isMemberInfoInitialized;

        /// <summary>
        /// Is read only.
        /// </summary>
        [NonSerialized]
        private bool _isReadOnly;

        /// <summary>
        /// Last value.
        /// </summary>
        [NonSerialized]
        private T _lastValue;

        /// <summary>
        /// Member type.
        /// </summary>
        [NonSerialized]
        private MemberType _memberType = MemberType.None;

        /// <summary>
        /// Property info.
        /// </summary>
        [NonSerialized]
        private PropertyInfo _propertyInfo;

        /// <summary>
        /// Routine.
        /// </summary>
        [NonSerialized]
        private IEnumerator _routine;

        /// <summary>
        /// Is value changed from last check.
        /// </summary>
        public bool IsChanged
        {
            get
            {
                T value = Value;
                bool isChanged = !Equals(value, _lastValue);
                _lastValue = value;
                return isChanged;
            }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public T Value
        {
            get
            {
                if (!_isMemberInfoInitialized) InitializeMemberInfo();
                
                switch (_memberType)
                {
                    case MemberType.Field:
                        return (T)_fieldInfo.GetValue(_target);
                    case MemberType.Property:
                        return (T)_propertyInfo.GetValue(_target);
                    default:
                        return default;
                }
            }
            set
            {
                if (!_isMemberInfoInitialized) InitializeMemberInfo();
                if (_isReadOnly) return;

                switch (_memberType)
                {
                    case MemberType.Field:
                        _fieldInfo.SetValue(_target, value);
                        break;
                    case MemberType.Property:
                        _propertyInfo.SetValue(_target, value);
                        break;
                }
            }
        }
        
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="target">Target object.</param>
        /// <param name="propertyPath">Path to field or property.</param>
        public LinkedValue(Object target, string propertyPath)
        {
            _target = target;
            _propertyPath = propertyPath;
            _displayName = propertyPath;
        }
        
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="target">Target object.</param>
        /// <param name="fieldInfo">Field info.</param>
        public LinkedValue(Object target, FieldInfo fieldInfo)
        {
            _target = target;
            _fieldInfo = fieldInfo;
            _displayName = fieldInfo.Name;
            _memberType = MemberType.Field;
            _isReadOnly = true;
            _isMemberInfoInitialized = true;
        }
        
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="target">Target object.</param>
        /// <param name="propertyInfo">Property info.</param>
        public LinkedValue(Object target, PropertyInfo propertyInfo)
        {
            _target = target;
            _propertyInfo = propertyInfo;
            _displayName = propertyInfo.Name;
            _memberType = MemberType.Property;
            _isReadOnly = !_propertyInfo.CanWrite;
            _isMemberInfoInitialized = true;
        }

        /// <summary>
        /// Adds a listener to the Changed event.
        /// </summary>
        /// <param name="action">Listener.</param>
        public void AddListener(UnityAction<T, T> action)
        {
            _changed.AddListener(action);
        }

        private bool CompareMemberName(string memberName)
        {
            if (memberName.Length != _propertyPath.Length - 2) return false;
            
            for (int i = 0; i < memberName.Length; i++)
            {
                if (char.ToUpperInvariant(memberName[i]) != char.ToUpperInvariant(_propertyPath[i + 2])) return false;
            }

            return true;
        }

        /// <summary>
        /// Releases all resources used by LinkedValue.
        /// </summary>
        public void Dispose()
        {
            _target = null;
            _propertyPath = null;
            _displayName = null;
            _fieldInfo = null;
            _propertyInfo = null;
            _memberType = MemberType.None;
            _isMemberInfoInitialized = false;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">Object to compare.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return obj is ObservableVar<T> other && Equals(other);
        }

        /// <summary>
        /// Determines whether the specified LinkedValue is equal to the current LinkedValue.
        /// </summary>
        /// <param name="other">LinkedValue to compare.</param>
        /// <returns>true if the specified LinkedValue is equal to the current LinkedValue; otherwise, false.</returns>
        public bool Equals(LinkedValue<T> other)
        {
            if (other == null) return false;
            return Equals(Value, other.Value);
        }

        /// <summary>
        /// Gets the hash code for this LinkedValue.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        private void InitializeMemberInfo()
        {
            _isMemberInfoInitialized = true;
            _memberType = MemberType.None;

            if (_target == null)
            {
                Throw(new NullReferenceException("Target is null."));
                return;
            }

            if (string.IsNullOrEmpty(_propertyPath))
            {
                Throw(new Exception("Property path is empty."));
                return;
            }
                
            Type type = _target.GetType();
            if (TryInitMember(type, _propertyPath)) return;
            TryInitUnityMember(type);
            
            if (_memberType == MemberType.None)
            {
                Throw(new MissingMemberException(GetMissingMemberMessage()));
            }
        }
        
        /// <summary>
        /// Invokes the Changed event.
        /// </summary>
        public void InvokeChanged()
        {
            try
            {
                _changed?.Invoke(Value, Value);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private string GetMissingMemberMessage()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(TypeHelper.GetTypeName(typeof(T).Name));
            sb.Append(" ");
            sb.Append(_target.GetType().Name);
            sb.Append(".");
            sb.Append(_propertyPath);
            return sb.ToString();
        }

        private IEnumerator Observe()
        {
            while (true)
            {
                yield return null;
                if (IsChanged) _changed?.Invoke(_lastValue, _lastValue);
            }
        }
        
        /// <summary>
        /// Removes all listeners from the Changed event.
        /// </summary>
        public void RemoveAllListeners()
        {
#if UNITY_EDITOR
            FieldInfo persistentCalls = typeof(UnityEventBase).GetField("m_PersistentCalls", BindingFlags.Instance | BindingFlags.NonPublic);
            object value = persistentCalls.GetValue(_changed);
            value.GetType().GetMethod("Clear").Invoke(value, null);
#else 
            _changed.RemoveAllListeners();
#endif
        }
        
        /// <summary>
        /// Removes a listener from the Changed event.
        /// </summary>
        /// <param name="action">Listener.</param>
        public void RemoveListener(UnityAction<T, T> action)
        {
            if (action == null) return;

#if UNITY_EDITOR
            UnityEventTools.RemovePersistentListener(_changed, action);
#else
            _changed.RemoveListener(action);
#endif
        }

        /// <summary>
        /// Starts observing for changes.
        /// </summary>
        /// <param name="script">Script that will be used to start the coroutine.</param>
        /// <returns>The current instance.</returns>
        public LinkedValue<T> StartObserving(MonoBehaviour script)
        {
            _lastValue = Value;
            if (_routine != null) return this;
            
            _routine = Observe();
            script.StartCoroutine(_routine);

            return this;
        }
        
        /// <summary>
        /// Stops observing for changes.
        /// </summary>
        /// <param name="script">Script that was used to start the coroutine.</param>
        public void StopObserving(MonoBehaviour script)
        {
            if (_routine != null)
            {
                script.StopCoroutine(_routine);
                _routine = null;
            }
        }

        private static void Throw(Exception e)
        {
            if (ThrowExceptions) throw e;
            Debug.LogException(e);
        }

        private bool TryInitMember(Type type, string name)
        {
            _fieldInfo = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (_fieldInfo != null)
            {
                if (_fieldInfo.FieldType != typeof(T)) return false;
                
                _isReadOnly = true;
                _memberType = MemberType.Field;
                return true;
            }

            _propertyInfo = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (_propertyInfo != null)
            {
                if (_propertyInfo.PropertyType != typeof(T)) return false;
                
                _isReadOnly = !_propertyInfo.CanWrite;
                _memberType = MemberType.Property;
                _lastValue = Value;
                return true;
            }

            return false;
        }

        private void TryInitUnityMember(Type type)
        {
            if (!_propertyPath.StartsWith("m_")) return;

            MemberInfo[] members = type.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            for (int i = 0; i < members.Length; i++)
            {
                MemberInfo member = members[i];
                if (TryInitUnityMember(member)) return;
            }
        }

        private bool TryInitUnityMember(MemberInfo member)
        {
            if (member.MemberType != MemberTypes.Field && member.MemberType != MemberTypes.Property) return false;
            if (!CompareMemberName(member.Name)) return false;

            if (member.MemberType == MemberTypes.Field)
            {
                FieldInfo fieldInfo = (FieldInfo)member;
                if (fieldInfo.FieldType != typeof(T)) return false;

                _fieldInfo = fieldInfo;
                _memberType = MemberType.Field;
                _lastValue = Value;
                return true;
            }

            PropertyInfo propertyInfo = (PropertyInfo)member;
            if (propertyInfo.PropertyType != typeof(T)) return false;

            _propertyInfo = propertyInfo;
            _isReadOnly = !_propertyInfo.CanWrite;
            _memberType = MemberType.Property;
            _lastValue = Value;
            return true;
        }

        public override string ToString()
        {
            return Value != null? Value.ToString() : "null";
        }

        public static bool operator ==(LinkedValue<T> a, LinkedValue<T> b)
        {
            if (a is null && b is null) return true;
            if (a is null || b is null) return false;
            return Equals(a.Value, b.Value);
        }

        public static bool operator !=(LinkedValue<T> a, LinkedValue<T> b)
        {
            if (a is null && b is null) return false;
            if (a is null || b is null) return true;
            return !Equals(a.Value, b.Value);
        }

        public static bool operator ==(LinkedValue<T> a, T b)
        {
            if (a is null) return false;
            return Equals(a.Value, b);
        }

        public static bool operator !=(LinkedValue<T> a, T b)
        {
            if (a is null) return true;
            return !Equals(a.Value, b);
        }

        public static implicit operator T(LinkedValue<T> value)
        {
            return value.Value;
        }
        
        public static LinkedValue<T> operator +(LinkedValue<T> value, UnityAction<T, T> action)
        {
            value.AddListener(action);
            return value;
        }
        
        public static LinkedValue<T> operator -(LinkedValue<T> value, UnityAction<T, T> action)
        {
            value.RemoveListener(action);
            return value;
        }
        
        private enum MemberType
        {
            None,
            Field,
            Property
        }
    }
}