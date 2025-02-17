﻿using System;
using System.Runtime.InteropServices;

namespace Sedulous.Sdl2.Native
{
#pragma warning disable 1591
    [StructLayout(LayoutKind.Sequential)]
    public struct SDL_MouseWheelEvent
    {
        public UInt32 type;
        public UInt32 timestamp;
        public UInt32 windowID;
        public UInt32 which;
        public Int32 x;
        public Int32 y;
    }
#pragma warning restore 1591
}