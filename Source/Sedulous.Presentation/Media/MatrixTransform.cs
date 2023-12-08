using System;
using System.Numerics;

namespace Sedulous.Presentation.Media
{
    /// <summary>
    /// Represents a transformation based on an arbitrary matrix.
    /// </summary>
    [UvmlKnownType]
    public sealed class MatrixTransform : Transform
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MatrixTransform"/> class.
        /// </summary>
        public MatrixTransform()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MatrixTransform"/> class.
        /// </summary>
        /// <param name="matrix">The transformation matrix that this transform represents.</param>
        public MatrixTransform(Matrix4x4 matrix)
        {
            this.Matrix = matrix;
        }

        /// <inheritdoc/>
        public override Matrix4x4 Value
        {
            get { return GetValue<Matrix4x4>(MatrixProperty); }
        }

        /// <inheritdoc/>
        public override Matrix4x4? Inverse
        {
            get { return inverse; }
        }

        /// <inheritdoc/>
        public override Boolean IsIdentity
        {
            get { return isIdentity; }
        }

        /// <summary>
        /// Gets or sets the transformation matrix that this transform represents.
        /// </summary>
        /// <value>A <see cref="Matrix4x4"/> which represents the transformation applied by this instance.
        /// The default value is <see cref="Matrix4x4.Identity"/>.</value>
        /// <remarks>
        /// <dprop>
        ///		<dpropField><see cref="MatrixProperty"/></dpropField>
        ///		<dpropStylingName>matrix</dpropStylingName>
        ///		<dpropMetadata>None</dpropMetadata>
        /// </dprop>
        /// </remarks>
        public Matrix4x4 Matrix
        {
            get { return GetValue<Matrix4x4>(MatrixProperty); }
            set { SetValue<Matrix4x4>(MatrixProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Matrix"/> dependency property.
        /// </summary>
        /// <value>The identifier for the <see cref="Matrix"/> dependency property.</value>
        public static readonly DependencyProperty MatrixProperty = DependencyProperty.Register("Matrix", typeof(Matrix4x4), typeof(MatrixTransform),
            new PropertyMetadata<Matrix4x4>(Matrix4x4.Identity, PropertyMetadataOptions.None, HandleMatrixChanged));

        /// <summary>
        /// Called when the value of the <see cref="Matrix"/> dependency property changes.
        /// </summary>
        private static void HandleMatrixChanged(DependencyObject dobj, Matrix4x4 oldValue, Matrix4x4 newValue)
        {
            var transform = (MatrixTransform)dobj;

            Matrix4x4 inverse;
            if (Matrix4x4.Invert(newValue, out inverse))
            {
                transform.inverse = inverse;
            }
            else
            {
                transform.inverse = null;
            }

            transform.isIdentity = Matrix4x4.Identity.Equals(newValue);
            transform.InvalidateDependencyObject();
        }

        // The matrix's cached inverse.
        private Matrix4x4? inverse;
        private Boolean isIdentity = true;
    }
}
