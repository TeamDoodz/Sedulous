﻿using System;
using System.Drawing;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Sedulous
{
    /// <summary>
    /// Represents a circle with integer radius and position.
    /// </summary>
    [Serializable, DataContract]
    public partial struct Circle : IEquatable<Circle>, IInterpolatable<Circle>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Circle"/> structure.
        /// </summary>
        /// <param name="x">The x-coordinate of the circle's center.</param>
        /// <param name="y">The y-coordinate of the circle's center.</param>
        /// <param name="radius">The circle's radius.</param>
        [JsonConstructor]
        public Circle(Int32 x, Int32 y, Int32 radius)
        {
            this.X = x;
            this.Y = y;
            this.Radius = radius;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Circle"/> structure.
        /// </summary>
        /// <param name="position">The position of the circle's center.</param>
        /// <param name="radius">The circle's radius.</param>
        public Circle(Point position, Int32 radius)
            : this(position.X, position.Y, radius)
        {

        }
        
        /// <summary>
        /// Offsets the specified <see cref="Circle"/> by adding the specified <see cref="Point"/> to its location.
        /// </summary>
        /// <param name="circle">The <see cref="Circle"/> to offset.</param>
        /// <param name="point">The <see cref="Point"/> by which to offset the Circle.</param>
        /// <returns>A <see cref="Circle"/> that has been offset by the specified amount.</returns>
        public static Circle operator +(Circle circle, Point point)
        {
            Circle result;

            result.X = circle.X + point.X;
            result.Y = circle.Y + point.Y;
            result.Radius = circle.Radius;

            return result;
        }

        /// <summary>
        /// Offsets the specified <see cref="Circle"/> by subtracting the specified <see cref="Point"/> from its location.
        /// </summary>
        /// <param name="circle">The <see cref="Circle"/> to offset.</param>
        /// <param name="point">The <see cref="Point"/> by which to offset the Circle.</param>
        /// <returns>A <see cref="Circle"/> that has been offset by the specified amount.</returns>
        public static Circle operator -(Circle circle, Point point)
        {
            Circle result;

            result.X = circle.X - point.X;
            result.Y = circle.Y - point.Y;
            result.Radius = circle.Radius;

            return result;
        }

        /// <summary>
        /// Offsets the specified <see cref="Circle"/> by adding the specified <see cref="PointF"/> to its location.
        /// </summary>
        /// <param name="circle">The <see cref="Circle"/> to offset.</param>
        /// <param name="point">The <see cref="PointF"/> by which to offset the Circle.</param>
        /// <returns>A <see cref="CircleF"/> that has been offset by the specified amount.</returns>
        public static CircleF operator +(Circle circle, PointF point)
        {
            CircleF result;

            result.X = circle.X + point.X;
            result.Y = circle.Y + point.Y;
            result.Radius = circle.Radius;

            return result;
        }

        /// <summary>
        /// Offsets the specified <see cref="Circle"/> by subtracting the specified <see cref="PointF"/> from its location.
        /// </summary>
        /// <param name="circle">The <see cref="Circle"/> to offset.</param>
        /// <param name="point">The <see cref="PointF"/> by which to offset the Circle.</param>
        /// <returns>A <see cref="CircleF"/> that has been offset by the specified amount.</returns>
        public static CircleF operator -(Circle circle, PointF point)
        {
            CircleF result;

            result.X = circle.X - point.X;
            result.Y = circle.Y - point.Y;
            result.Radius = circle.Radius;

            return result;
        }

        /// <summary>
        /// Offsets the specified <see cref="Circle"/> by adding the specified <see cref="Point2D"/> to its location.
        /// </summary>
        /// <param name="circle">The <see cref="Circle"/> to offset.</param>
        /// <param name="point">The <see cref="Point2D"/> by which to offset the Circle.</param>
        /// <returns>A <see cref="CircleD"/> that has been offset by the specified amount.</returns>
        public static CircleD operator +(Circle circle, Point2D point)
        {
            CircleD result;

            result.X = circle.X + point.X;
            result.Y = circle.Y + point.Y;
            result.Radius = circle.Radius;

            return result;
        }

        /// <summary>
        /// Offsets the specified <see cref="Circle"/> by subtracting the specified <see cref="Point2D"/> from its location.
        /// </summary>
        /// <param name="circle">The <see cref="Circle"/> to offset.</param>
        /// <param name="point">The <see cref="Point2D"/> by which to offset the Circle.</param>
        /// <returns>A <see cref="CircleD"/> that has been offset by the specified amount.</returns>
        public static CircleD operator -(Circle circle, Point2D point)
        {
            CircleD result;

            result.X = circle.X - point.X;
            result.Y = circle.Y - point.Y;
            result.Radius = circle.Radius;

            return result;
        }

        /// <summary>
        /// Implicitly converts a <see cref="Circle"/> structure to a <see cref="CircleF"/> structure.
        /// </summary>
        /// <param name="circle">The structure to convert.</param>
        /// <returns>The converted structure.</returns>
        public static implicit operator CircleF(Circle circle)
        {
            CircleF result;

            result.X = circle.X;
            result.Y = circle.Y;
            result.Radius = circle.Radius;

            return result;
        }

        /// <summary>
        /// Implicitly converts a <see cref="Circle"/> structure to a <see cref="CircleD"/> structure.
        /// </summary>
        /// <param name="circle">The structure to convert.</param>
        /// <returns>The converted structure.</returns>
        public static implicit operator CircleD(Circle circle)
        {
            CircleD result;

            result.X = circle.X;
            result.Y = circle.Y;
            result.Radius = circle.Radius;

            return result;
        }

        /// <inheritdoc/>
        public override String ToString() => $"{{Position:{Position} Radius:{Radius}}}";

        /// <summary>
        /// Interpolates between this value and the specified value.
        /// </summary>
        /// <param name="target">The target value.</param>
        /// <param name="t">A value between 0.0 and 1.0 representing the interpolation factor.</param>
        /// <returns>The interpolated value.</returns>
        public Circle Interpolate(Circle target, Single t)
        {
            Circle result;

            result.X = Tweening.Lerp(this.X, target.X, t);
            result.Y = Tweening.Lerp(this.Y, target.Y, t);
            result.Radius = Tweening.Lerp(this.Radius, target.Radius, t);

            return result;
        }

        /// <summary>
        /// Gets an instance representing the unit circle.
        /// </summary>
        public static Circle Unit
        {
            get { return new Circle(0, 0, 1); }
        }

        /// <summary>
        /// The circle's position.
        /// </summary>
        [JsonIgnore, IgnoreDataMember, System.Text.Json.Serialization.JsonIgnore]
        public Point Position
        {
            get { return new Point(X, Y); }
        }

        /// <summary>
        /// The x-coordinate of the circle's center.
        /// </summary>
        [JsonProperty(Required = Required.Always), DataMember]
        public Int32 X;

        /// <summary>
        /// The y-coordinate of the circle's center.
        /// </summary>
        [JsonProperty(Required = Required.Always), DataMember]
        public Int32 Y;

        /// <summary>
        /// Gets the circle's radius.
        /// </summary>
        [JsonProperty(Required = Required.Always), DataMember]
        public Int32 Radius;
    }
}
