﻿using System;
using System.Runtime.InteropServices;
using SDL_GestureID = System.Int64;
using SDL_TouchID = System.Int64;

namespace Sedulous.Sdl2.Native
{
#pragma warning disable 1591
    [StructLayout(LayoutKind.Sequential)]
    public struct SDL_DollarGestureEvent
    {
        public UInt32 type;
        public UInt32 timestamp;
        public SDL_TouchID touchId;
        public SDL_GestureID gestureId;
        public UInt32 numFingers;
        public Single error;
        public Single x;
        public Single y;
    }
#pragma warning restore 1591
}
