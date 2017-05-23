using System;
using Ultraviolet.Core;

namespace Ultraviolet.Graphics.Graphics2D
{
    partial struct SpriteAnimationName : IEquatable<SpriteAnimationName>
    {
        /// <inheritdoc/>
        public override Int32 GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                hash = hash * 23 + animationName?.GetHashCode() ?? 0;
                hash = hash * 23 + animationIndex.GetHashCode();
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
        public static Boolean operator ==(SpriteAnimationName v1, SpriteAnimationName v2)
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
        public static Boolean operator !=(SpriteAnimationName v1, SpriteAnimationName v2)
        {
            return !v1.Equals(v2);
        }
        
        /// <inheritdoc/>
        [Preserve]
        public override Boolean Equals(Object other)
        {
            return (other is SpriteAnimationName x) ? Equals(x) : false;
        }
        
        /// <inheritdoc/>
        [Preserve]
        public Boolean Equals(SpriteAnimationName other)
        {
            return
                this.animationName == other.animationName &&
                this.animationIndex == other.animationIndex;
        }
    }
}
