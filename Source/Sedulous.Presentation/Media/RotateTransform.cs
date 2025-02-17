﻿using System;
using System.Numerics;
using Sedulous.Core;

namespace Sedulous.Presentation.Media
{
    /// <summary>
    /// Represents a transformation which rotates an object around the specified origin.
    /// </summary>
    [UvmlKnownType]
    public sealed class RotateTransform : Transform
    {
        /// <inheritdoc/>
        public override Matrix4x4 Value
        {
            get { return value; }
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
        /// Gets tor sets the angle of rotation in degrees.
        /// </summary>
        /// <value>A <see cref="Single"/> value that represents the transformation's angle
        /// of rotation in degrees. The default value is 0.</value>
        /// <remarks>
        /// <dprop>
        ///		<dpropField><see cref="AngleProperty"/></dpropField>
        ///		<dpropStylingName>angle</dpropStylingName>
        ///		<dpropMetadata>None</dpropMetadata>
        /// </dprop>
        /// </remarks>
        public Single Angle
        {
            get { return GetValue<Single>(AngleProperty); }
            set { SetValue(AngleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the x-coordinate around which the object is rotated.
        /// </summary>
        /// <value>A <see cref="Double"/> value that represents the x-coordinate, in device-independent pixels,
        /// around which the object is rotated. The default value is 0.0.</value>
        /// <remarks>
        /// <dprop>
        ///		<dpropField><see cref="CenterXProperty"/></dpropField>
        ///		<dpropStylingName>center-x</dpropStylingName>
        ///		<dpropMetadata>None</dpropMetadata>
        /// </dprop>
        /// </remarks>
        public Double CenterX
        {
            get { return GetValue<Double>(CenterXProperty); }
            set { SetValue(CenterXProperty, value); }
        }

        /// <summary>
        /// Gets or sets the y-coordinate around which the object is rotated.
        /// </summary>
        /// <value>A <see cref="Double"/> value that represents the y-coordinate, in device-independent pixels,
        /// around which the object is rotated. The default value is 0.0.</value>
        /// <remarks>
        /// <dprop>
        ///		<dpropField><see cref="CenterYProperty"/></dpropField>
        ///		<dpropStylingName>center-y</dpropStylingName>
        ///		<dpropMetadata>None</dpropMetadata>
        /// </dprop>
        /// </remarks>
        public Double CenterY
        {
            get { return GetValue<Double>(CenterYProperty); }
            set { SetValue(CenterYProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Angle"/> dependency property.
        /// </summary>
        /// <value>The identifier for the <see cref="Angle"/> dependency property.</value>
        public static readonly DependencyProperty AngleProperty = DependencyProperty.Register("Angle", typeof(Single), typeof(RotateTransform),
            new PropertyMetadata<Single>(CommonBoxedValues.Single.Zero, PropertyMetadataOptions.None, HandleAngleChanged));

        /// <summary>
        /// Identifies the <see cref="CenterX"/> dependency property.
        /// </summary>
        /// <value>The identifier for the <see cref="CenterX"/> dependency property.</value>
        public static readonly DependencyProperty CenterXProperty = DependencyProperty.Register("CenterX", typeof(Double), typeof(RotateTransform),
            new PropertyMetadata<Double>(CommonBoxedValues.Double.Zero, PropertyMetadataOptions.None, HandleCenterChanged));

        /// <summary>
        /// Identifies the <see cref="CenterY"/> dependency property.
        /// </summary>
        /// <value>The identifier for the <see cref="CenterY"/> dependency property.</value>
        public static readonly DependencyProperty CenterYProperty = DependencyProperty.Register("CenterY", typeof(Double), typeof(RotateTransform),
            new PropertyMetadata<Double>(CommonBoxedValues.Double.Zero, PropertyMetadataOptions.None, HandleCenterChanged));

        /// <summary>
        /// Called when the value of the <see cref="Angle"/> dependency property changes.
        /// </summary>
        private static void HandleAngleChanged(DependencyObject dobj, Single oldValue, Single newValue)
        {
            var transform = (RotateTransform)dobj;
            transform.UpdateValue();
            transform.InvalidateDependencyObject();
        }

        /// <summary>
        /// Called when the value of the <see cref="CenterX"/> or <see cref="CenterY"/> dependency properties change.
        /// </summary>
        private static void HandleCenterChanged(DependencyObject dobj, Double oldValue, Double newValue)
        {
            var transform = (RotateTransform)dobj;
            transform.UpdateValue();
            transform.InvalidateDependencyObject();
        }

        /// <summary>
        /// Updates the transform's cached value.
        /// </summary>
        private void UpdateValue()
        {
            var centerX = (Single)CenterX;
            var centerY = (Single)CenterY;

            var degrees = MathUtility.IsApproximatelyZero(Angle % 360) ? 0f : Angle;
            var radians = Radians.FromDegrees(degrees);

            var hasCenter = (centerX != 0 || centerY != 0);
            if (hasCenter)
            {
                var mtxRotate = Matrix4x4.CreateRotationZ(radians);
                var mtxTransformCenter = Matrix4x4.CreateTranslation(-centerX, -centerY, 0f);
                var mtxTransformCenterInverse = Matrix4x4.CreateTranslation(centerX, centerY, 0f);

                Matrix4x4 mtxResult = mtxTransformCenter * mtxRotate * mtxTransformCenterInverse;

                this.value = mtxResult;
            }
            else
            {
                this.value = Matrix4x4.CreateRotationZ(radians);
            }

            Matrix4x4 invertedValue;
            this.inverse = Matrix4x4.Invert(value, out invertedValue) ? invertedValue : (Matrix4x4?)null;
            this.isIdentity = Matrix4x4.Identity.Equals(value);
        }

        // Property values.
        private Matrix4x4 value = Matrix4x4.Identity;
        private Matrix4x4? inverse;
        private Boolean isIdentity = true;
    }
}
