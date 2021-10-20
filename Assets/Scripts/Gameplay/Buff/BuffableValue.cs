using System;
using System.Globalization;
using UnityEngine;

namespace Gameplay.Buff
{
    public abstract class BuffableValue<T> where T : struct, IConvertible, IComparable, IComparable<T>, IEquatable<T>, IFormattable
    {
        [SerializeField] protected T     _value;
        protected                  T     _additionalValue;
        protected                  float _magnification = 1;

        public abstract T Value
        {
            get;
            set;
        }

        public abstract void AddAdditionalValue(T   additionalValue);
        public abstract void AddMagnification(float magnification);
        
        public static implicit operator T(BuffableValue<T> n)
        {
            return n.Value;
        }
        
    }
}
