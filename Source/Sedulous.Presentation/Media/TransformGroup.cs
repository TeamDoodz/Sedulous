using System;
using System.Numerics;

namespace Sedulous.Presentation.Media
{
    /// <summary>
    /// Represents a group of transformations.
    /// </summary>
    [UvmlKnownType]
    [UvmlDefaultProperty("Children")]
    public sealed class TransformGroup : Transform, IIndexable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransformGroup"/> class.
        /// </summary>
        public TransformGroup()
        {
            Children = new TransformCollection();
        }

        /// <inheritdoc/>
        public override Boolean WasInvalidatedAfterDigest(Int64 digestID)
        {
            foreach (var child in Children)
            {
                if (child.WasInvalidatedAfterDigest(digestID))
                    return true;
            }
            return base.WasInvalidatedAfterDigest(digestID);
        }

        /// <summary>
        /// Gets the transform at the specified index within the group.
        /// </summary>
        /// <param name="index">The index of the transform to retrieve.</param>
        /// <returns>The transform at the specified index within the group.</returns>
        public Transform this[Int32 index]
        {
            get
            {
                var children = Children;
                if (children == null)
                    throw new ArgumentOutOfRangeException("index");

                return children[index];
            }
        }

        /// <inheritdoc/>
        Object IIndexable.this[Int32 index]
        {
            get
            {
                return this[index];
            }
        }

        /// <inheritdoc/>
        Int32 IIndexable.Count
        {
            get
            {
                var children = Children;
                if (children == null)
                    return 0;

                return children.Count;
            }
        }

        /// <inheritdoc/>
        public override Matrix4x4 Value
        {
            get
            {
                var matrix   = Matrix4x4.Identity;
                var children = Children;

                if (children != null && children.Count > 0)
                {
                    Matrix4x4 value;
                    foreach (var child in children)
                    {
                        value = child.Value;
                        matrix = Matrix4x4.Multiply(matrix, value);
                    }
                }

                return matrix;
            }
        }

        /// <inheritdoc/>
        public override Matrix4x4? Inverse
        {
            get
            {
                Matrix4x4 value = Value;
                Matrix4x4 inverse;
                if (Matrix4x4.Invert(value, out inverse))
                {
                    return inverse;
                }
                return null;
            }
        }

        /// <inheritdoc/>
        public override Boolean IsIdentity
        {
            get
            {
                var children = Children;
                if (children != null)
                {
                    foreach (var child in children)
                    {
                        if (!child.IsIdentity)
                            return false;
                    }
                }
                return true;
            }
        }
        
        /// <summary>
        /// Gets or sets the transform group's list of child transformations.
        /// </summary>
        public TransformCollection Children
        {
            get { return GetValue<TransformCollection>(ChildrenProperty); }
            set { SetValue<TransformCollection>(ChildrenProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Children"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ChildrenProperty = DependencyProperty.Register("Children", typeof(TransformCollection), typeof(TransformGroup),
            new PropertyMetadata<TransformCollection>(null, PropertyMetadataOptions.None));

        /// <inheritdoc/>
        protected override void OnDigesting(FrameworkTime time)
        {
            var children = Children;
            if (children != null)
            {
                foreach (var child in children)
                {
                    child.Digest(time);
                }
            }
            base.OnDigesting(time);
        }
    }
}
