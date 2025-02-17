﻿using System;
using System.Runtime.InteropServices;
using SDL_JoystickID = System.Int32;

namespace Sedulous.Sdl2.Native
{
#pragma warning disable 1591
    [StructLayout(LayoutKind.Sequential)]
    public struct SDL_ControllerButtonEvent
    {
        public UInt32 type;
        public UInt32 timestamp;
        public SDL_JoystickID which;
        public Byte button;
        public Byte state;
        public Byte padding1;
        public Byte epadding2;
    }
#pragma warning restore 1591
}
