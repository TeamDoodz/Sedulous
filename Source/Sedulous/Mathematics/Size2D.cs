using System;
using System.Drawing;
using System.Numerics;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Sedulous
{
    /// <summary>
    /// Represents a two-dimensional area with double-precision floating point components.
    /// </summary>
    [Serializable, DataContract]
    public partial struct Size2D : IEquatable<Size2D>, IInterpolatable<Size2D>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Size2D"/> structure.
        /// </summary>
        /// <param name="width">The area's width.</param>
        /// <param name="height">The area's height.</param>
        [JsonConstructor]
        public Size2D(Double width, Double height)
        {
            this.Width = width;
            this.Height = height;
        }
        
        /// <summary>
        /// Adds a <see cref="Size2D"/> to another <see cref="Size2D"/>.
        /// </summary>
        /// <param name="s1">The <see cref="Size2D"/> on the left side of the operator.</param>
        /// <param name="s2">The <see cref="Size2D"/> on the right side of the operator.</param>
        /// <returns>The result of adding the two instances.</returns>
        public static Size2D operator +(Size2D s1, Size2D s2)
        {
            Size2D result;

            result.Width = s1.Width + s2.Width;
            result.Height = s1.Height + s2.Height;

            return result;
        }

        /// <summary>
        /// Subtracts a <see cref="Size2D"/> from another <see cref="Size2D"/>.
        /// </summary>
        /// <param name="s1">The <see cref="Size2D"/> on the left side of the operator.</param>
        /// <param name="s2">The <see cref="Size2D"/> on the right side of the operator.</param>
        /// <returns>The result of subtracting the two instances.</returns>
        public static Size2D operator -(Size2D s1, Size2D s2)
        {
            Size2D result;

            result.Width = s1.Width - s2.Width;
            result.Height = s1.Height - s2.Height;

            return result;
        }

        /// <summary>
        /// Multiplies a <see cref="Size2D"/> by a scalar multiplier.
        /// </summary>
        /// <param name="size">The size to multiply.</param>
        /// <param name="multiplier">The multiplier to apply to the size.</param>
        /// <returns>A <see cref="Size2D"/> which is the result of the muliplication.</returns>
        public static Size2D operator *(Size2D size, Int32 multiplier)
        {
            Size2D result;

            result.Width = size.Width * multiplier;
            result.Height = size.Height * multiplier;

            return result;
        }

        /// <summary>
        /// Multiplies a <see cref="Size2D"/> by a scalar multiplier.
        /// </summary>
        /// <param name="size">The size to multiply.</param>
        /// <param name="multiplier">The multiplier to apply to the size.</param>
        /// <returns>A <see cref="Size2D"/> which is the result of the muliplication.</returns>
        public static Size2D operator *(Size2D size, Single multiplier)
        {
            Size2D result;

            result.Width = size.Width * multiplier;
            result.Height = size.Height * multiplier;

            return result;
        }

        /// <summary>
        /// Multiplies a <see cref="Size2D"/> by a scalar multiplier.
        /// </summary>
        /// <param name="size">The size to multiply.</param>
        /// <param name="multiplier">The multiplier to apply to the size.</param>
        /// <returns>A <see cref="Size2D"/> which is the result of the muliplication.</returns>
        public static Size2D operator *(Size2D size, Double multiplier)
        {
            Size2D result;

            result.Width = size.Width * multiplier;
            result.Height = size.Height * multiplier;

            return result;
        }

        /// <summary>
        /// Divides a <see cref="Size2D"/> by a scalar divisor.
        /// </summary>
        /// <param name="size">The size to divide.</param>
        /// <param name="divisor">The divisor to apply to the size.</param>
        /// <returns>A <see cref="Size2D"/> which is the result of the muliplication.</returns>
        public static Size2D operator /(Size2D size, Int32 divisor)
        {
            Size2D result;

            result.Width = size.Width / divisor;
            result.Height = size.Height / divisor;

            return result;
        }

        /// <summary>
        /// Divides a <see cref="Size2D"/> by a scalar divisor.
        /// </summary>
        /// <param name="size">The size to divide.</param>
        /// <param name="divisor">The divisor to apply to the size.</param>
        /// <returns>A <see cref="Size2D"/> which is the result of the muliplication.</returns>
        public static Size2D operator /(Size2D size, Single divisor)
        {
            Size2D result;

            result.Width = size.Width / divisor;
            result.Height = size.Height / divisor;

            return result;
        }

        /// <summary>
        /// Divides a <see cref="Size2D"/> by a scalar divisor.
        /// </summary>
        /// <param name="size">The size to divide.</param>
        /// <param name="divisor">The divisor to apply to the size.</param>
        /// <returns>A <see cref="Size2D"/> which is the result of the muliplication.</returns>
        public static Size2D operator /(Size2D size, Double divisor)
        {
            Size2D result;

            result.Width = size.Width / divisor;
            result.Height = size.Height / divisor;

            return result;
        }

        /// <summary>
        /// Explicitly converts a <see cref="Size2D"/> structure to a <see cref="Vector2"/> structure.
        /// </summary>
        /// <param name="size">The structure to convert.</param>
        /// <returns>The converted structure.</returns>
        public static explicit operator Vector2(Size2D size)
        {
            Vector2 result = new();

            result.X = (Single)size.Width;
            result.Y = (Single)size.Height;

            return result;
        }

        /// <summary>
        /// Explicitly converts a <see cref="Size2D"/> structure to a <see cref="Point"/> structure.
        /// </summary>
        /// <param name="size">The structure to convert.</param>
        /// <returns>The converted structure.</returns>
        public static explicit operator Point(Size2D size)
        {
            Point result = new();

            result.X = (Int32)size.Width;
            result.Y = (Int32)size.Height;

            return result;
        }

        /// <summary>
        /// Explicitly converts a <see cref="Size2D"/> structure to a <see cref="PointF"/> structure.
        /// </summary>
        /// <param name="size">The structure to convert.</param>
        /// <returns>The converted structure.</returns>
        public static explicit operator PointF(Size2D size)
        {
            PointF result = new();

            result.X = (Single)size.Width;
            result.Y = (Single)size.Height;

            return result;
        }

        /// <summary>
        /// Explicitly converts a <see cref="Size2D"/> structure to a <see cref="Point2D"/> structure.
        /// </summary>
        /// <param name="size">The structure to convert.</param>
        /// <returns>The converted structure.</returns>
        public static explicit operator Point2D(Size2D size)
        {
            Point2D result;

            result.X = size.Width;
            result.Y = size.Height;

            return result;
        }

        /// <summary>
        /// Explicitly converts a <see cref="Size2D"/> structure to a <see cref="Size"/> structure.
        /// </summary>
        /// <param name="size">The structure to convert.</param>
        /// <returns>The converted structure.</returns>
        public static explicit operator Size(Size2D size)
        {
            Size result = new();

            result.Width = (Int32)size.Width;
            result.Height = (Int32)size.Height;

            return result;
        }
        
        /// <summary>
        /// Explicitly converts a <see cref="Size2D"/> structure to a <see cref="SizeF"/> structure.
        /// </summary>
        /// <param name="size">The structure to convert.</param>
        /// <returns>The converted structure.</returns>
        public static explicit operator SizeF(Size2D size)
        {
            SizeF result = new();

            result.Width = (Single)size.Width;
            result.Height = (Single)size.Height;

            return result;
        }

        /// <inheritdoc/>
        public override String ToString() => $"{{Width:{Width} Height:{Height}}}";

        /// <summary>
        /// Interpolates between this value and the specified value.
        /// </summary>
        /// <param name="target">The target value.</param>
        /// <param name="t">A value between 0.0 and 1.0 representing the interpolation factor.</param>
        /// <returns>The interpolated value.</returns>
        public Size2D Interpolate(Size2D target, Single t)
        {
            Size2D result;

            result.Width = Tweening.Lerp(this.Width, target.Width, t);
            result.Height = Tweening.Lerp(this.Height, target.Height, t);

            return result;
        }

        /// <summary>
        /// Gets an area with zero width and height.
        /// </summary>
        public static Size2D Zero
        {
            get { return new Size2D(0, 0); }
        }

        /// <summary>
        /// Gets the size's total area (width times height).
        /// </summary>
        [JsonIgnore, IgnoreDataMember, System.Text.Json.Serialization.JsonIgnore]
        public Double Area
        {
            get { return Width * Height; }
        }

        /// <summary>
        /// The area's width.
        /// </summary>
        [JsonProperty(Required = Required.Always), DataMember]
        public Double Width;

        /// <summary>
        /// The area's height.
        /// </summary>
        [JsonProperty(Required = Required.Always), DataMember]
        public Double Height;
    }
}
