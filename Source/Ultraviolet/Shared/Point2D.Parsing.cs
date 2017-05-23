using System;
using System.Globalization;
using Ultraviolet.Core;

namespace Ultraviolet
{
    partial struct Point2D
    {
        /// <inheritdoc/>
        public override String ToString()
        {
            return ToString(null);
        }
        
        /// <summary>
        /// Converts the object to a human-readable string using the specified culture information.
        /// </summary>
        /// <param name="provider">A format provider that provides culture-specific formatting information.</param>
        /// <returns>A human-readable string that represents the object.</returns>
        public String ToString(IFormatProvider provider)
        {
            return String.Format(provider, "{0} {1}", X, Y);
        }
        
        /// <summary>
        /// Converts the string representation of a <see cref="Point2D"/> to an object instance.
        /// A return value indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="s">The string to convert.</param>
        /// <param name="v">The converted value.</param>
        /// <returns><see langword="true"/> if the conversion succeeded; otherwise, <see langword="false"/>.</returns>
        [Preserve]
        public static Boolean TryParse(String s, out Point2D v)
        {
            return TryParse(s, NumberStyles.Number, NumberFormatInfo.CurrentInfo, out v);
        }
        
        /// <summary>
        /// Converts the string representation of a <see cref="Point2D"/> to an object instance.
        /// </summary>
        /// <param name="s">The string to convert.</param>
        /// <returns>The converted value.</returns>
        [Preserve]
        public static Point2D Parse(String s)
        {
            var v = default(Point2D);
            if (!TryParse(s, NumberStyles.Number, NumberFormatInfo.CurrentInfo, out v))
                throw new FormatException();
            
            return v;
        }
        
        /// <summary>
        /// Converts the string representation of a <see cref="Point2D"/> to an object instance.
        /// A return value indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="s">The string to convert.</param>
        /// <param name="style">A set of <see cref="NumberStyles"/> values indicating which elements are present in <paramref name="s"/>.</param>
        /// <param name="provider">A format provider that provides culture-specific formatting information.</param>
        /// <param name="v">The converted value.</param>
        /// <returns><see langword="true"/> if the conversion succeeded; otherwise, <see langword="false"/>.</returns>
        [Preserve]
        public static Boolean TryParse(String s, NumberStyles style, IFormatProvider provider, out Point2D v)
        {
            v = default(Point2D);

            if (String.IsNullOrEmpty(s))
                return false;

            var components = s.Split((Char[])null, StringSplitOptions.RemoveEmptyEntries);
            if (components.Length != 2)
                return false;

            Double x, y;
            if (!Double.TryParse(components[0], style, provider, out x))
                return false;
            if (!Double.TryParse(components[1], style, provider, out y))
                return false;

            v = new Point2D(x, y);
            return true;
        }
        
        /// <summary>
        /// Converts the string representation of a <see cref="Point2D"/> to an object instance.
        /// </summary>
        /// <param name="s">The string to convert.</param>
        /// <param name="style">A set of <see cref="NumberStyles"/> values indicating which elements are present in <paramref name="s"/>.</param>
        /// <param name="provider">A format provider that provides culture-specific formatting information.</param>
        /// <returns>The converted value.</returns>
        [Preserve]
        public static Point2D Parse(String s, NumberStyles style, IFormatProvider provider)
        {
            var v = default(Point2D);
            if (!TryParse(s, style, provider, out v))
                throw new FormatException();
            
            return v;
        }
    }
}
