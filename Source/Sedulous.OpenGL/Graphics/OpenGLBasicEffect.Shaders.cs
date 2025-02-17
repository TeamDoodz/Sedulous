﻿using System;
using Sedulous.Core;
using Sedulous.Graphics;

namespace Sedulous.OpenGL.Graphics
{
    partial class OpenGLBasicEffect
    {
        /// <summary>
        /// Creates the effect implementation.
        /// </summary>
        /// <param name="context">The Sedulous context.</param>
        /// <returns>The effect implementation.</returns>
        private static EffectImplementation CreateEffectImplementation(FrameworkContext context)
        {
            Contract.Require(context, nameof(context));

            var programs = new OpenGLShaderProgram[VSIndices.Length];
            for (var i = 0; i < programs.Length; i++)
            {
                var vShader = VSArray[VSIndices[i]];
                var fShader = PSArray[PSIndices[i]];
                programs[i] = new OpenGLShaderProgram(context, vShader, fShader, false);
            }

            var passes = new[] { new OpenGLEffectPass(context, null, programs) };
            var techniques = new[] { new OpenGLEffectTechnique(context, null, passes) };
            return new OpenGLEffectImplementation(context, techniques);
        }

        // An array containing all of the vertex shaders used by this effect.
        private static readonly FrameworkSingleton<OpenGLVertexShader>[] VSArray = new[]
        {
            new FrameworkSingleton<OpenGLVertexShader>(FrameworkSingletonFlags.DisabledInServiceMode | FrameworkSingletonFlags.Lazy,
                uv => { return new OpenGLVertexShader(uv, ResourceUtil.ReadShaderResourceString("BasicEffect_VSBasic.vert")); }),
            new FrameworkSingleton<OpenGLVertexShader>(FrameworkSingletonFlags.DisabledInServiceMode | FrameworkSingletonFlags.Lazy,
                uv => { return new OpenGLVertexShader(uv, ResourceUtil.ReadShaderResourceString("BasicEffect_VSBasicNoFog.vert")); }),
            new FrameworkSingleton<OpenGLVertexShader>(FrameworkSingletonFlags.DisabledInServiceMode | FrameworkSingletonFlags.Lazy,
                uv => { return new OpenGLVertexShader(uv, ResourceUtil.ReadShaderResourceString("BasicEffect_VSBasicVc.vert")); }),
            new FrameworkSingleton<OpenGLVertexShader>(FrameworkSingletonFlags.DisabledInServiceMode | FrameworkSingletonFlags.Lazy,
                uv => { return new OpenGLVertexShader(uv, ResourceUtil.ReadShaderResourceString("BasicEffect_VSBasicVcNoFog.vert")); }),
            new FrameworkSingleton<OpenGLVertexShader>(FrameworkSingletonFlags.DisabledInServiceMode | FrameworkSingletonFlags.Lazy,
                uv => { return new OpenGLVertexShader(uv, ResourceUtil.ReadShaderResourceString("BasicEffect_VSBasicTx.vert")); }),
            new FrameworkSingleton<OpenGLVertexShader>(FrameworkSingletonFlags.DisabledInServiceMode | FrameworkSingletonFlags.Lazy,
                uv => { return new OpenGLVertexShader(uv, ResourceUtil.ReadShaderResourceString("BasicEffect_VSBasicTxNoFog.vert")); }),
            new FrameworkSingleton<OpenGLVertexShader>(FrameworkSingletonFlags.DisabledInServiceMode | FrameworkSingletonFlags.Lazy,
                uv => { return new OpenGLVertexShader(uv, ResourceUtil.ReadShaderResourceString("BasicEffect_VSBasicTxVc.vert")); }),
            new FrameworkSingleton<OpenGLVertexShader>(FrameworkSingletonFlags.DisabledInServiceMode | FrameworkSingletonFlags.Lazy,
                uv => { return new OpenGLVertexShader(uv, ResourceUtil.ReadShaderResourceString("BasicEffect_VSBasicTxVcNoFog.vert")); }),

            new FrameworkSingleton<OpenGLVertexShader>(FrameworkSingletonFlags.DisabledInServiceMode | FrameworkSingletonFlags.Lazy,
                uv => { return new OpenGLVertexShader(uv, ResourceUtil.ReadShaderResourceString("BasicEffect_VSBasicVertexLighting.vert")); }),
            new FrameworkSingleton<OpenGLVertexShader>(FrameworkSingletonFlags.DisabledInServiceMode | FrameworkSingletonFlags.Lazy,
                uv => { return new OpenGLVertexShader(uv, ResourceUtil.ReadShaderResourceString("BasicEffect_VSBasicVertexLightingVc.vert")); }),
            new FrameworkSingleton<OpenGLVertexShader>(FrameworkSingletonFlags.DisabledInServiceMode | FrameworkSingletonFlags.Lazy,
                uv => { return new OpenGLVertexShader(uv, ResourceUtil.ReadShaderResourceString("BasicEffect_VSBasicVertexLightingTx.vert")); }),
            new FrameworkSingleton<OpenGLVertexShader>(FrameworkSingletonFlags.DisabledInServiceMode | FrameworkSingletonFlags.Lazy,
                uv => { return new OpenGLVertexShader(uv, ResourceUtil.ReadShaderResourceString("BasicEffect_VSBasicVertexLightingTxVc.vert")); }),

            new FrameworkSingleton<OpenGLVertexShader>(FrameworkSingletonFlags.DisabledInServiceMode | FrameworkSingletonFlags.Lazy,
                uv => { return new OpenGLVertexShader(uv, ResourceUtil.ReadShaderResourceString("BasicEffect_VSBasicOneLight.vert")); }),
            new FrameworkSingleton<OpenGLVertexShader>(FrameworkSingletonFlags.DisabledInServiceMode | FrameworkSingletonFlags.Lazy,
                uv => { return new OpenGLVertexShader(uv, ResourceUtil.ReadShaderResourceString("BasicEffect_VSBasicOneLightVc.vert")); }),
            new FrameworkSingleton<OpenGLVertexShader>(FrameworkSingletonFlags.DisabledInServiceMode | FrameworkSingletonFlags.Lazy,
                uv => { return new OpenGLVertexShader(uv, ResourceUtil.ReadShaderResourceString("BasicEffect_VSBasicOneLightTx.vert")); }),
            new FrameworkSingleton<OpenGLVertexShader>(FrameworkSingletonFlags.DisabledInServiceMode | FrameworkSingletonFlags.Lazy,
                uv => { return new OpenGLVertexShader(uv, ResourceUtil.ReadShaderResourceString("BasicEffect_VSBasicOneLightTxVc.vert")); }),

            new FrameworkSingleton<OpenGLVertexShader>(FrameworkSingletonFlags.DisabledInServiceMode | FrameworkSingletonFlags.Lazy,
                uv => { return new OpenGLVertexShader(uv, ResourceUtil.ReadShaderResourceString("BasicEffect_VSBasicPixelLighting.vert")); }),
            new FrameworkSingleton<OpenGLVertexShader>(FrameworkSingletonFlags.DisabledInServiceMode | FrameworkSingletonFlags.Lazy,
                uv => { return new OpenGLVertexShader(uv, ResourceUtil.ReadShaderResourceString("BasicEffect_VSBasicPixelLightingVc.vert")); }),
            new FrameworkSingleton<OpenGLVertexShader>(FrameworkSingletonFlags.DisabledInServiceMode | FrameworkSingletonFlags.Lazy,
                uv => { return new OpenGLVertexShader(uv, ResourceUtil.ReadShaderResourceString("BasicEffect_VSBasicPixelLightingTx.vert")); }),
            new FrameworkSingleton<OpenGLVertexShader>(FrameworkSingletonFlags.DisabledInServiceMode | FrameworkSingletonFlags.Lazy,
                uv => { return new OpenGLVertexShader(uv, ResourceUtil.ReadShaderResourceString("BasicEffect_VSBasicPixelLightingTxVc.vert")); }),
        };

