﻿using System.Numerics;
using Sedulous.Core;
using Sedulous.Presentation.Media;

namespace Sedulous.Presentation.Documents
{
    /// <summary>
    /// Represents an element which decorates another selement.
    /// </summary>
    [UvmlKnownType]
    public abstract class Adorner : FrameworkElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Adorner"/> class.
        /// </summary>
        /// <param name="adornedElement">The element which is being adorned by this element.</param>
        protected Adorner(UIElement adornedElement)
            : base(adornedElement.FrameworkContext, null)
        {
            Contract.Require(adornedElement, nameof(adornedElement));

            this.adornedElement = adornedElement;
        }

        /// <summary>
        /// Provides the <see cref="Adorner"/> a chance to modify the transformation matrix which will be applied to it.
        /// </summary>
        /// <param name="transform">The transformation matrix which is being applied to the adorner.</param>
        public virtual void GetDesiredTransform(ref Matrix4x4 transform)
        {

        }

        /// <summary>
        /// Gets the element which is being adorned by this element.
        /// </summary>
        public UIElement AdornedElement
        {
            get { return adornedElement; }
        }

        /// <inheritdoc/>
        protected override Size2D MeasureOverride(Size2D availableSize)
        {
            var desiredSize = new Size2D(AdornedElement.RenderSize.Width, AdornedElement.RenderSize.Height);

            var childrenCount = VisualTreeHelper.GetChildrenCount(this);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(this, i) as UIElement;
                if (child != null)
                {
                    child.Measure(desiredSize);
                }
            }

            return desiredSize;
        }

        /// <inheritdoc/>
        protected override Size2D ArrangeOverride(Size2D finalSize, ArrangeOptions options)
        {
            return base.ArrangeOverride(finalSize, options);
        }

        /// <inheritdoc/>
        protected override Visual HitTestCore(Point2D point)
        {
            var result = base.HitTestCore(point);
            return (result == this) ? null : result;
        }

        /// <inheritdoc/>
        protected override RectangleD CalculateTransformedVisualBounds()
        {
            if (AdornedElement == null)
                return base.CalculateTransformedVisualBounds();

            var clipTransformMatrix = AdornedElement.GetTransformToViewMatrix();
            return UnionAbsoluteVisualBoundsWithChildrenAndApplyClipping(AdornedElement.TransformedVisualBounds, ref clipTransformMatrix);
        }        

        // Property values.
        private readonly UIElement adornedElement;
    }
}
