﻿using System;
using TwistedLogik.Nucleus.Data;

namespace TwistedLogik.Ultraviolet.Layout.Elements
{
    /// <summary>
    /// Represents an asset which can be loaded from either the global or local content source.
    /// </summary>
    /// <typeparam name="T">The type of asset which this object represents.</typeparam>
    public struct SourcedRef<T> : IEquatable<SourcedRef<T>> where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SourcedRef{T}"/> structure.
        /// </summary>
        /// <param name="value">The underlying value of the sourced asset.</param>
        /// <param name="source">An <see cref="AssetSource"/> value describing how to load the asset.</param>
        public SourcedRef(T value, AssetSource source)
        {
            this.value  = value;
            this.source = source;
        }

        /// <summary>
        /// Implicitly converts a sourced asset to its underlying value.
        /// </summary>
        /// <param name="sourced">The <see cref="SourcedRef{T}"/> to convert.</param>
        /// <returns>The underlying value of the sourced asset.</returns>
        public static implicit operator T(SourcedRef<T> sourced)
        {
            return sourced.Value;
        }

        /// <summary>
        /// Returns <c>true</c> if the specified sourced assets are equal.
        /// </summary>
        /// <param name="id1">The first <see cref="SourcedRef{T}"/> to compare.</param>
        /// <param name="id2">The second <see cref="SourcedRef{T}"/> to compare.</param>
        /// <returns><c>true</c> if the specified sourced assets are equal; otherwise, <c>false</c>.</returns>
        public static Boolean operator ==(SourcedRef<T> id1, SourcedRef<T> id2)
        {
            return id1.Equals(id2);
        }

        /// <summary>
        /// Returns <c>true</c> if the specified sourced assets are not equal.
        /// </summary>
        /// <param name="id1">The first <see cref="SourcedRef{T}"/> to compare.</param>
        /// <param name="id2">The second <see cref="SourcedRef{T}"/> to compare.</param>
        /// <returns><c>true</c> if the specified sourced assets are unequal; otherwise, <c>false</c>.</returns>
        public static Boolean operator !=(SourcedRef<T> id1, SourcedRef<T> id2)
        {
            return !id1.Equals(id2);
        }

        /// <summary>
        /// Parses a string into an instance of <see cref="SourcedRef{T}"/>.
        /// </summary>
        /// <param name="str">The string to parse.</param>
        /// <returns>The <see cref="SourcedRef{T}"/> instance that was created from the specified string.</returns>
        public static SourcedRef<T> Parse(String str)
        {
            return Parse(str, null);
        }

        /// <summary>
        /// Parses a string into an instance of <see cref="SourcedRef{T}"/>.
        /// </summary>
        /// <param name="str">The string to parse.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>The <see cref="SourcedRef{T}"/> instance that was created from the specified string.</returns>
        public static SourcedRef<T> Parse(String str, IFormatProvider provider)
        {
            var source = AssetSource.Global;

            if (str.EndsWith(" local"))
            {
                source = AssetSource.Local;
                str    = str.Substring(0, str.Length - " local".Length);
            }
            else if (str.EndsWith(" global"))
            {
                source = AssetSource.Global;
                str    = str.Substring(0, str.Length - " global".Length);
            }

            var underlyingValue = (T)ObjectResolver.FromString(str, typeof(T), provider);
            return new SourcedRef<T>(underlyingValue, source);
        }

        /// <inheritdoc/>
        public override Int32 GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                hash = hash * 23 + value.GetHashCode();
                hash = hash * 23 + source.GetHashCode();
                return hash;
            }
        }

        /// <inheritdoc/>
        public override String ToString()
        {
            return String.Format("{0} {1}", value, source.ToString().ToLowerInvariant());
        }

        /// <inheritdoc/>
        public override Boolean Equals(Object obj)
        {
            return obj is SourcedRef<T> && Equals((SourcedRef<T>)obj);
        }

        /// <inheritdoc/>
        public Boolean Equals(SourcedRef<T> other)
        {
            return
                this.value  == other.value &&
                this.source == other.source;
        }

        /// <summary>
        /// Gets the sourced asset value.
        /// </summary>
        public T Value
        {
            get { return value; }
        }

        /// <summary>
        /// Gets a <see cref="AssetSource"/> value indicating how to load the asset.
        /// </summary>
        public AssetSource Source
        {
            get { return source; }
        }

        // Property values.
        private readonly T value;
        private readonly AssetSource source;
    }
}
