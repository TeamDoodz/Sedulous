using System;
using System.Globalization;
using Sedulous.Content;
using Sedulous.Core;

namespace Sedulous.Graphics.Graphics2D
{
    partial struct SpriteAnimationId
    {
        /// <summary>
        /// Converts the string representation of a sprite animation identifier to an instance of the SpriteAnimationID structure.
        /// </summary>
        /// <param name="manifests">The content manifest registry that contains the currently-loaded content manifests.</param>
        /// <param name="s">A string containing the sprite animation identifier to convert.</param>
        /// <returns>An instance of the SpriteAnimationID structure that is equivalent to the specified string.</returns>
        public static SpriteAnimationId Parse(ContentManifestRegistry manifests, String s)
        {
            Contract.Require(manifests, nameof(manifests));
            Contract.Require(s, nameof(s));
            
            if (!TryParseInternal(manifests, s, out SpriteAnimationId value))
            {
                throw new FormatException();
            }
            return value;
        }

        /// <summary>
        /// Converts the string representation of a sprite animation identifier to an instance of the SpriteAnimationID structure.
        /// </summary>
        /// <param name="manifests">The content manifest registry that contains the currently-loaded content manifests.</param>
        /// <param name="s">A string containing the sprite animation identifier to convert.</param>
        /// <param name="value">An instance of the SpriteAnimationID structure that is equivalent to the specified string.</param>
        /// <returns><see langword="true"/> if the string was successfully parsed; otherwise, <see langword="false"/>.</returns>
        public static Boolean TryParse(ContentManifestRegistry manifests, String s, out SpriteAnimationId value)
        {
            Contract.Require(manifests, nameof(manifests));
            Contract.Require(s, nameof(s));

            return TryParseInternal(manifests, s, out value);
        }

        /// <summary>
        /// Converts the string representation of a <see cref="SpriteAnimationId"/> to an object instance.
        /// A return value indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="s">The string to convert.</param>
        /// <param name="v">The converted value.</param>
        /// <returns><see langword="true"/> if the conversion succeeded; otherwise, <see langword="false"/>.</returns>
        public static Boolean TryParse(String s, out SpriteAnimationId v)
        {
            return TryParse(s, NumberStyles.Integer, NumberFormatInfo.CurrentInfo, out v);
        }
        
        /// <summary>
        /// Converts the string representation of a <see cref="SpriteAnimationId"/> to an object instance.
        /// </summary>
        /// <param name="s">The string to convert.</param>
        /// <returns>The converted value.</returns>
        public static SpriteAnimationId Parse(String s)
        {
            var v = default(SpriteAnimationId);
            if (!TryParse(s, NumberStyles.Integer, NumberFormatInfo.CurrentInfo, out v))
                throw new FormatException();
            
            return v;
        }
        
        /// <summary>
        /// Converts the string representation of a <see cref="SpriteAnimationId"/> to an object instance.
        /// A return value indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="s">The string to convert.</param>
        /// <param name="style">A set of <see cref="NumberStyles"/> values indicating which elements are present in <paramref name="s"/>.</param>
        /// <param name="provider">A format provider that provides culture-specific formatting information.</param>
        /// <param name="v">The converted value.</param>
        /// <returns><see langword="true"/> if the conversion succeeded; otherwise, <see langword="false"/>.</returns>
        public static Boolean TryParse(String s, NumberStyles style, IFormatProvider provider, out SpriteAnimationId v)
        {
            Contract.Require(s, nameof(s));

            var uv = FrameworkContext.DemandCurrent();
            return TryParseInternal(uv.GetContent().Manifests, s, out v);
        }
        
        /// <summary>
        /// Converts the string representation of a <see cref="SpriteAnimationId"/> to an object instance.
        /// </summary>
        /// <param name="s">The string to convert.</param>
        /// <param name="style">A set of <see cref="NumberStyles"/> values indicating which elements are present in <paramref name="s"/>.</param>
        /// <param name="provider">A format provider that provides culture-specific formatting information.</param>
        /// <returns>The converted value.</returns>
        public static SpriteAnimationId Parse(String s, NumberStyles style, IFormatProvider provider)
        {
            var v = default(SpriteAnimationId);
            if (!TryParse(s, style, provider, out v))
                throw new FormatException();
            
            return v;
        }
        
        /// <summary>
        /// Converts the string representation of an asset identifier to an instance of the SpriteAnimationID structure.
        /// </summary>
        /// <param name="manifests">The content manifest registry that contains the currently-loaded content manifests.</param>
        /// <param name="s">A string containing the asset identifier to convert.</param>
        /// <param name="value">An instance of the SpriteAnimationID structure that is equivalent to the specified string.</param>
        /// <returns><see langword="true"/> if the string was successfully parsed; otherwise, <see langword="false"/>.</returns>
        private static Boolean TryParseInternal(ContentManifestRegistry manifests, String s, out SpriteAnimationId value)
        {
            value = default(SpriteAnimationId);

            var delimiterIndex = s.LastIndexOf(':');
            if (delimiterIndex < 0)
                return false;

            var assetID = AssetId.Invalid;
            if (!AssetId.TryParse(manifests, s.Substring(0, delimiterIndex), out assetID))
                return false;

            var animation = s.Substring(delimiterIndex + 1);
            if (String.IsNullOrEmpty(animation))
                return false;

            var animationIndex = 0;
            var animationIndexIsValid = Int32.TryParse(animation, out animationIndex);

            value = animationIndexIsValid ?
                new SpriteAnimationId(assetID, animationIndex) :
                new SpriteAnimationId(assetID, animation);

            return true;
        }
    }
}
