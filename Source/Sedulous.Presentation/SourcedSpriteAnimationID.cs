﻿using System;
using Newtonsoft.Json;
using Sedulous.Core.Data;
using Sedulous.Graphics.Graphics2D;

namespace Sedulous.Presentation
{
    /// <summary>
    /// Represents an animation identifier which is flagged as being either globally- or locally-sourced.
    /// </summary>
    [JsonConverter(typeof(ObjectResolverJsonConverter))]
    public partial struct SourcedSpriteAnimationID : IEquatable<SourcedSpriteAnimationID>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SourcedSpriteAnimationID"/> class.
        /// </summary>
        /// <param name="spriteAnimationID">The sprite animation's identifier.</param>
        /// <param name="spriteSource">The sprite asset's source.</param>
        public SourcedSpriteAnimationID(SpriteAnimationId spriteAnimationID, AssetSource spriteSource)
        {
            this.spriteAnimationID = spriteAnimationID;
            this.spriteSource = spriteSource;
        }

        /// <inheritdoc/>
        public override String ToString() => $"{SpriteAnimationID} {SpriteSource.ToString().ToLowerInvariant()}";

        /// <summary>
        /// Gets the sprite animation's identifier.
        /// </summary>
        public SpriteAnimationId SpriteAnimationID
        {
            get { return spriteAnimationID; }
        }

        /// <summary>
        /// Gets the sprite's source.
        /// </summary>
        public AssetSource SpriteSource
        {
            get { return spriteSource; }
        }
        
        // Property values.
        private readonly SpriteAnimationId spriteAnimationID;
        private readonly AssetSource spriteSource;
    }
}
