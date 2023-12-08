using System;
using System.Numerics;

namespace Sedulous.Presentation.Media
{
    /// <summary>
    /// Represents an identity transformation.
    /// </summary>
    [UvmlKnownType]
    public sealed class IdentityTransform : Transform
    {
        /// <inheritdoc/>
        public override Matrix4x4 Value
        {
            get { return Matrix4x4.Identity; }
        }

        /// <inheritdoc/>
        public override Matrix4x4? Inverse
        {
            get { return Matrix4x4.Identity; }
        }

        /// <inheritdoc/>
        public override Boolean IsIdentity
        {
            get { return true; }
        }
    }
}
