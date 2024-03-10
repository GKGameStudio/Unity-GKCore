/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GKCore.Observers
{
    /// <summary>
    /// Base class for linked methods.
    /// </summary>
    public abstract class LinkedMethod
    {
        /// <summary>
        /// Should exceptions be thrown when something goes wrong.
        /// </summary>
        public static bool ThrowExceptions = false;
        
        /// <summary>
        /// Target object.
        /// </summary>
        [SerializeField]
        protected Object _target;
        
        /// <summary>
        /// Method name.
        /// </summary>
        [SerializeField]
        protected  string _methodName;
        
        [NonSerialized]
        protected  MethodInfo _methodInfo;
        
        [NonSerialized]
        protected bool _isInitialized;

        protected LinkedMethod()
        {
            
        }
        
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="target">Target object.</param>
        /// <param name="methodName">Method name.</param>
        public LinkedMethod(Object target, string methodName)
        {
            _target = target;
            _methodName = methodName;
        }
        
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="target">Target object.</param>
        /// <param name="methodInfo">Method info.</param>
        public LinkedMethod(Object target, MethodInfo methodInfo)
        {
            _target = target;
            _methodInfo = methodInfo;
            _methodName = methodInfo.Name;
            _isInitialized = true;
        }

        private bool CompareMethod(MethodInfo method, Type resultType, Type[] types)
        {
            if (method.Name != _methodName) return false;
            if (method.ReturnType != resultType) return false;
            ParameterInfo[] parameters = method.GetParameters();
            if (parameters.Length != types.Length) return false;
            return parameters.Select(p => p.ParameterType).SequenceEqual(types);
        }
        
        /// <summary>
        /// Disposes the object.
        /// </summary>
        public void Dispose()
        {
            _target = null;
            _methodName = null;
            _methodInfo = null;
            _isInitialized = false;
        }

        protected void Initialize()
        {
            _isInitialized = true;
            if (_target == null || string.IsNullOrEmpty(_methodName)) return;

            Type[] genericArguments = GetType().GetGenericArguments();

            Type resultType = genericArguments.Last();
            Type[] types = genericArguments.Take(genericArguments.Length - 1).ToArray();

            MethodInfo[] methods = _target.GetType().GetMethods(ReflectionHelper.BindingFlags);
            for (int i = 0; i < methods.Length; i++)
            {
                MethodInfo method = methods[i];
                if (!CompareMethod(method, resultType, types)) continue;
                
                _methodInfo = method;
                return;
            }
        }
        
        protected object InvokeInternal(params object[] parameters)
        {
            if (!_isInitialized) Initialize();

            if (_methodInfo != null)
            {
                try
                {
                    return _methodInfo.Invoke(_target, parameters);
                }
                catch (Exception e)
                {
                    Throw(e);
                }
            }
            else
            {
                if (_target == null)
                {
                    Throw(new NullReferenceException("Target is null"));
                }
                else if (string.IsNullOrEmpty(_methodName))
                {
                    Throw(new Exception("Method name is empty"));
                }
                else
                {
                    Throw(new MissingMethodException(GetMissingMethodMessage()));
                }
            }

            return null;
        }

        private string GetMissingMethodMessage()
        {
            Type[] types = GetType().GenericTypeArguments;

            StringBuilder sb = new StringBuilder();
            sb.Append(TypeHelper.GetTypeName(types.Last().Name));
            sb.Append(" ");
            sb.Append(_target.GetType().Name);
            sb.Append(".");
            sb.Append(_methodName);
            sb.Append("(");
            for (int i = 0; i < types.Length - 1; i++)
            {
                if (i > 0) sb.Append(", ");
                sb.Append(TypeHelper.GetTypeName(types[i].Name));
            }

            sb.Append(")");
            return sb.ToString();
        }

        private static void Throw(Exception e)
        {
            if (ThrowExceptions) throw e;
            Debug.LogException(e);
        }
    }
    
    /// <summary>
    /// Linked method without parameters.
    /// </summary>
    /// <typeparam name="TResult">Type of result.</typeparam>
    [Serializable]
    public class LinkedMethod<TResult>: LinkedMethod
    {
        /// <summary>
        /// Invoke method.
        /// </summary>
        /// <returns>Result of method.</returns>
        public TResult Invoke()
        {
            return (TResult)InvokeInternal();
        }
    }
    
    /// <summary>
    /// Linked method with one parameter.
    /// </summary>
    /// <typeparam name="T">Type of parameter.</typeparam>
    /// <typeparam name="TResult">Type of result.</typeparam>
    [Serializable]
    public class LinkedMethod<T, TResult>: LinkedMethod
    {
        /// <summary>
        /// Invoke method.
        /// </summary>
        /// <param name="p">Parameter.</param>
        /// <returns></returns>
        public TResult Invoke(T p)
        {
            object result = InvokeInternal(p);
            if (result != null) return (TResult)result;
            return default;
        }
    }
    
    /// <summary>
    /// Linked method with two parameters.
    /// </summary>
    /// <typeparam name="T1">Type of first parameter.</typeparam>
    /// <typeparam name="T2">Type of second parameter.</typeparam>
    /// <typeparam name="TResult">Type of result.</typeparam>
    [Serializable]
    public class LinkedMethod<T1, T2, TResult>: LinkedMethod
    {
        /// <summary>
        /// Invoke method.
        /// </summary>
        /// <param name="p1">The first parameter.</param>
        /// <param name="p2">The second parameter.</param>
        /// <returns>Result of method.</returns>
        public TResult Invoke(T1 p1, T2 p2)
        {
            object result = InvokeInternal(p1, p2);
            if (result != null) return (TResult)result;
            return default;
        }
    }
    
    /// <summary>
    /// Linked method with three parameters.
    /// </summary>
    /// <typeparam name="T1">Type of first parameter.</typeparam>
    /// <typeparam name="T2">Type of second parameter.</typeparam>
    /// <typeparam name="T3">Type of third parameter.</typeparam>
    /// <typeparam name="TResult">Type of result.</typeparam>
    [Serializable]
    public class LinkedMethod<T1, T2, T3, TResult>: LinkedMethod
    {
        /// <summary>
        /// Invoke method.
        /// </summary>
        /// <param name="p1">The first parameter.</param>
        /// <param name="p2">The second parameter.</param>
        /// <param name="p3">The third parameter.</param>
        /// <returns>Result of method.</returns>
        public TResult Invoke(T1 p1, T2 p2, T3 p3)
        {
            object result = InvokeInternal(p1, p2, p3);
            if (result != null) return (TResult)result;
            return default;
        }
    }

    /// <summary>
    /// Linked method with four parameters.
    /// </summary>
    /// <typeparam name="T1">Type of first parameter.</typeparam>
    /// <typeparam name="T2">Type of second parameter.</typeparam>
    /// <typeparam name="T3">Type of third parameter.</typeparam>
    /// <typeparam name="T4">Type of fourth parameter.</typeparam>
    /// <typeparam name="TResult">Type of result.</typeparam>
    [Serializable]
    public class LinkedMethod<T1, T2, T3, T4, TResult>: LinkedMethod
    {
        /// <summary>
        /// Invoke method.
        /// </summary>
        /// <param name="p1">The first parameter.</param>
        /// <param name="p2">The second parameter.</param>
        /// <param name="p3">The third parameter.</param>
        /// <param name="p4">The fourth parameter.</param>
        /// <returns>Result of method.</returns>
        public TResult Invoke(T1 p1, T2 p2, T3 p3, T4 p4)
        {
            object result = InvokeInternal(p1, p2, p3, p4);
            if (result != null) return (TResult)result;
            return default;
        }
    }

    /// <summary>
    /// Linked method with five parameters.
    /// </summary>
    /// <typeparam name="T1">Type of first parameter.</typeparam>
    /// <typeparam name="T2">Type of second parameter.</typeparam>
    /// <typeparam name="T3">Type of third parameter.</typeparam>
    /// <typeparam name="T4">Type of fourth parameter.</typeparam>
    /// <typeparam name="T5">Type of fifth parameter.</typeparam>
    /// <typeparam name="TResult">Type of result.</typeparam>
    [Serializable]
    public class LinkedMethod<T1, T2, T3, T4, T5, TResult>: LinkedMethod
    {
        /// <summary>
        /// Invoke method.
        /// </summary>
        /// <param name="p1">The first parameter.</param>
        /// <param name="p2">The second parameter.</param>
        /// <param name="p3">The third parameter.</param>
        /// <param name="p4">The fourth parameter.</param>
        /// <param name="p5">The fifth parameter.</param>
        /// <returns>Result of method.</returns>
        public TResult Invoke(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5)
        {
            object result = InvokeInternal(p1, p2, p3, p4, p5);
            if (result != null) return (TResult)result;
            return default;
        }
    }

    /// <summary>
    /// Linked method with six parameters.
    /// </summary>
    /// <typeparam name="T1">Type of first parameter.</typeparam>
    /// <typeparam name="T2">Type of second parameter.</typeparam>
    /// <typeparam name="T3">Type of third parameter.</typeparam>
    /// <typeparam name="T4">Type of fourth parameter.</typeparam>
    /// <typeparam name="T5">Type of fifth parameter.</typeparam>
    /// <typeparam name="T6">Type of sixth parameter.</typeparam>
    /// <typeparam name="TResult">Type of result.</typeparam>
    [Serializable]
    public class LinkedMethod<T1, T2, T3, T4, T5, T6, TResult>: LinkedMethod
    {
        /// <summary>
        /// Invoke method.
        /// </summary>
        /// <param name="p1">The first parameter.</param>
        /// <param name="p2">The second parameter.</param>
        /// <param name="p3">The third parameter.</param>
        /// <param name="p4">The fourth parameter.</param>
        /// <param name="p5">The fifth parameter.</param>
        /// <param name="p6">The sixth parameter.</param>
        /// <returns>Result of method.</returns>
        public TResult Invoke(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, 
            T6 p6)
        {
            object result = InvokeInternal(p1, p2, p3, p4, p5, p6);
            if (result != null) return (TResult)result;
            return default;
        }
    }

    /// <summary>
    /// Linked method with seven parameters.
    /// </summary>
    /// <typeparam name="T1">Type of first parameter.</typeparam>
    /// <typeparam name="T2">Type of second parameter.</typeparam>
    /// <typeparam name="T3">Type of third parameter.</typeparam>
    /// <typeparam name="T4">Type of fourth parameter.</typeparam>
    /// <typeparam name="T5">Type of fifth parameter.</typeparam>
    /// <typeparam name="T6">Type of sixth parameter.</typeparam>
    /// <typeparam name="T7">Type of seventh parameter.</typeparam>
    /// <typeparam name="TResult">Type of result.</typeparam>
    [Serializable]
    public class LinkedMethod<T1, T2, T3, T4, T5, T6, T7, TResult>: LinkedMethod
    {
        /// <summary>
        /// Invoke method.
        /// </summary>
        /// <param name="p1">The first parameter.</param>
        /// <param name="p2">The second parameter.</param>
        /// <param name="p3">The third parameter.</param>
        /// <param name="p4">The fourth parameter.</param>
        /// <param name="p5">The fifth parameter.</param>
        /// <param name="p6">The sixth parameter.</param>
        /// <param name="p7">The seventh parameter.</param>
        /// <returns>Result of method.</returns>
        public TResult Invoke(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, 
            T6 p6, T7 p7)
        {
            object result = InvokeInternal(p1, p2, p3, p4, p5, p6, p7);
            if (result != null) return (TResult)result;
            return default;
        }
    }

    /// <summary>
    /// Linked method with eight parameters.
    /// </summary>
    /// <typeparam name="T1">Type of first parameter.</typeparam>
    /// <typeparam name="T2">Type of second parameter.</typeparam>
    /// <typeparam name="T3">Type of third parameter.</typeparam>
    /// <typeparam name="T4">Type of fourth parameter.</typeparam>
    /// <typeparam name="T5">Type of fifth parameter.</typeparam>
    /// <typeparam name="T6">Type of sixth parameter.</typeparam>
    /// <typeparam name="T7">Type of seventh parameter.</typeparam>
    /// <typeparam name="T8">Type of eighth parameter.</typeparam>
    /// <typeparam name="TResult">Type of result.</typeparam>
    [Serializable]
    public class LinkedMethod<T1, T2, T3, T4, T5, T6, T7, T8, TResult>: LinkedMethod
    {
        /// <summary>
        /// Invoke method.
        /// </summary>
        /// <param name="p1">The first parameter.</param>
        /// <param name="p2">The second parameter.</param>
        /// <param name="p3">The third parameter.</param>
        /// <param name="p4">The fourth parameter.</param>
        /// <param name="p5">The fifth parameter.</param>
        /// <param name="p6">The sixth parameter.</param>
        /// <param name="p7">The seventh parameter.</param>
        /// <param name="p8">The eighth parameter.</param>
        /// <returns>Result of method.</returns>
        public TResult Invoke(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8)
        {
            object result = InvokeInternal(p1, p2, p3, p4, p5, p6, p7, p8);
            if (result != null) return (TResult)result;
            return default;
        }
    }

    /// <summary>
    /// Linked method with nine parameters.
    /// </summary>
    /// <typeparam name="T1">Type of first parameter.</typeparam>
    /// <typeparam name="T2">Type of second parameter.</typeparam>
    /// <typeparam name="T3">Type of third parameter.</typeparam>
    /// <typeparam name="T4">Type of fourth parameter.</typeparam>
    /// <typeparam name="T5">Type of fifth parameter.</typeparam>
    /// <typeparam name="T6">Type of sixth parameter.</typeparam>
    /// <typeparam name="T7">Type of seventh parameter.</typeparam>
    /// <typeparam name="T8">Type of eighth parameter.</typeparam>
    /// <typeparam name="T9">Type of ninth parameter.</typeparam>
    /// <typeparam name="TResult">Type of result.</typeparam>
    [Serializable]
    public class LinkedMethod<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>: LinkedMethod
    {
        /// <summary>
        /// Invoke method.
        /// </summary>
        /// <param name="p1">The first parameter.</param>
        /// <param name="p2">The second parameter.</param>
        /// <param name="p3">The third parameter.</param>
        /// <param name="p4">The fourth parameter.</param>
        /// <param name="p5">The fifth parameter.</param>
        /// <param name="p6">The sixth parameter.</param>
        /// <param name="p7">The seventh parameter.</param>
        /// <param name="p8">The eighth parameter.</param>
        /// <param name="p9">The ninth parameter.</param>
        /// <returns>Result of method.</returns>
        public TResult Invoke(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, 
            T6 p6, T7 p7, T8 p8, T9 p9)
        {
            object result = InvokeInternal(p1, p2, p3, p4, p5, p6, p7, p8, p9);
            if (result != null) return (TResult)result;
            return default;
        }
    }

    /// <summary>
    /// Linked method with nine parameters.
    /// </summary>
    /// <typeparam name="T1">Type of first parameter.</typeparam>
    /// <typeparam name="T2">Type of second parameter.</typeparam>
    /// <typeparam name="T3">Type of third parameter.</typeparam>
    /// <typeparam name="T4">Type of fourth parameter.</typeparam>
    /// <typeparam name="T5">Type of fifth parameter.</typeparam>
    /// <typeparam name="T6">Type of sixth parameter.</typeparam>
    /// <typeparam name="T7">Type of seventh parameter.</typeparam>
    /// <typeparam name="T8">Type of eighth parameter.</typeparam>
    /// <typeparam name="T9">Type of ninth parameter.</typeparam>
    /// <typeparam name="T10">Type of tenth parameter.</typeparam>
    /// <typeparam name="TResult">Type of result.</typeparam>
    [Serializable]
    public class LinkedMethod<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>: LinkedMethod
    {
        /// <summary>
        /// Invoke method.
        /// </summary>
        /// <param name="p1">The first parameter.</param>
        /// <param name="p2">The second parameter.</param>
        /// <param name="p3">The third parameter.</param>
        /// <param name="p4">The fourth parameter.</param>
        /// <param name="p5">The fifth parameter.</param>
        /// <param name="p6">The sixth parameter.</param>
        /// <param name="p7">The seventh parameter.</param>
        /// <param name="p8">The eighth parameter.</param>
        /// <param name="p9">The ninth parameter.</param>
        /// <param name="p10">The tenth parameter.</param>
        /// <returns>Result of method.</returns>
        public TResult Invoke(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, 
            T6 p6, T7 p7, T8 p8, T9 p9, T10 p10)
        {
            object result = InvokeInternal(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10);
            if (result != null) return (TResult)result;
            return default;
        }
    }
}