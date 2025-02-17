using System;

namespace Sedulous.Core.Data
{
    partial struct ResolvedDataObjectReference
    {
        /// <inheritdoc/>
        public override Int32 GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                hash = hash * 23 + value.GetHashCode();
                hash = hash * 23 + source?.GetHashCode() ?? 0;
                return hash;
            }
        }
        
        /// <summary>
        /// Compares two objects to determine whether they are equal.
        /// </summary>
        /// <param name="v1">The first value to compare.</param>
        /// <param name="v2">The second value to compare.</param>
        /// <returns><see langword="true"/> if the two values are equal; otherwise, <see langword="false"/>.</returns>
        public static Boolean operator ==(ResolvedDataObjectReference v1, ResolvedDataObjectReference v2)
        {
            return v1.Equals(v2);
        }
        
        /// <summary>
        /// Compares two objects to determine whether they are unequal.
        /// </summary>
        /// <param name="v1">The first value to compare.</param>
        /// <param name="v2">The second value to compare.</param>
        /// <returns><see langword="true"/> if the two values are unequal; otherwise, <see langword="false"/>.</returns>
        public static Boolean operator !=(ResolvedDataObjectReference v1, ResolvedDataObjectReference v2)
        {
            return !v1.Equals(v2);
        }
        
        /// <inheritdoc/>
        public override Boolean Equals(Object other)
        {
            return (other is ResolvedDataObjectReference x) ? Equals(x) : false;
        }
        
        /// <inheritdoc/>
        public Boolean Equals(ResolvedDataObjectReference other)
        {
            return
                this.value == other.value &&
                this.source == other.source;
        }
    }
}
