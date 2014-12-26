﻿using System;

namespace TwistedLogik.Gluon
{
	public static unsafe partial class gl
	{
        private delegate void glTexStorage1DDelegate(uint target, int levels, uint internalformat, int width);
        [Require(MinVersion = "4.2", Extension = "GL_ARB_texture_storage", ExtensionFunction = "glTextureStorage1DEXT")]
        private static readonly glTexStorage1DDelegate glTexStorage1D = null;

        public static void TexStorage1D(uint target, int levels, uint internalformat, int width)
        {
            glTexStorage1D(target, levels, internalformat, width);
        }

        private delegate void glTexStorage2DDelegate(uint target, int levels, uint internalformat, int width, int height);
        [Require(MinVersion = "4.2", Extension = "GL_ARB_texture_storage", ExtensionFunction = "glTextureStorage2DEXT")]
        private static readonly glTexStorage2DDelegate glTexStorage2D = null;

        public static void TexStorage2D(uint target, int levels, uint internalformat, int width, int height)
        {
            glTexStorage2D(target, levels, internalformat, width, height);
        }

        private delegate void glTexStorage3DDelegate(uint target, int levels, uint internalformat, int width, int height, int depth);
        [Require(MinVersion = "4.2", Extension = "GL_ARB_texture_storage", ExtensionFunction = "glTextureStorage3DEXT")]
        private static readonly glTexStorage3DDelegate glTexStorage3D = null;

        public static void TexStorage3D(uint target, int levels, uint internalformat, int width, int height, int depth)
        {
            glTexStorage3D(target, levels, internalformat, width, height, depth);
        }

        public const UInt32 TEXTURE_IMMUTABLE_FORMAT = 0x912F;
    }
}
