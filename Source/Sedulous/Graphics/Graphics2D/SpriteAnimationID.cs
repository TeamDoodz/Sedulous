using System;
using Newtonsoft.Json;
using Sedulous.Content;

namespace Sedulous.Graphics.Graphics2D
{
    /// <summary>
    /// Represents a value which identifies a particular sprite animation.
    /// </summary>
    [JsonConverter(typeof(FrameworkJsonConverter))]
    public partial struct SpriteAnimationId : IEquatable<SpriteAnimationId>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpriteAnimationId"/> structure from the specified animation name.
        /// </summary>
        /// <param name="spriteAssetID">The <see cref="AssetId"/> that represents the sprite that contains the animation.</param>
        /// <param name="animationName">The name of the referenced animation.</param>
        internal SpriteAnimationId(AssetId spriteAssetID, String animationName)
        {
            this.spriteAssetID = spriteAssetID;
            this.animationName = animationName;
            this.animationIndex = -1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpriteAnimationId"/> structure from the specified animation name.
        /// </summary>
        /// <param name="spriteAssetID">The <see cref="AssetId"/> that represents the sprite that contains the animation.</param>
        /// <param name="animationIndex">The index of the referenced animation.</param>
        internal SpriteAnimationId(AssetId spriteAssetID, Int32 animationIndex)
        {
            this.spriteAssetID = spriteAssetID;
            this.animationName = null;
            this.animationIndex = animationIndex;
        }
        
        /// <summary>
        /// Gets the asset identifier of the sprite that contains the specified animation.
        /// </summary>
        /// <param name="id">The identifier of the animation to evaluate.</param>
        /// <returns>The asset identifier of the sprite that contains the specified animation.</returns>
        public static AssetId GetSpriteAssetId(SpriteAnimationId id)
        {
            return id.spriteAssetID;
        }

        /// <summary>
        /// Gets the asset identifier of the sprite that contains the specified animation.
        /// </summary>
        /// <param name="id">The identifier of the animation to evaluate.</param>
        /// <returns>The asset identifier of the sprite that contains the specified animation.</returns>
        public static AssetId GetSpriteAssetIdRef(ref SpriteAnimationId id)
        {
            return id.spriteAssetID;
        }

        /// <summary>
        /// Gets the name of the specified animation.
        /// </summary>
        /// <param name="id">The identifier of the animation to evaluate.</param>
        /// <returns>The name of the specified animation.</returns>
        public static String GetAnimationName(SpriteAnimationId id)
        {
            return id.animationName;
        }
        
        /// <summary>
        /// Gets the name of the specified animation.
        /// </summary>
        /// <param name="id">The identifier of the animation to evaluate.</param>
        /// <returns>The name of the specified animation.</returns>
        public static String GetAnimationNameRef(ref SpriteAnimationId id)
        {
            return id.animationName;
        }

        /// <summary>
        /// Gets the name of the specified animation within its sprite's animation list.
        /// </summary>
        /// <param name="id">The identifier of the animation to evaluate.</param>
        /// <returns>The name of the specified animation within its sprite's animation list.</returns>
        public static Int32 GetAnimationIndex(SpriteAnimationId id)
        {
            return id.animationIndex;
        }

        /// <summary>
        /// Gets the name of the specified animation within its sprite's animation list.
        /// </summary>
        /// <param name="id">The identifier of the animation to evaluate.</param>
        /// <returns>The name of the specified animation within its sprite's animation list.</returns>
        public static Int32 GetAnimationIndexRef(ref SpriteAnimationId id)
        {
            return id.animationIndex;
        }
        
        /// <summary>
        /// Gets an invalid animation identifier.
        /// </summary>
        public static SpriteAnimationId Invalid
        {
            get { return new SpriteAnimationId(); }
        }

        /// <inheritdoc/>
        public override String ToString() => String.Format("{0}:{1}",
            spriteAssetID, String.IsNullOrEmpty(animationName) ? animationIndex.ToString() : animationName);

        /// <summary>
        /// Gets the asset identifier of the sprite that contains the animation.
        /// </summary>
        public AssetId SpriteAssetId => spriteAssetID;

        /// <summary>
        /// Gets a value indicating whether this is a valid sprite animation identifier.
        /// </summary>
        public Boolean IsValid
        {
            get { return spriteAssetID.IsValid && (!String.IsNullOrEmpty(animationName) || animationIndex <= 0); }
        }

        // Property values.
        private readonly AssetId spriteAssetID;
        private readonly String animationName;
        private readonly Int32 animationIndex;
    }
}
