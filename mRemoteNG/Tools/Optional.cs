﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace mRemoteNG.Tools
{
    /// <summary>
    /// Represents a type that may or may not have been assigned a value.
    /// A strongly typed collection that contains either 0 or 1 values.
    /// </summary>
    /// <typeparam name="T">The underlying type that may or may not have a value</typeparam>
    public class Optional<T> : IEnumerable<T>, IComparable<Optional<T>>
    {
        private readonly T[] _optional;

        /// <summary>
        /// Create a new empty instance of Optional
        /// </summary>
        public Optional()
        {
            _optional = Array.Empty<T>();
        }

        /// <summary>
        /// Create a new instance of Optional from the given value.
        /// If the value is null, the Optional will be empty
        /// </summary>
        public Optional(T value)
        {
            _optional = value != null
                ? new[] {value}
                : Array.Empty<T>();
        }

        public override string ToString()
        {
            return _optional.Any() ? _optional.First().ToString() : "";
        }

        public static implicit operator Optional<T>(T value)
        {
            return new Optional<T>(value);
        }

        public static Optional<TOut> FromNullable<TOut>(TOut? value) where TOut : struct
        {
            return value.HasValue
                ? new Optional<TOut>(value.Value)
                : new Optional<TOut>();
        }

        /// <summary>
        /// Returns an empty <see cref="Optional{T}"/>
        /// </summary>
        public static Optional<T> Empty => new();

        #region IEnumerable

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)_optional).GetEnumerator();
        }

        #endregion

        #region IComparable

        /// <summary>
        /// Compares this <see cref="Optional{T}"/> to another instance
        /// of the same type. For purposes of comparison, empty Optional
        /// objects are treated like Null and will be valued lower than
        /// an Optional that contains a value. If both Optionals contain
        /// values, the values are compared directly.
        /// </summary>
        /// <param name="other"></param>
        public int CompareTo(Optional<T> other)
        {
            bool otherHasAnything = other.Any();
            bool thisHasAnything = _optional.Length > 0;

            // both are empty, equivalent value
            if (!thisHasAnything && !otherHasAnything)
                return 0;
            // we are empty, they are greater value
            if (!thisHasAnything)
                return -1;
            // they are empty, we are greater value
            if (!otherHasAnything)
                return 1;
            // neither are empty, compare wrapped objects directly
            if (_optional[0] is IComparable<T>)
                return ((IComparable<T>)_optional[0]).CompareTo(other.First());

            throw new ArgumentException($"Cannot compare objects. Optional type {typeof(T).FullName} is not comparable to itself");
        }

        #endregion

        #region Override Equals and GetHashCode

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            Optional<T> objAsOptional = obj as Optional<T>;
            if (objAsOptional != null)
                return Equals(objAsOptional);

            if (obj is T)
                Equals((T)obj);

            return false;
        }

        private bool Equals(Optional<T> other)
        {
            T otherObj = other.FirstOrDefault();
            T thisObj = _optional.FirstOrDefault();
            if (thisObj == null && otherObj == null)
                return true;
            if (thisObj == null)
                return false;
            return thisObj.Equals(otherObj);
        }

        public override int GetHashCode()
        {
            return _optional != null
                ? _optional.GetHashCode()
                : 0;
        }

        #endregion

        #region Operators

        public static bool operator ==(Optional<T> left, Optional<T> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Optional<T> left, Optional<T> right)
        {
            return !Equals(left, right);
        }

        #endregion
    }
}