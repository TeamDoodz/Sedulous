using System;
using System.Globalization;
using Sedulous.Content;
using Sedulous.Core;
using Sedulous.Graphics.Graphics2D;

namespace Sedulous.Presentation
{
    partial struct SourcedSpriteAnimationID
    {
        /// <summary>
        /// Converts the string representation of a <see cref="SourcedSpriteAnimationID"/> to an object instance.
        /// </summary>
        /// <param name="manifests">The content manifest registry that contains the currently-loaded content manifests.</param>
        /// <param name="s">The string to convert.</param>
        /// <returns>The converted value.</returns>
        public static SourcedSpriteAnimationID Parse(ContentManifestRegistry manifests, String s)
        {
            Contract.Require(manifests, nameof(manifests));
            Contract.Require(s, nameof(s));

            SourcedSpriteAnimationID value;
            if (!TryParseInternal(manifests, s, out value))
            {
                throw new FormatException();
            }
            return value;
        }

        /// <summary>
        /// Converts the string representation of a <see cref="SourcedSpriteAnimationID"/> to an object instance.
        /// A return value indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="manifests">The content manifest registry that contains the currently-loaded content manifests.</param>
        /// <param name="s">The string to convert.</param>
        /// <param name="v">The converted value.</param>
        /// <returns><see langword="true"/> if the conversion succeeded; otherwise, <see langword="false"/>.</returns>
        public static Boolean TryParse(ContentManifestRegistry manifests, String s, out SourcedSpriteAnimationID v)
        {
            Contract.Require(manifests, nameof(manifests));
            Contract.Require(s, nameof(s));

            return TryParseInternal(manifests, s, out v);
        }

        /// <summary>
        /// Converts the string representation of a <see cref="SourcedSpriteAnimationID"/> to an object instance.
        /// A return value indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="s">The string to convert.</param>
        /// <param name="v">The converted value.</param>
        /// <returns><see langword="true"/> if the conversion succeeded; otherwise, <see langword="false"/>.</returns>
        public static Boolean TryParse(String s, out SourcedSpriteAnimationID v)
        {
            return TryParse(s, NumberStyles.Number, NumberFormatInfo.CurrentInfo, out v);
        }

        /// <summary>
        /// Converts the string representation of a <see cref="SourcedSpriteAnimationID"/> to an object instance.
        /// </summary>
        /// <param name="s">The string to convert.</param>
        /// <returns>The converted value.</returns>
        public static SourcedSpriteAnimationID Parse(String s)
        {
            var v = default(SourcedSpriteAnimationID);
            if (!TryParse(s, NumberStyles.Number, NumberFormatInfo.CurrentInfo, out v))
                throw new FormatException();

            return v;
        }

        /// <summary>
        /// Converts the string representation of a <see cref="SourcedSpriteAnimationID"/> to an object instance.
        /// A return value indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="s">The string to convert.</param>
        /// <param name="style">A set of <see cref="NumberStyles"/> values indicating which elements are present in <paramref name="s"/>.</param>
        /// <param name="provider">A format provider that provides culture-specific formatting information.</param>
        /// <param name="v">The converted value.</param>
        /// <returns><see langword="true"/> if the conversion succeeded; otherwise, <see langword="false"/>.</returns>
        public static Boolean TryParse(String s, NumberStyles style, IFormatProvider provider, out SourcedSpriteAnimationID v)
        {
            Contract.Require(s, nameof(s));

            return TryParseInternal(null, s, out v);
        }

        /// <summary>
        /// Converts the string representation of a <see cref="SourcedSpriteAnimationID"/> to an object instance.
        /// </summary>
        /// <param name="s">The string to convert.</param>
        /// <param name="style">A set of <see cref="NumberStyles"/> values indicating which elements are present in <paramref name="s"/>.</param>
        /// <param name="provider">A format provider that provides culture-specific formatting information.</param>
        /// <returns>The converted value.</returns>
        public static SourcedSpriteAnimationID Parse(String s, NumberStyles style, IFormatProvider provider)
        {
            var v = default(SourcedSpriteAnimationID);
            if (!TryParse(s, style, provider, out v))
                throw new FormatException();

            return v;
        }

        /// <summary>
        /// Converts the string representation of a sprite animation identifier to an instance of 
        /// the <see cref="SourcedSpriteAnimationID"/> structure.
        /// </summary>
        /// <param name="manifests">The content manifest registry that contains the currently-loaded content manifests.</param>
        /// <param name="s">A string containing the sprite animation identifier to convert.</param>
        /// <param name="value">An instance of the <see cref="SourcedSpriteAnimationID"/> structure that is equivalent to the specified string.</param>
        /// <returns><see langword="true"/> if the string was successfully parsed; otherwise, <see langword="false"/>.</returns>
        private static Boolean TryParseInternal(ContentManifestRegistry manifests, String s, out SourcedSpriteAnimationID value)
        {
            var parts = s.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 2)
                throw new FormatException();

            // Parse the asset identifier
            var spriteAnimationID = default(SpriteAnimationId);
            var assetIDParsed = false;

            if (manifests == null)
            {
                assetIDParsed = Graphics.Graphics2D.SpriteAnimationId.TryParse(parts[0], out spriteAnimationID);
            }
            else
            {
                assetIDParsed = Graphics.Graphics2D.SpriteAnimationId.TryParse(manifests, parts[0], out spriteAnimationID);
            }

            if (!assetIDParsed)
            {
                value = default(SourcedSpriteAnimationID);
                return false;
            }

            // Parse the asset source
            AssetSource spriteSource = AssetSource.Global;
            if (parts.Length == 2)
            {
                if (!Enum.TryParse(parts[1], true, out spriteSource))
                {
                    value = default(SourcedSpriteAnimationID);
                    return false;
                }
            }

            value = new SourcedSpriteAnimationID(spriteAnimationID, spriteSource);
            return true;
        }
    }
}