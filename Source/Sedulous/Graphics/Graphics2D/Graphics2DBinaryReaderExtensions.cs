using System;
using System.IO;
using Sedulous.Content;
using Sedulous.Core;

namespace Sedulous.Graphics.Graphics2D
{
    /// <summary>
    /// Contains extension methods for the <see cref="BinaryReader"/> class.
    /// </summary>
    public static class Graphics2DBinaryReaderExtensions
    {
        /// <summary>
        /// Reads a sprite animation identifier from the stream using the content manifest registry
        /// belonging to the current Framework context.
        /// </summary>
        /// <param name="reader">The <see cref="BinaryReader"/> which to read the sprite animation identifier.</param>
        /// <returns>The <see cref="SpriteAnimationId"/> that was read from the stream.</returns>
        public static SpriteAnimationId ReadSpriteAnimationId(this BinaryReader reader)
        {
            Contract.Require(reader, nameof(reader));

            return ReadSpriteAnimationId(reader, FrameworkContext.DemandCurrent().GetContent().Manifests);
        }

        /// <summary>
        /// Reads a nullable asset identifier from the stream using the content manifest registry
        /// belonging to the current Framework context.
        /// </summary>
        /// <param name="reader">The <see cref="BinaryReader"/> from which to read the sprite animation identifier.</param>
        /// <returns>The <see cref="Nullable{SpriteAnimationID}"/> identifier that was read from the stream.</returns>
        public static SpriteAnimationId? ReadNullableSpriteAnimationId(this BinaryReader reader)
        {
            Contract.Require(reader, nameof(reader));

            return ReadNullableSpriteAnimationId(reader, FrameworkContext.DemandCurrent().GetContent().Manifests);
        }

        /// <summary>
        /// Reads a sprite animation identifier from the stream.
        /// </summary>
        /// <param name="reader">The <see cref="BinaryReader"/> from which to read the sprite animation identifier.</param>
        /// <param name="manifests">The <see cref="ContentManifestRegistry"/> that contains the application's loaded manifests.</param>
        /// <returns>The <see cref="SpriteAnimationId"/> that was read from the stream.</returns>
        public static SpriteAnimationId ReadSpriteAnimationId(this BinaryReader reader, ContentManifestRegistry manifests)
        {
            Contract.Require(reader, nameof(reader));
            Contract.Require(manifests, nameof(manifests));

            var valid = reader.ReadBoolean();
            if (valid)
            {
                var spriteAssetID = reader.ReadAssetId();
                var animationName = reader.ReadString();
                var animationIndex = reader.ReadInt32();

                return String.IsNullOrEmpty(animationName) ? 
                    new SpriteAnimationId(spriteAssetID, animationIndex) :
                    new SpriteAnimationId(spriteAssetID, animationName);
            }
            return SpriteAnimationId.Invalid;
        }

        /// <summary>
        /// Reads a nullable asset identifier from the stream.
        /// </summary>
        /// <param name="reader">The <see cref="BinaryReader"/> from which to read the sprite animation identifier.</param>
        /// <param name="manifests">The <see cref="ContentManifestRegistry"/> that contains the application's loaded manifests.</param>
        /// <returns>The <see cref="SpriteAnimationId"/> that was read from the stream.</returns>
        public static SpriteAnimationId? ReadNullableSpriteAnimationId(this BinaryReader reader, ContentManifestRegistry manifests)
        {
            Contract.Require(reader, nameof(reader));
            Contract.Require(manifests, nameof(manifests));

            var hasValue = reader.ReadBoolean();
            if (hasValue)
            {
                return reader.ReadSpriteAnimationId(manifests);
            }
            return null;
        }
    }
}
