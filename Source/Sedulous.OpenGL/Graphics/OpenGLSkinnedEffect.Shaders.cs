﻿using System;
using System.Linq;
using Sedulous.Core;
using Sedulous.Graphics;

namespace Sedulous.OpenGL.Graphics
{
    partial class OpenGLSkinnedEffect
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
                uv => { return new OpenGLVertexShader(uv, ResourceUtil.ReadShaderResourceString("SkinnedEffect_VSSkinnedVertexLightingOneBone.vert")); }),
            new FrameworkSingleton<OpenGLVertexShader>(FrameworkSingletonFlags.DisabledInServiceMode | FrameworkSingletonFlags.Lazy,
                uv => { return new OpenGLVertexShader(uv, ResourceUtil.ReadShaderResourceString("SkinnedEffect_VSSkinnedVertexLightingTwoBones.vert")); }),
            new FrameworkSingleton<OpenGLVertexShader>(FrameworkSingletonFlags.DisabledInServiceMode | FrameworkSingletonFlags.Lazy,
                uv => { return new OpenGLVertexShader(uv, ResourceUtil.ReadShaderResourceString("SkinnedEffect_VSSkinnedVertexLightingFourBones.vert")); }),

            new FrameworkSingleton<OpenGLVertexShader>(FrameworkSingletonFlags.DisabledInServiceMode | FrameworkSingletonFlags.Lazy,
                uv => { return new OpenGLVertexShader(uv, ResourceUtil.ReadShaderResourceString("SkinnedEffect_VSSkinnedOneLightOneBone.vert")); }),
            new FrameworkSingleton<OpenGLVertexShader>(FrameworkSingletonFlags.DisabledInServiceMode | FrameworkSingletonFlags.Lazy,
                uv => { return new OpenGLVertexShader(uv, ResourceUtil.ReadShaderResourceString("SkinnedEffect_VSSkinnedOneLightTwoBones.vert")); }),
            new FrameworkSingleton<OpenGLVertexShader>(FrameworkSingletonFlags.DisabledInServiceMode | FrameworkSingletonFlags.Lazy,
                uv => { return new OpenGLVertexShader(uv, ResourceUtil.ReadShaderResourceString("SkinnedEffect_VSSkinnedOneLightFourBones.vert")); }),

            new FrameworkSingleton<OpenGLVertexShader>(FrameworkSingletonFlags.DisabledInServiceMode | FrameworkSingletonFlags.Lazy,
                uv => { return new OpenGLVertexShader(uv, ResourceUtil.ReadShaderResourceString("SkinnedEffect_VSSkinnedPixelLightingOneBone.vert")); }),
            new FrameworkSingleton<OpenGLVertexShader>(FrameworkSingletonFlags.DisabledInServiceMode | FrameworkSingletonFlags.Lazy,
                uv => { return new OpenGLVertexShader(uv, ResourceUtil.ReadShaderResourceString("SkinnedEffect_VSSkinnedPixelLightingTwoBones.vert")); }),
            new FrameworkSingleton<OpenGLVertexShader>(FrameworkSingletonFlags.DisabledInServiceMode | FrameworkSingletonFlags.Lazy,
                uv => { return new OpenGLVertexShader(uv, ResourceUtil.ReadShaderResourceString("SkinnedEffect_VSSkinnedPixelLightingFourBones.vert")); }),
        };

        // An array correlating the shader index of this effect to the vertex shader which that index uses.
        private static readonly Int32[] VSIndices = new[]
        {
            0,      // vertex lighting, one bone
            0,      // vertex lighting, one bone, no fog
            1,      // vertex lighting, two bones
            1,      // vertex lighting, two bones, no fog
            2,      // vertex lighting, four bones
            2,      // vertex lighting, four bones, no fog
    
            3,      // one light, one bone
            3,      // one light, one bone, no fog
            4,      // one light, two bones
            4,      // one light, two bones, no fog
            5,      // one light, four bones
            5,      // one light, four bones, no fog
    
            6,      // pixel lighting, one bone
            6,      // pixel lighting, one bone, no fog
            7,      // pixel lighting, two bones
            7,      // pixel lighting, two bones, no fog
            8,      // pixel lighting, four bones
            8,      // pixel lighting, four bones, no fog
        };

        // An array containing all of the fragment shaders used by this effect.
        private static readonly FrameworkSingleton<OpenGLFragmentShader>[] PSArray = new[]
        {
            new FrameworkSingleton<OpenGLFragmentShader>(FrameworkSingletonFlags.DisabledInServiceMode | FrameworkSingletonFlags.Lazy,
                uv => { return new OpenGLFragmentShader(uv, ResourceUtil.ReadShaderResourceString("SkinnedEffect_PSSkinnedVertexLighting.frag")); }),
            new FrameworkSingleton<OpenGLFragmentShader>(FrameworkSingletonFlags.DisabledInServiceMode | FrameworkSingletonFlags.Lazy,
                uv => { return new OpenGLFragmentShader(uv, ResourceUtil.ReadShaderResourceString("SkinnedEffect_PSSkinnedVertexLightingNoFog.frag")); }),
            new FrameworkSingleton<OpenGLFragmentShader>(FrameworkSingletonFlags.DisabledInServiceMode | FrameworkSingletonFlags.Lazy,
                uv => { return new OpenGLFragmentShader(uv, ResourceUtil.ReadShaderResourceString("SkinnedEffect_PSSkinnedPixelLighting.frag")); }),
        };

        // An array correlating the shader index of this effect to the fragment shader which that index uses.
        private static readonly Int32[] PSIndices = new[]
        {
            0,      // vertex lighting, one bone
            1,      // vertex lighting, one bone, no fog
            0,      // vertex lighting, two bones
            1,      // vertex lighting, two bones, no fog
            0,      // vertex lighting, four bones
            1,      // vertex lighting, four bones, no fog
    
            0,      // one light, one bone
            1,      // one light, one bone, no fog
            0,      // one light, two bones
            1,      // one light, two bones, no fog
            0,      // one light, four bones
            1,      // one light, four bones, no fog
    
            2,      // pixel lighting, one bone
            2,      // pixel lighting, one bone, no fog
            2,      // pixel lighting, two bones
            2,      // pixel lighting, two bones, no fog
            2,      // pixel lighting, four bones
            2,      // pixel lighting, four bones, no fog
        };
    }
}
