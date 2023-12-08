﻿using System;
using System.Runtime.InteropServices;

namespace Sedulous.Sdl2.Native
{
#pragma warning disable 1591
    [StructLayout(LayoutKind.Sequential)]
    public struct SDL_OSEvent
    {
        public UInt32 type;
        public UInt32 timestamp;
    }
#pragma warning restore 1591
}