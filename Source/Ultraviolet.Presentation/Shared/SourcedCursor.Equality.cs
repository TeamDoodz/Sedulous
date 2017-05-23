using System;
using Ultraviolet.Core;

namespace Ultraviolet.Presentation
{
    partial struct SourcedCursor : IEquatable<SourcedCursor>
    {
        /// <inheritdoc/>
        public override Int32 GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                hash = hash * 23 + resource?.GetHashCode() ?? 0;
                hash = hash * 23 + source.GetHashCode();
                return hash;
            }
        }
        
        /// <summary>
        /// Compares two objects to determine whether they are equal.
        /// </summary>
        /// <param name="v1">The first value to compare.</param>
        /// <param name="v2">The second value to compare.</param>
        /// <returns><see langword="true"/> if the two values are equal; otherwise, <see langword="false"/>.</returns>
        [Preserve]
        public static Boolean operator ==(SourcedCursor v1, SourcedCursor v2)
        {
            return v1.Equals(v2);
        }
        
        /// <summary>
        /// Compares two objects to determine whether they are unequal.
        /// </summary>
        /// <param name="v1">The first value to compare.</param>
        /// <param name="v2">The second value to compare.</param>
        /// <returns><see langword="true"/> if the two values are unequal; otherwise, <see langword="false"/>.</returns>
        [Preserve]
        public static Boolean operator !=(SourcedCursor v1, SourcedCursor v2)
        {
            return !v1.Equals(v2);
        }
        
        /// <inheritdoc/>
        [Preserve]
        public override Boolean Equals(Object other)
        {
            return (other is SourcedCursor x) ? Equals(x) : false;
        }
        
        /// <inheritdoc/>
        [Preserve]
        public Boolean Equals(SourcedCursor other)
        {
            return
                this.resource == other.resource &&
                this.source == other.source;
        }
    }
}
