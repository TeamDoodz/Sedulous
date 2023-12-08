﻿using System;
using System.Runtime.InteropServices;

namespace Sedulous.Sdl2.Native
{
#pragma warning disable 1591
    [StructLayout(LayoutKind.Sequential)]
    public struct SDL_SysWMinfo_wl
    {
        public IntPtr display;
        public IntPtr surface;
        public IntPtr shell_surface;
    }
#pragma warning restore 1591
}
