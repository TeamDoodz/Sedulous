﻿using System.IO;
using Sedulous.Content;

namespace Sedulous.Graphics.Graphics2D
{
    /// <summary>
    /// Contains extensoin methods for the <see cref="BinaryWriter"/> class.
    /// </summary>
    public static class Graphics2DBinaryWriterExtensions
    {
        /// <summary>
        /// Writes a sprite animation identifier to the stream.
        /// </summary>
        /// <param name="writer">The <see cref="BinaryWriter"/> with which to write the sprite animation identifier.</param>
        /// <param name="id">The <see cref="SpriteAnimationId"/> to write to the stream.</param>
        public static void Write(this BinaryWriter writer, SpriteAnimationId id)
        {
            writer.Write(id.IsValid);
            if (id.IsValid)
            {
                writer.Write(SpriteAnimationId.GetSpriteAssetIdRef(ref id));
                writer.Write(SpriteAnimationId.GetAnimationNameRef(ref id));
                writer.Write(SpriteAnimationId.GetAnimationIndexRef(ref id));
            }
        }

        /// <summary>
        /// Writes a sprite animation identifier to the stream.
        /// </summary>
        /// <param name="writer">The <see cref="BinaryReader"/> with which to write the sprite animation identifier.</param>
        /// <param name="id">The <see cref="System.Nullable{SpriteAnimationId}"/> to write to the stream.</param>
        public static void Write(this BinaryWriter writer, SpriteAnimationId? id)
        {
            writer.Write(id.HasValue);
            if (id.HasValue)
            {
                writer.Write(id.GetValueOrDefault());
            }
        }
    }
}
