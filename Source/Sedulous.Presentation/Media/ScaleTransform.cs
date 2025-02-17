﻿using System;
using System.Numerics;
using Sedulous.Core;

namespace Sedulous.Presentation.Media
{
    /// <summary>
    /// Represents a transformation which scales an object.
    /// </summary>
    [UvmlKnownType]
    public sealed class ScaleTransform : Transform
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
        /// Gets or sets the transform's scaling factor along the x-axis.
        /// </summary>
        /// <value>A <see cref="Single"/> value that represents the scaling factor applied
        /// along the transformed object's x-axis. The default value is 1.0.</value>
        /// <remarks>
        /// <dprop>
        ///		<dpropField><see cref="ScaleXProperty"/></dpropField>
        ///		<dpropStylingName>scale-x</dpropStylingName>
        ///		<dpropMetadata>None</dpropMetadata>
        /// </dprop>
        /// </remarks>
        public Single ScaleX
        {
            get { return GetValue<Single>(ScaleXProperty); }
            set { SetValue(ScaleXProperty, value); }
        }

        /// <summary>
        /// Gets or sets the transform's scaling factor along the y-axis.
        /// </summary>
        /// <value>A <see cref="Single"/> value that represents the scaling factor applied
        /// along the transformed object's y-axis. The default value is 1.0.</value>
        /// <remarks>
        /// <dprop>
        ///		<dpropField><see cref="ScaleYProperty"/></dpropField>
        ///		<dpropStylingName>scale-y</dpropStylingName>
        ///		<dpropMetadata>None</dpropMetadata>
        /// </dprop>
        /// </remarks>
        public Single ScaleY
        {
            get { return GetValue<Single>(ScaleYProperty); }
            set { SetValue(ScaleYProperty, value); }
        }

        /// <summary>
        /// Gets or sets the x-coordinate around which the object is scaled.
        /// </summary>
        /// <value>A <see cref="Double"/> value that represents the x-coordinate, in device-independent pixels,
        /// around which the object is scaled. The default value is 0.0.</value>
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
        /// Gets or sets the y-coordinate around which the object is scaled.
        /// </summary>
        /// <value>A <see cref="Double"/> value that represents the y-coordinate, in device-independent pixels,
        /// around which the object is scaled. The default value is 0.0.</value>
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
        /// Identifies the <see cref="ScaleX"/> dependency property.
        /// </summary>
        /// <value>The identifier for the <see cref="ScaleX"/> dependency property.</value>
        public static readonly DependencyProperty ScaleXProperty = DependencyProperty.Register("ScaleX", typeof(Single), typeof(ScaleTransform),
            new PropertyMetadata<Single>(CommonBoxedValues.Single.One, PropertyMetadataOptions.None, HandleScaleChanged));

        /// <summary>
        /// Identifies the <see cref="ScaleY"/> dependency property.
        /// </summary>
        /// <value>The identifier for the <see cref="ScaleY"/> dependency property.</value>
        public static readonly DependencyProperty ScaleYProperty = DependencyProperty.Register("ScaleY", typeof(Single), typeof(ScaleTransform),
            new PropertyMetadata<Single>(CommonBoxedValues.Single.One, PropertyMetadataOptions.None, HandleScaleChanged));

        /// <summary>
        /// Identifies the <see cref="CenterX"/> dependency property.
        /// </summary>
        /// <value>The identifier for the <see cref="CenterX"/> dependency property.</value>
        public static readonly DependencyProperty CenterXProperty = DependencyProperty.Register("CenterX", typeof(Double), typeof(ScaleTransform),
            new PropertyMetadata<Double>(CommonBoxedValues.Double.Zero, PropertyMetadataOptions.None, HandleCenterChanged));

        /// <summary>
        /// Identifies the <see cref="CenterY"/> dependency property.
        /// </summary>
        /// <value>The identifier for the <see cref="CenterY"/> dependency property.</value>
        public static readonly DependencyProperty CenterYProperty = DependencyProperty.Register("CenterY", typeof(Double), typeof(ScaleTransform),
            new PropertyMetadata<Double>(CommonBoxedValues.Double.Zero, PropertyMetadataOptions.None, HandleCenterChanged));

        /// <summary>
        /// Called when the value of the <see cref="ScaleX"/> or <see cref="ScaleY"/> dependency properties change.
        /// </summary>
        private static void HandleScaleChanged(DependencyObject dobj, Single oldValue, Single newValue)
        {
            var transform = (ScaleTransform)dobj;
            transform.UpdateValue();
            transform.InvalidateDependencyObject();
        }

        /// <summary>
        /// Called when the value of the <see cref="CenterX"/> or <see cref="CenterY"/> dependency properties change.
        /// </summary>
        private static void HandleCenterChanged(DependencyObject dobj, Double oldValue, Double newValue)
        {
            var transform = (ScaleTransform)dobj;
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

            var hasCenter = (centerX != 0 || centerY != 0);
            if (hasCenter)
            {
                var mtxScale  = Matrix4x4.CreateScale(ScaleX, ScaleY, 1f);
                var mtxTransformCenter = Matrix4x4.CreateTranslation(-centerX, -centerY, 0f);
                var mtxTransformCenterInverse = Matrix4x4.CreateTranslation(centerX, centerY, 0f);

                Matrix4x4 mtxResult = mtxTransformCenter * mtxScale * mtxTransformCenterInverse;

                this.value = mtxResult;
            }
            else
            {
                this.value = Matrix4x4.CreateScale(ScaleX, ScaleY, 1f);
            }

            Matrix4x4 invertedValue;
            this.inverse = Matrix4x4.Invert(value, out invertedValue) ? invertedValue : (Matrix4x4?)null;
            this.isIdentity = Matrix4x4.Identity.Equals(value);
        }

        // Property values.
        private Matrix4x4 value = Matrix4x4.Identity;
        private Matrix4x4? inverse;
        private Boolean isIdentity;
    }
}
