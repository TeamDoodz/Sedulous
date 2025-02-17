﻿using System;
using System.Runtime.InteropServices;

namespace Sedulous.FreeType2.Native
{
#pragma warning disable 1591
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct FT_CharMapRec64
    {
        public FT_FaceRec64* face;
        public FT_Encoding encoding;
        public UInt16 platform_id;
        public UInt16 encoding_id;
    }
#pragma warning restore 1591
}