        // An array correlating the shader index of this effect to the vertex shader which that index uses.
        private static readonly Int32[] VSIndices = new[]
        {
            0,      // basic
            1,      // no fog
            2,      // vertex color
            3,      // vertex color, no fog
            4,      // texture
            5,      // texture, no fog
            6,      // texture + vertex color
            7,      // texture + vertex color, no fog
    
            8,      // vertex lighting
            8,      // vertex lighting, no fog
            9,      // vertex lighting + vertex color
            9,      // vertex lighting + vertex color, no fog
            10,     // vertex lighting + texture
            10,     // vertex lighting + texture, no fog
            11,     // vertex lighting + texture + vertex color
            11,     // vertex lighting + texture + vertex color, no fog
    
            12,     // one light
            12,     // one light, no fog
            13,     // one light + vertex color
            13,     // one light + vertex color, no fog
            14,     // one light + texture
            14,     // one light + texture, no fog
            15,     // one light + texture + vertex color
            15,     // one light + texture + vertex color, no fog
            
            16,     // pixel lighting
            16,     // pixel lighting, no fog
            17,     // pixel lighting + vertex color
            17,     // pixel lighting + vertex color, no fog
            18,     // pixel lighting + texture
            18,     // pixel lighting + texture, no fog
            19,     // pixel lighting + texture + vertex color
            19,     // pixel lighting + texture + vertex color, no fog
        };

