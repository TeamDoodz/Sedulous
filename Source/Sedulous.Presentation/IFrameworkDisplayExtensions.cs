﻿using System;
using System.Numerics;
using Sedulous.Core;
using Sedulous.Platform;

namespace Sedulous.Presentation
{
    /// <summary>
    /// Contains extension methods for the <see cref="IFrameworkDisplay"/> interface.
    /// </summary>
    public static class IFrameworkDisplayExtensions
    {
        /// <summary>
        /// Converts a <see cref="Thickness"/> value with dimensions in inches to a <see cref="Thickness"/>
        /// value with dimensions in display pixels.
        /// </summary>
        /// <param name="this">The <see cref="IFrameworkDisplay"/> with which to perform the conversion.</param>
        /// <param name="inches">The <see cref="Thickness"/> in inches to convert.</param>
        /// <returns>The converted <see cref="Thickness"/> in display pixels.</returns>
        public static Thickness InchesToPixels(this IFrameworkDisplay @this, Thickness inches)
        {
            Contract.Require(@this, "this");

            var left   = @this.InchesToPixels(inches.Left);
            var top    = @this.InchesToPixels(inches.Top);
            var right  = @this.InchesToPixels(inches.Right);
            var bottom = @this.InchesToPixels(inches.Bottom);

            return new Thickness(left, top, right, bottom);
        }

        /// <summary>
        /// Converts a <see cref="Thickness"/> value with dimensions in display pixels to a <see cref="Thickness"/>
        /// value with dimensions in inches.
        /// </summary>
        /// <param name="this">The <see cref="IFrameworkDisplay"/> with which to perform the conversion.</param>
        /// <param name="pixels">The <see cref="Thickness"/> in display pixels to convert.</param>
        /// <returns>The converted <see cref="Thickness"/> in inches.</returns>
        public static Thickness PixelsToInches(this IFrameworkDisplay @this, Thickness pixels)
        {
            Contract.Require(@this, "this");

            var left   = @this.PixelsToInches(pixels.Left);
            var top    = @this.PixelsToInches(pixels.Top);
            var right  = @this.PixelsToInches(pixels.Right);
            var bottom = @this.PixelsToInches(pixels.Bottom);

            return new Thickness(left, top, right, bottom);
        }

        /// <summary>
        /// Converts a <see cref="Thickness"/> value with dimensions in display independent pixels to a <see cref="Thickness"/>
        /// value with dimensions in display pixels.
        /// </summary>
        /// <param name="this">The <see cref="IFrameworkDisplay"/> with which to perform the conversion.</param>
        /// <param name="dips">The <see cref="Thickness"/> in display independent pixels to convert.</param>
        /// <returns>The converted <see cref="Thickness"/> in display pixels.</returns>
        public static Thickness DipsToPixels(this IFrameworkDisplay @this, Thickness dips)
        {
            Contract.Require(@this, "this");

            var left   = @this.DipsToPixels(dips.Left);
            var top    = @this.DipsToPixels(dips.Top);
            var right  = @this.DipsToPixels(dips.Right);
            var bottom = @this.DipsToPixels(dips.Bottom);

            return new Thickness(left, top, right, bottom);
        }

        /// <summary>
        /// Converts a <see cref="Thickness"/> value with dimensions in display pixels to a <see cref="Thickness"/>
        /// value with dimensions in display independent pixels.
        /// </summary>
        /// <param name="this">The <see cref="IFrameworkDisplay"/> with which to perform the conversion.</param>
        /// <param name="pixels">The <see cref="Thickness"/> in display pixels to convert.</param>
        /// <returns>The converted <see cref="Thickness"/> in display independent pixels.</returns>
        public static Thickness PixelsToDips(this IFrameworkDisplay @this, Thickness pixels)
        {
            Contract.Require(@this, "this");

            var left   = @this.PixelsToDips(pixels.Left);
            var top    = @this.PixelsToDips(pixels.Top);
            var right  = @this.PixelsToDips(pixels.Right);
            var bottom = @this.PixelsToDips(pixels.Bottom);

            return new Thickness(left, top, right, bottom);
        }

