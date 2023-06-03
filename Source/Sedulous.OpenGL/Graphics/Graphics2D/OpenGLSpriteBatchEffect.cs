﻿using Sedulous.Core;
using Sedulous.Graphics;
using Sedulous.Graphics.Graphics2D;

namespace Sedulous.OpenGL.Graphics.Graphics2D
{
    /// <summary>
    /// Represents the OpenGL implementation of the sprite batch custom effect.
    /// </summary>
    public sealed class OpenGLSpriteBatchEffect : SpriteBatchEffect
    {
        /// <summary>
        /// Initializes a new instance of the OpenGLSpriteBatchEffect class.
        /// </summary>
        public OpenGLSpriteBatchEffect(FrameworkContext uv)
            : base(CreateEffectImplementation(uv))
        {

        }

        /// <summary>
        /// Creates the effect implementation.
        /// </summary>
        /// <param name="uv">The Sedulous context.</param>
        /// <returns>The effect implementation.</returns>
        private static EffectImplementation CreateEffectImplementation(FrameworkContext uv)
        {
            Contract.Require(uv, nameof(uv));

            var programs = new[] { new OpenGLShaderProgram(uv, vertShader, fragShader, false) };
            var passes = new[] { new OpenGLEffectPass(uv, null, programs) };
            var techniques = new[] { new OpenGLEffectTechnique(uv, null, passes) };
            return new OpenGLEffectImplementation(uv, techniques);
        }

        // The shaders that make up this effect.
        private static readonly FrameworkSingleton<OpenGLVertexShader> vertShader = 
            new FrameworkSingleton<OpenGLVertexShader>(FrameworkSingletonFlags.DisabledInServiceMode | FrameworkSingletonFlags.Lazy, uv => { 
                return new OpenGLVertexShader(uv, ResourceUtil.ReadShaderResourceString("SpriteBatchEffect.vert")); });
        private static readonly FrameworkSingleton<OpenGLFragmentShader> fragShader = 
            new FrameworkSingleton<OpenGLFragmentShader>(FrameworkSingletonFlags.DisabledInServiceMode | FrameworkSingletonFlags.Lazy, uv => {
                return new OpenGLFragmentShader(uv, ResourceUtil.ReadShaderResourceString("SpriteBatchEffect.frag")); });
    }
}