        // An array containing all of the fragment shaders used by this effect.
        private static readonly FrameworkSingleton<OpenGLFragmentShader>[] PSArray = new[]
        {
            new FrameworkSingleton<OpenGLFragmentShader>(FrameworkSingletonFlags.DisabledInServiceMode | FrameworkSingletonFlags.Lazy,
                uv => { return new OpenGLFragmentShader(uv, ResourceUtil.ReadShaderResourceString("BasicEffect_PSBasic.frag")); }),
            new FrameworkSingleton<OpenGLFragmentShader>(FrameworkSingletonFlags.DisabledInServiceMode | FrameworkSingletonFlags.Lazy,
                uv => { return new OpenGLFragmentShader(uv, ResourceUtil.ReadShaderResourceString("BasicEffect_PSBasicNoFog.frag")); }),
            new FrameworkSingleton<OpenGLFragmentShader>(FrameworkSingletonFlags.DisabledInServiceMode | FrameworkSingletonFlags.Lazy,
                uv => { return new OpenGLFragmentShader(uv, ResourceUtil.ReadShaderResourceString("BasicEffect_PSBasicTx.frag")); }),
            new FrameworkSingleton<OpenGLFragmentShader>(FrameworkSingletonFlags.DisabledInServiceMode | FrameworkSingletonFlags.Lazy,
                uv => { return new OpenGLFragmentShader(uv, ResourceUtil.ReadShaderResourceString("BasicEffect_PSBasicTxNoFog.frag")); }),

            new FrameworkSingleton<OpenGLFragmentShader>(FrameworkSingletonFlags.DisabledInServiceMode | FrameworkSingletonFlags.Lazy,
                uv => { return new OpenGLFragmentShader(uv, ResourceUtil.ReadShaderResourceString("BasicEffect_PSBasicVertexLighting.frag")); }),
            new FrameworkSingleton<OpenGLFragmentShader>(FrameworkSingletonFlags.DisabledInServiceMode | FrameworkSingletonFlags.Lazy,
                uv => { return new OpenGLFragmentShader(uv, ResourceUtil.ReadShaderResourceString("BasicEffect_PSBasicVertexLightingNoFog.frag")); }),
            new FrameworkSingleton<OpenGLFragmentShader>(FrameworkSingletonFlags.DisabledInServiceMode | FrameworkSingletonFlags.Lazy,
                uv => { return new OpenGLFragmentShader(uv, ResourceUtil.ReadShaderResourceString("BasicEffect_PSBasicVertexLightingTx.frag")); }),
            new FrameworkSingleton<OpenGLFragmentShader>(FrameworkSingletonFlags.DisabledInServiceMode | FrameworkSingletonFlags.Lazy,
                uv => { return new OpenGLFragmentShader(uv, ResourceUtil.ReadShaderResourceString("BasicEffect_PSBasicVertexLightingTxNoFog.frag")); }),

            new FrameworkSingleton<OpenGLFragmentShader>(FrameworkSingletonFlags.DisabledInServiceMode | FrameworkSingletonFlags.Lazy,
                uv => { return new OpenGLFragmentShader(uv, ResourceUtil.ReadShaderResourceString("BasicEffect_PSBasicPixelLighting.frag")); }),
            new FrameworkSingleton<OpenGLFragmentShader>(FrameworkSingletonFlags.DisabledInServiceMode | FrameworkSingletonFlags.Lazy,
                uv => { return new OpenGLFragmentShader(uv, ResourceUtil.ReadShaderResourceString("BasicEffect_PSBasicPixelLightingTx.frag")); }),
        };

        // An array correlating the shader index of this effect to the fragment shader which that index uses.
        private static readonly Int32[] PSIndices = new[]
        {
            0,      // basic
            1,      // no fog
            0,      // vertex color
            1,      // vertex color, no fog
            2,      // texture
            3,      // texture, no fog
            2,      // texture + vertex color
            3,      // texture + vertex color, no fog
    
            4,      // vertex lighting
            5,      // vertex lighting, no fog
            4,      // vertex lighting + vertex color
            5,      // vertex lighting + vertex color, no fog
            6,      // vertex lighting + texture
            7,      // vertex lighting + texture, no fog
            6,      // vertex lighting + texture + vertex color
            7,      // vertex lighting + texture + vertex color, no fog
    
            4,      // one light
            5,      // one light, no fog
            4,      // one light + vertex color
            5,      // one light + vertex color, no fog
            6,      // one light + texture
            7,      // one light + texture, no fog
            6,      // one light + texture + vertex color
            7,      // one light + texture + vertex color, no fog    
            
            8,      // pixel lighting
            8,      // pixel lighting, no fog
            8,      // pixel lighting + vertex color
            8,      // pixel lighting + vertex color, no fog
            9,      // pixel lighting + texture
            9,      // pixel lighting + texture, no fog
            9,      // pixel lighting + texture + vertex color
            9,      // pixel lighting + texture + vertex color, no fog
        };
    }
}
