/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor.Events;
#endif

namespace GKCore.Observers
{
    /// <summary>
    /// Implementation of the observer pattern.
    /// </summary>
    /// <typeparam name="T">Type of value.</typeparam>
    [Serializable]
    public class ObservableValue<T>
    {
        [SerializeField] 
        private UnityEvent<T, T> _changed;
        
        [SerializeField]
        private T _value;
        
        [SerializeField]
        private UnityEvent<Validatable<T>> _validate;
        
        [NonSerialized]
        private Validatable<T> _validatable;

        /// <summary>
        /// Gets or sets the value.
        /// If the value changes, then the Changed event will be triggered.
        /// </summary>
        public T Value
        {
            get { return _value; }
            set { Set(value); }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="value">Initial value.</param>
        /// <param name="changedCallback">Method that will be called when the value changes.</param>
        public ObservableValue(T value, UnityAction<T, T> changedCallback = null)
        {
            _value = value;
            _changed = new UnityEvent<T, T>();
            if (changedCallback != null) _changed.AddListener(changedCallback);
        }
        
        /// <summary>
        /// Adds a listener to the Changed event.
        /// </summary>
        /// <param name="action">Listener.</param>
        public void AddListener(UnityAction<T, T> action)
        {
            if (action == null) return;
            if (_changed == null) _changed = new UnityEvent<T, T>();

#if UNITY_EDITOR
            if (action.Target is Object) UnityEventTools.AddPersistentListener(_changed, action);
            else _changed.AddListener(action);
#else
            _changed.AddListener(action);
#endif
        }

        /// <summary>
        /// Adds a listener to the Validate event.
        /// </summary>
        /// <param name="action">Listener.</param>
        public void AddValidation(UnityAction<Validatable<T>> action)
        {
            if (action == null) return;
            if (_validate == null) _validate = new UnityEvent<Validatable<T>>();

#if UNITY_EDITOR
            if (action.Target is Object) UnityEventTools.AddPersistentListener(_validate, action);
            else _validate.AddListener(action);
#else
            _validate.AddListener(action);
#endif
        }
        
        /// <summary>
        /// Disposes the object.
        /// </summary>
        public void Dispose()
        {
            RemoveAllListeners();
            RemoveAllValidation();

            _changed = null;
            _validate = null;
            _value = default;
            _validatable = null;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ObservableValue<T>);
        }

        public bool Equals(ObservableValue<T> other)
        {
            if (other is null) return false;
            return Equals(_value, other._value);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        /// <summary>
        /// Invokes the Changed event.
        /// </summary>
        public void InvokeChanged(T oldValue, T newValue)
        {
            try
            {
                _changed?.Invoke(oldValue, newValue);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
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
#endif
            _changed.RemoveAllListeners();
        }

        /// <summary>
        /// Removes all listeners from the Validate event.
        /// </summary>
        public void RemoveAllValidation()
        {
#if UNITY_EDITOR
            FieldInfo persistentCalls = typeof(UnityEventBase).GetField("m_PersistentCalls", BindingFlags.Instance | BindingFlags.NonPublic);
            object value = persistentCalls.GetValue(_validate);
            value.GetType().GetMethod("Clear").Invoke(value, null);
#endif
            _validate.RemoveAllListeners();
        }

        /// <summary>
        /// Removes a listener from the Changed event.
        /// </summary>
        /// <param name="action">Listener.</param>
        public void RemoveListener(UnityAction<T, T> action)
        {
            if (action == null) return;

#if UNITY_EDITOR
            if (action.Target is Object) UnityEventTools.RemovePersistentListener(_changed, action);
            else _changed.RemoveListener(action);
#else
            _changed.RemoveListener(action);
#endif
        }

        /// <summary>
        /// Removes a listener from the Validate event.
        /// </summary>
        /// <param name="action">Listener.</param>
        public void RemoveValidation(UnityAction<Validatable<T>> action)
        {
            if (action == null) return;

#if UNITY_EDITOR
            if (action.Target is Object) UnityEventTools.RemovePersistentListener(_validate, action);
            else _validate.RemoveListener(action);
#else
            _validate.RemoveListener(action);
#endif
        }

        private void Set(object value, bool invokeChanged = true)
        {
            if (value is T) Set((T)value, invokeChanged);
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="value">New value.</param>
        /// <param name="invokeChanged">Invoke Changed event?</param>
        public void Set(T value, bool invokeChanged = true)
        {
            if (Equals(_value, value)) return;
            if (_validate != null)
            {
                if (_validatable == null) _validatable = new Validatable<T>(value);
                else _validatable.Reset(value);

                try
                {
                    _validate.Invoke(_validatable);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }

                if (_validatable.Changed)
                {
                    if (Equals(_value, _validatable.Value)) return;
                    value = _validatable;
                }
            }
            T oldValue = _value;
            _value = value;
            if (invokeChanged) InvokeChanged(oldValue, _value);
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public static bool operator ==(ObservableValue<T> a, ObservableValue<T> b)
        {
            if (a is null && b is null) return true;
            if (a is null || b is null) return false;
            return Equals(a.Value, b.Value);
        }

        public static bool operator !=(ObservableValue<T> a, ObservableValue<T> b)
        {
            if (a is null && b is null) return false;
            if (a is null || b is null) return true;
            return !Equals(a.Value, b.Value);
        }

        public static bool operator ==(ObservableValue<T> a, T b)
        {
            if (a is null) return false;
            return Equals(a.Value, b);
        }

        public static bool operator !=(ObservableValue<T> a, T b)
        {
            if (a is null) return true;
            return !Equals(a.Value, b);
        }

        public static implicit operator T(ObservableValue<T> value)
        {
            return value.Value;
        }
        
        public static ObservableValue<T> operator +(ObservableValue<T> value, UnityAction<T, T> action)
        {
            value.AddListener(action);
            return value;
        }
        
        public static ObservableValue<T> operator -(ObservableValue<T> value, UnityAction<T, T> action)
        {
            value.RemoveListener(action);
            return value;
        }
    }

}