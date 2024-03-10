/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace GKCore.Observers
{
    /// <summary>
    /// A class for interacting with another Value Observer.
    /// </summary>
    /// <typeparam name="T">Type of value.</typeparam>
    [Serializable]
    public class LinkedObserver<T>
    {
        /// <summary>
        /// Target object.
        /// </summary>
        [SerializeField]
        private Object _target;
        
        /// <summary>
        /// Property path.
        /// </summary>
        [SerializeField]
        private string _propertyPath;
        
        /// <summary>
        /// Display name. Used in the inspector.
        /// </summary>
        [SerializeField]
#pragma warning disable 0414
        private string _displayName = "None";
#pragma warning restore 0414

        [NonSerialized]
        private ValueObserver<T> _valueObserver;
        
        [NonSerialized]
        private bool _isObserverInitialized;

        /// <summary>
        /// Gets or sets the Value Observer.
        /// </summary>
        public ValueObserver<T> Observer
        {
            get
            {
                return _valueObserver;
            }
            set
            {
                _valueObserver = value;
            }
        }

        /// <summary>
        /// Gets or sets the value of the Value Observer.
        /// </summary>
        public T Value
        {
            get
            {
                if (!_isObserverInitialized) InitializeObserver();
                if (_valueObserver != null) return _valueObserver.Value;
                return default;
            }
            set
            {
                Set(value);
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="target">Target object.</param>
        /// <param name="propertyPath">Property path.</param>
        /// <param name="changedCallback">Method that will be called when the value changes.</param>
        public LinkedObserver(Object target, string propertyPath, UnityAction<T, T> changedCallback = null)
        {
            _target = target;
            _propertyPath = propertyPath;
            if (changedCallback != null) Observer?.AddListener(changedCallback);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="valueObserver">Value Observer.</param>
        /// <param name="changedCallback">Method that will be called when the value changes.</param>
        public LinkedObserver(ValueObserver<T> valueObserver, UnityAction<T, T> changedCallback = null)
        {
            _valueObserver = valueObserver;
            _isObserverInitialized = true;
            if (changedCallback != null) Observer?.AddListener(changedCallback);
        }
        
        /// <summary>
        /// Adds a listener to the Changed event of the Value Observer.
        /// </summary>
        /// <param name="action">Listener.</param>
        public void AddListener(UnityAction<T, T> action)
        {
            _valueObserver?.AddListener(action);
        }
        
        /// <summary>
        /// Adds a listener to the Validate event of the Value Observer.
        /// </summary>
        /// <param name="action">Listener.</param>
        public void AddValidation(UnityAction<Validatable<T>> action)
        {
            _valueObserver?.AddValidation(action);
        }
        
        /// <summary>
        /// Disposes the object.
        /// </summary>
        public void Dispose()
        {
            _target = null;
            _propertyPath = null;
            _valueObserver = null;
        }

        private void InitializeObserver()
        {
            _isObserverInitialized = true;
            if (_target == null) return;
            if (string.IsNullOrEmpty(_propertyPath)) return;
            
            Type targetType = _target.GetType();
            FieldInfo fieldInfo = targetType.GetField(_propertyPath);
            if (fieldInfo != null)
            {
                _valueObserver = (ValueObserver<T>)fieldInfo.GetValue(_target);
            }
        }

        /// <summary>
        /// Invokes the Changed event of the Value Observer.
        /// </summary>
        public void InvokeChanged()
        {
            _valueObserver?.InvokeChanged(Value, Value);
        }
        
        /// <summary>
        /// Removes all listeners from the Changed event of the Value Observer.
        /// </summary>
        public void RemoveAllListeners()
        {
            _valueObserver?.RemoveAllListeners();
        }

        /// <summary>
        /// Removes all listeners from the Validate event of the Value Observer.
        /// </summary>
        public void RemoveAllValidation()
        {
            _valueObserver?.RemoveAllValidation();
        }

        /// <summary>
        /// Removes the listener from the Changed event of the Value Observer.
        /// </summary>
        /// <param name="action">Listener.</param>
        public void RemoveListener(UnityAction<T, T> action)
        {
            _valueObserver?.RemoveListener(action);
        }

        /// <summary>
        /// Removes the listener from the Validate event of the Value Observer.
        /// </summary>
        /// <param name="action">Listener.</param>
        public void RemoveValidation(UnityAction<Validatable<T>> action)
        {
            _valueObserver?.RemoveValidation(action);
        }

        /// <summary>
        /// Sets the value of the Value Observer.
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="invokeChanged">Invoke Changed event?</param>
        public void Set(T value, bool invokeChanged = true)
        {
            if (!_isObserverInitialized) InitializeObserver();
            if (_valueObserver == null) return;
            
            if (Equals(_valueObserver.Value, value)) return;
            _valueObserver.Set(value, invokeChanged);
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public static implicit operator T(LinkedObserver<T> observer)
        {
            return observer.Value;
        }
        
        public static LinkedObserver<T> operator +(LinkedObserver<T> value, UnityAction<T, T> action)
        {
            value.AddListener(action);
            return value;
        }
        
        public static LinkedObserver<T> operator -(LinkedObserver<T> value, UnityAction<T, T> action)
        {
            value.RemoveListener(action);
            return value;
        }
    }
}