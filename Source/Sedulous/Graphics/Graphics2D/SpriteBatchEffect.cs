﻿using System;
using System.Drawing;
using System.Numerics;

namespace Sedulous.Graphics.Graphics2D
{
    /// <summary>
    /// Represents a factory method which constructs instances of the <see cref="SpriteBatchEffect"/> class.
    /// </summary>
    /// <param name="context">The Framework context.</param>
    /// <returns>The instance of <see cref="SpriteBatchEffect"/> that was created.</returns>
    public delegate SpriteBatchEffect SpriteBatchEffectFactory(FrameworkContext context);

    /// <summary>
    /// Represents the <see cref="Effect"/> used by <see cref="SpriteBatchBase{VertexType, SpriteData}"/> to render sprites.
    /// </summary>
    public abstract class SpriteBatchEffect : Effect, ISpriteBatchEffect, IEffectTexture, IEffectTextureSize
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpriteBatchEffect"/> class.
        /// </summary>
        /// <param name="impl">The <see cref="EffectImplementation"/> that implements the effect.</param>
        protected SpriteBatchEffect(EffectImplementation impl)
            : base(impl)
        {
            this.epTexture = Parameters["Texture"];
            this.epTextureSize = Parameters["TextureSize"];
            this.epMatrixTransform = Parameters["MatrixTransform"];
            this.epSrgbColor = Parameters["SrgbColor"];
        }

        /// <summary>
        /// Creates a new instance of the <see cref="SpriteBatchEffect"/> class.
        /// </summary>
        /// <returns>The instance of <see cref="SpriteBatchEffect"/> that was created.</returns>
        public static SpriteBatchEffect Create()
        {
            var uv = FrameworkContext.DemandCurrent();
            return uv.GetFactoryMethod<SpriteBatchEffectFactory>()(uv);
        }

        /// <inheritdoc/>
        public Texture2D Texture
        {
            get => epTexture?.GetValueTexture2D();
            set => epTexture?.SetValue(value);
        }

        /// <inheritdoc/>
        public Size TextureSize
        {
            // NOTE: On OpenGL ES 2.0 we don't support integer vertex attributes and therefore
            // we don't do the normalization in the GLSL, which means that TextureSize gets
            // optimized out. So we can just ignore it.
            get {
				Vector2 vector2 = epTextureSize?.GetValueVector2() ?? Vector2.Zero;
				return new Size((int)vector2.X, (int)vector2.Y);
			}
			set => epTextureSize?.SetValue(new Vector2(value.Width, value.Height));
        }

        /// <inheritdoc/>
        public Matrix4x4 MatrixTransform
        {
            get => epMatrixTransform.GetValueMatrix();
            set => epMatrixTransform.SetValue(value);
        }

        /// <inheritdoc/>
        public Boolean SrgbColor
        {
            get => epSrgbColor?.GetValueBoolean() ?? false;
            set => epSrgbColor?.SetValue(value);
        }

        // Cached effect parameters.
        private readonly EffectParameter epTexture;
        private readonly EffectParameter epTextureSize;
        private readonly EffectParameter epMatrixTransform;
        private readonly EffectParameter epSrgbColor;
    }
}
