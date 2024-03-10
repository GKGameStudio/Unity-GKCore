/*           INFINITY CODE          */
/*     https://infinity-code.com    */

namespace GKCore.Observers
{
    /// <summary>
    /// A class for validating values.
    /// </summary>
    /// <typeparam name="T">Type of value.</typeparam>
    public class Validatable<T>
    {
        private bool _changed;
        private T _value;
        
        /// <summary>
        /// Returns true if the value has changed.
        /// </summary>
        public bool Changed
        {
            get { return _changed; }
        }
        
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public T Value
        {
            get { return _value; }
            set
            {
                if (Equals(_value, value)) return;
                _value = value;
                _changed = true;
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="value">Initial value.</param>
        public Validatable(T value)
        {
            _value = value;
        }

        /// <summary>
        /// Resets the value and the Changed flag.
        /// </summary>
        /// <param name="value">New value.</param>
        public void Reset(T value)
        {
            _value = value;
            _changed = false;
        }

        public static implicit operator T(Validatable<T> value)
        {
            return value.Value;
        }
    }
}