        /// <summary>
        /// Converts a <see cref="Thickness"/> value with dimensions in inches to a <see cref="Thickness"/>
        /// value with dimensions in display independent pixels.
        /// </summary>
        /// <param name="this">The <see cref="IFrameworkDisplay"/> with which to perform the conversion.</param>
        /// <param name="inches">The <see cref="Thickness"/> in inches to convert.</param>
        /// <returns>The converted <see cref="Thickness"/> in display independent pixels.</returns>
        public static Thickness InchesToDips(this IFrameworkDisplay @this, Thickness inches)
        {
            Contract.Require(@this, "this");

            var left   = @this.InchesToDips(inches.Left);
            var top    = @this.InchesToDips(inches.Top);
            var right  = @this.InchesToDips(inches.Right);
            var bottom = @this.InchesToDips(inches.Bottom);

            return new Thickness(left, top, right, bottom);
        }

        /// <summary>
        /// Converts a <see cref="Thickness"/> value with dimensions in display independent pixels to a <see cref="Thickness"/>
        /// value with dimensions in inches.
        /// </summary>
        /// <param name="this">The <see cref="IFrameworkDisplay"/> with which to perform the conversion.</param>
        /// <param name="dips">The <see cref="Thickness"/> in display independent pixels to convert.</param>
        /// <returns>The converted <see cref="Thickness"/> in inches.</returns>
        public static Thickness DipsToInches(this IFrameworkDisplay @this, Thickness dips)
        {
            Contract.Require(@this, "this");

            var left   = @this.DipsToInches(dips.Left);
            var top    = @this.DipsToInches(dips.Top);
            var right  = @this.DipsToInches(dips.Right);
            var bottom = @this.DipsToInches(dips.Bottom);

            return new Thickness(left, top, right, bottom);
        }

        /// <summary>
        /// Converts a <see cref="Matrix4x4"/> representing a 2D transformation in display independent pixels to an
        /// equivalent <see cref="Matrix4x4"/> in display pixels.
        /// </summary>
        /// <param name="this">The <see cref="IFrameworkDisplay"/> with which to perform the conversion.</param>
        /// <param name="matrix">The <see cref="Matrix4x4"/> in display independent pixels to convert.</param>
        /// <returns>The converted <see cref="Matrix4x4"/> in display pixels.</returns>
        public static Matrix4x4 DipsToPixels(this IFrameworkDisplay @this, Matrix4x4 matrix)
        {
            var x = (Single)@this.DipsToPixels(matrix.M14);
            var y = (Single)@this.DipsToPixels(matrix.M24);
            var z = (Single)@this.DipsToPixels(matrix.M34);

            return new Matrix4x4(
                matrix.M11, matrix.M12, matrix.M13, matrix.M14,
                matrix.M21, matrix.M22, matrix.M23, matrix.M24,
                matrix.M31, matrix.M32, matrix.M33, matrix.M34,
                         x,          y,          z, matrix.M44);
        }

        /// <summary>
        /// Converts a <see cref="Matrix4x4"/> representing a 2D transformation in display independent pixels to an
        /// equivalent <see cref="Matrix4x4"/> in display pixels.
        /// </summary>
        /// <param name="this">The <see cref="IFrameworkDisplay"/> with which to perform the conversion.</param>
        /// <param name="matrix">The <see cref="Matrix4x4"/> in display independent pixels to convert.</param>
        /// <param name="result">The converted <see cref="Matrix4x4"/> in display pixels.</param>
        public static void DipsToPixels(this IFrameworkDisplay @this, ref Matrix4x4 matrix, out Matrix4x4 result)
        {
            var x = (Single)@this.DipsToPixels(matrix.M14);
            var y = (Single)@this.DipsToPixels(matrix.M24);
            var z = (Single)@this.DipsToPixels(matrix.M34);

            result = new Matrix4x4(
                matrix.M11, matrix.M12, matrix.M13, matrix.M14,
                matrix.M21, matrix.M22, matrix.M23, matrix.M24,
                matrix.M31, matrix.M32, matrix.M33, matrix.M34,
                         x,          y,          z, matrix.M44);
        }
    }
}
