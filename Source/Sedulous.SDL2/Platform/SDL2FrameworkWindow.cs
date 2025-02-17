﻿using System;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;
using Sedulous.Core;
using Sedulous.Core.Messages;
using Sedulous.Graphics;
using Sedulous.Messages;
using Sedulous.Platform;
using Sedulous.Sdl2.Native;
using Sedulous.Sdl2.Platform.Surface;
using static Sedulous.Sdl2.Native.SDL_EventType;
using static Sedulous.Sdl2.Native.SDL_PixelFormatEnum;
using static Sedulous.Sdl2.Native.SDL_SysWM_Type;
using static Sedulous.Sdl2.Native.SDL_WindowEventID;
using static Sedulous.Sdl2.Native.SDL_WindowFlags;
using static Sedulous.Sdl2.Native.SDLNative;

namespace Sedulous.Sdl2.Platform
{
    /// <summary>
    /// Represents the SDL2 implementation of the <see cref="IFrameworkWindow"/> interface.
    /// </summary>    
    public sealed unsafe partial class Sdl2FrameworkWindow : FrameworkResource,
        IMessageSubscriber<FrameworkMessageId>,
        IFrameworkWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Sdl2FrameworkWindow"/> class.
        /// </summary>
        /// <param name="context">The Sedulous context.</param>
        /// <param name="ptr">The SDL2 pointer that represents the window.</param>
        /// <param name="visible">A value indicating whether this window should be visible by default.</param>
        /// <param name="native">A value indicating whether the window was created from a native pointer.</param>
        internal Sdl2FrameworkWindow(FrameworkContext context, IntPtr ptr, Boolean visible, Boolean native = false)
            : base(context)
        {
            this.ptr = ptr;
            this.Id = (Int32)SDL_GetWindowID(ptr);
            this.Native = native;

            SetIcon(DefaultWindowIcon.Value);

            FixPlatformSpecificIssues();

            UpdateWindowedPosition(Position);
            UpdateWindowedClientSize(ClientSize);

            var flags = SDL_GetWindowFlags(ptr);

            if ((flags & SDL_WINDOW_OPENGL) == SDL_WINDOW_OPENGL)
                this.windowStatus |= WindowStatusFlags.OpenGL;

            if ((flags & SDL_WINDOW_VULKAN) == SDL_WINDOW_VULKAN)
                this.windowStatus |= WindowStatusFlags.Vulkan;

            if ((flags & SDL_WINDOW_INPUT_FOCUS) == SDL_WINDOW_INPUT_FOCUS)
                this.windowStatus |= WindowStatusFlags.Focused;

            if ((flags & SDL_WINDOW_MINIMIZED) == SDL_WINDOW_MINIMIZED)
                this.windowStatus |= WindowStatusFlags.Minimized;

            if ((flags & SDL_WINDOW_HIDDEN) == SDL_WINDOW_HIDDEN && visible)
                this.windowStatus |= WindowStatusFlags.Unshown;

            this.WindowScale = Display?.DensityScale ?? 1f;

            ChangeCompositor(DefaultCompositor.Create(this));
        }

        /// <summary>
        /// Explicitly converts an Sedulous window to its underlying SDL2 pointer.
        /// </summary>
        /// <param name="window">The Sedulous window to convert.</param>
        /// <returns>The window's underlying SDL2 pointer.</returns>
        public static explicit operator IntPtr(Sdl2FrameworkWindow window)
        {
            return (window == null) ? IntPtr.Zero : window.ptr;
        }

        /// <inheritdoc/>
        void IMessageSubscriber<FrameworkMessageId>.ReceiveMessage(FrameworkMessageId type, MessageData data)
        {
            if (type != Sdl2FrameworkMessages.SdlEvent)
                return;

            var msg = (Messages.Sdl2EventMessageData)data;
            if (msg.Event.type != SDL_WINDOWEVENT || msg.Event.window.windowID != Id)
                return;

            switch (msg.Event.window.@event)
            {
                case SDL_WINDOWEVENT_SHOWN:
                    OnShown();
                    break;

                case SDL_WINDOWEVENT_HIDDEN:
                    OnHidden();
                    break;

                case SDL_WINDOWEVENT_MINIMIZED:
                    this.windowStatus |= WindowStatusFlags.Minimized;
                    OnMinimized();
                    break;

                case SDL_WINDOWEVENT_MAXIMIZED:
                    this.windowStatus &= ~WindowStatusFlags.Minimized;
                    OnMaximized();
                    break;

                case SDL_WINDOWEVENT_RESTORED:
                    this.windowStatus &= ~WindowStatusFlags.Minimized;
                    OnRestored();
                    break;

                case SDL_WINDOWEVENT_MOVED:
                    UpdateWindowedPosition(new Point(msg.Event.window.data1, msg.Event.window.data2));
                    break;

                case SDL_WINDOWEVENT_SIZE_CHANGED:
                    UpdateWindowedClientSize(new Size(msg.Event.window.data1, msg.Event.window.data2));
                    break;

                case SDL_WINDOWEVENT_FOCUS_GAINED:
                    this.windowStatus |= WindowStatusFlags.Focused;
                    break;

                case SDL_WINDOWEVENT_FOCUS_LOST:
                    this.windowStatus &= ~WindowStatusFlags.Focused;
                    break;
            }
        }

        /// <inheritdoc/>
        public void WarpMouseWithinWindow(Int32 x, Int32 y)
        {
            Contract.EnsureNotDisposed(this, Disposed);

            SDL_WarpMouseInWindow(ptr, x, y);
        }

        /// <inheritdoc/>
        public void SetFullscreenDisplayMode(DisplayMode displayMode)
        {
            Contract.EnsureNotDisposed(this, Disposed);

            SetFullscreenDisplayModeInternal(displayMode);
        }

        /// <inheritdoc/>
        public void SetFullscreenDisplayMode(Int32 width, Int32 height, Int32 bpp, Int32 refresh, Int32? displayIndex = null)
        {
            Contract.EnsureNotDisposed(this, Disposed);
            Contract.EnsureRange(width > 0, nameof(width));
            Contract.EnsureRange(height > 0, nameof(height));
            Contract.EnsureRange(bpp > 0, nameof(bpp));
            Contract.EnsureRange(refresh > 0, nameof(refresh));

            if (displayIndex.HasValue)
            {
                var displayIndexValue = displayIndex.Value;
                if (displayIndexValue < 0 || displayIndexValue >= FrameworkContext.GetPlatform().Displays.Count)
                    throw new ArgumentOutOfRangeException(nameof(displayIndex));
            }

            SetFullscreenDisplayModeInternal(new DisplayMode(width, height, bpp, refresh, displayIndex));
        }

        /// <inheritdoc/>
        public DisplayMode GetFullscreenDisplayMode()
        {
            Contract.EnsureNotDisposed(this, Disposed);

            return displayMode;
        }

        /// <inheritdoc/>
        public void SetWindowBounds(Rectangle bounds, Single scale = 1f)
        {
            Contract.EnsureNotDisposed(this, Disposed);
            Contract.EnsureRange(scale >= 1f, nameof(scale));

            this.WindowedPosition = bounds.Location;
            this.WindowedClientSize = bounds.Size;
            this.WindowScale = scale;
        }

        /// <inheritdoc/>
        public void SetWindowedClientSize(Size size, Single scale = 1f)
        {
            Contract.EnsureNotDisposed(this, Disposed);
            Contract.EnsureRange(scale >= 1f, nameof(scale));

            this.WindowedClientSize = size;
            this.WindowScale = scale;
        }

        /// <inheritdoc/>
        public void SetWindowedClientSizeCentered(Size size, Single scale = 1f)
        {
            Contract.EnsureNotDisposed(this, Disposed);
            Contract.EnsureRange(scale >= 1f, nameof(scale));

            this.WindowedClientSize = size;
            this.WindowScale = scale;
            this.WindowedPosition = new Point((Int32)SDL_WINDOWPOS_CENTERED_MASK, (Int32)SDL_WINDOWPOS_CENTERED_MASK);
        }

        /// <inheritdoc/>
        public void SetWindowMode(WindowMode mode)
        {
            Contract.EnsureNotDisposed(this, Disposed);

            if (windowMode == mode)
                return;

            UpdateWindowedPosition(Position);
            UpdateWindowedClientSize(ClientSize);

            switch (mode)
            {
                case WindowMode.Windowed:
                    {
                        if (SDL_SetWindowFullscreen(ptr, 0) < 0)
                            throw new Sdl2Exception();

                        var x = windowedPosition?.X ?? FrameworkConfiguration.DefaultWindowPositionX;
                        var y = windowedPosition?.Y ?? FrameworkConfiguration.DefaultWindowPositionY;
                        var w = windowedClientSize?.Width ?? FrameworkConfiguration.DefaultWindowClientWidth;
                        var h = windowedClientSize?.Height ?? FrameworkConfiguration.DefaultWindowClientHeight;

                        if (!ApplyWin32FullscreenWindowedFix_Windowed())
                            SDL_SetWindowBordered(ptr, true);

                        SDL_SetWindowSize(ptr, w, h);
                        SDL_SetWindowPosition(ptr, x, y);

                        if (FrameworkContext.Platform == FrameworkPlatform.Windows)
                            win32CachedStyle = IntPtr.Zero;
                    }
                    break;

                case WindowMode.Fullscreen:
                    {
                        if (displayMode != null)
                        {
                            if (displayMode.DisplayIndex.HasValue)
                            {
                                var display = FrameworkContext.GetPlatform().Displays[displayMode.DisplayIndex.Value];
                                ChangeDisplay(display);
                            }
                        }
                        else
                        {
                            SetDesktopDisplayMode();
                        }

                        if (SDL_SetWindowFullscreen(ptr, (uint)SDL_WINDOW_FULLSCREEN) < 0)
                            throw new Sdl2Exception();

                        if (FrameworkContext.Platform == FrameworkPlatform.Windows)
                            win32CachedStyle = IntPtr.Zero;
                    }
                    break;

                case WindowMode.FullscreenWindowed:
                    {
                        if (SDL_SetWindowFullscreen(ptr, 0) < 0)
                            throw new Sdl2Exception();

                        var displayBounds = Display.Bounds;

                        if (!ApplyWin32FullscreenWindowedFix_FullscreenWindowed())
                            SDL_SetWindowBordered(ptr, false);

                        SDL_SetWindowSize(ptr, displayBounds.Width, displayBounds.Height);
                        SDL_SetWindowPosition(ptr, displayBounds.X, displayBounds.Y);
                    }
                    break;

                default:
                    throw new NotSupportedException(nameof(mode));
            }

            windowMode = mode;
            UpdateMouseGrab();
        }

        /// <inheritdoc/>
        public WindowMode GetWindowMode()
        {
            Contract.EnsureNotDisposed(this, Disposed);

            return windowMode;
        }

        /// <inheritdoc/>
        public void SetWindowState(WindowState state)
        {
            switch (state)
            {
                case WindowState.Normal:
                    SDL_RestoreWindow(ptr);
                    break;

                case WindowState.Minimized:
                    SDL_MinimizeWindow(ptr);
                    break;

                case WindowState.Maximized:
                    SDL_MaximizeWindow(ptr);
                    break;

                default:
                    throw new NotSupportedException("state");
            }
        }

        /// <inheritdoc/>
        public WindowState GetWindowState()
        {
            Contract.EnsureNotDisposed(this, Disposed);

            var flags = SDL_GetWindowFlags(ptr);

            if ((flags & SDL_WINDOW_MAXIMIZED) == SDL_WINDOW_MAXIMIZED)
                return WindowState.Maximized;

            if ((flags & SDL_WINDOW_MINIMIZED) == SDL_WINDOW_MINIMIZED)
                return WindowState.Minimized;

            return WindowState.Normal;
        }

        /// <inheritdoc/>
        public void ChangeCompositor(Compositor compositor)
        {
            Contract.EnsureNotDisposed(this, Disposed);

            if (compositor.Window != this)
                throw new InvalidOperationException(FrameworkStrings.CompositorAssociatedWithWrongWindow);

            if (IsCurrentWindow)
                throw new InvalidOperationException(FrameworkStrings.CannotChangeCompositorWhileCurrent);

            this.Compositor?.Dispose();
            this.Compositor = compositor ?? DefaultCompositor.Create(this);
        }

        /// <inheritdoc/>
        public void ChangeDisplay(Int32 displayIndex)
        {
            Contract.EnsureNotDisposed(this, Disposed);

            if (displayIndex < 0 || displayIndex >= FrameworkContext.GetPlatform().Displays.Count)
                displayIndex = 0;

            var display = FrameworkContext.GetPlatform().Displays[displayIndex];
            ChangeDisplay(display);
        }

        /// <inheritdoc/>
        public void ChangeDisplay(IFrameworkDisplay display)
        {
            Contract.EnsureNotDisposed(this, Disposed);
            Contract.Require(display, nameof(display));

            if (Display == display)
                return;

            var x = GetCenter(display.Bounds).X - (ClientSize.Width / 2);
            var y = GetCenter(display.Bounds).Y - (ClientSize.Height / 2);

            Position = new Point((int)x, (int)y);
        }

        private static Vector2 GetCenter(Rectangle rectangle) {
            return new Vector2(rectangle.X + rectangle.Width / 2f, rectangle.Y + rectangle.Height / 2f);
        }

        /// <summary>
        /// Updates the window's state.
        /// </summary>
        /// <param name="time">Time elapsed since the last call to <see cref="FrameworkContext.Update(FrameworkTime)"/>.</param>
        public void Update(FrameworkTime time)
        {
            if (Display.DensityScale != WindowScale)
                HandleDpiChanged();
        }

        /// <summary>
        /// Draws the window.
        /// </summary>
        /// <param name="time">Time elapsed since the last call to Draw.</param>
        public void Draw(FrameworkTime time)
        {
            OnDrawing(time);
            OnDrawingUI(time);
        }

        /// <inheritdoc/>
        public Int32 Id { get; }

        /// <inheritdoc/>
        public String Caption
        {
            get
            {
                Contract.EnsureNotDisposed(this, Disposed);

                return SDL_GetWindowTitle(ptr);
            }
            set
            {
                Contract.EnsureNotDisposed(this, Disposed);

                SDL_SetWindowTitle(ptr, value ?? String.Empty);
            }
        }

        /// <inheritdoc/>
        public Single WindowScale { get; private set; }

        /// <inheritdoc/>
        public Point Position
        {
            get
            {
                Contract.EnsureNotDisposed(this, Disposed);

                SDL_GetWindowPosition(ptr, out var x, out var y);
                return new Point(x, y);
            }
            set
            {
                Contract.EnsureNotDisposed(this, Disposed);

                if (GetWindowMode() == WindowMode.Windowed && GetWindowState() == WindowState.Normal)
                    windowedPosition = value;

                SDL_SetWindowPosition(ptr, value.X, value.Y);
            }
        }

        /// <inheritdoc/>
        public Point WindowedPosition
        {
            get => windowedPosition.GetValueOrDefault();
            set
            {
                Contract.EnsureNotDisposed(this, Disposed);

                windowedPosition = value;
                if (GetWindowMode() == WindowMode.Windowed && GetWindowState() == WindowState.Normal)
                {
                    SDL_SetWindowPosition(ptr, value.X, value.Y);
                }
            }
        }

        /// <inheritdoc/>
        public Size DrawableSize
        {
            get
            {
                Contract.EnsureNotDisposed(this, Disposed);

                Int32 w, h;

                var isOpenGLWindow = (this.windowStatus & WindowStatusFlags.OpenGL) == WindowStatusFlags.OpenGL;
                if (isOpenGLWindow)
                {
                    SDL_GL_GetDrawableSize(ptr, out w, out h);
                }
                else
                {
                    SDL_GetWindowSize(ptr, out w, out h);
                }

                return new Size(w, h);
            }
        }

        /// <inheritdoc/>
        public Size ClientSize
        {
            get
            {
                Contract.EnsureNotDisposed(this, Disposed);

                SDL_GetWindowSize(ptr, out var w, out var h);
                return new Size(w, h);
            }
            set
            {
                Contract.EnsureNotDisposed(this, Disposed);

                if (GetWindowMode() == WindowMode.Windowed && GetWindowState() == WindowState.Normal)
                {
                    windowedClientSize = value;
                }

                SDL_SetWindowSize(ptr, value.Width, value.Height);
            }
        }

        /// <inheritdoc/>
        public Size WindowedClientSize
        {
            get => windowedClientSize.GetValueOrDefault();
            set
            {
                Contract.EnsureNotDisposed(this, Disposed);

                windowedClientSize = value;
                if (GetWindowMode() == WindowMode.Windowed && GetWindowState() == WindowState.Normal)
                {
                    SDL_SetWindowSize(ptr, value.Width, value.Height);
                }
            }
        }

        /// <inheritdoc/>
        public Size MinimumClientSize
        {
            get
            {
                Contract.EnsureNotDisposed(this, Disposed);

                SDL_GetWindowMinimumSize(ptr, out var w, out var h);
                return new Size(w, h);
            }
            set
            {
                Contract.EnsureNotDisposed(this, Disposed);

                SDL_SetWindowMinimumSize(ptr, value.Width, value.Height);
            }
        }

        /// <inheritdoc/>
        public Size MaximumClientSize
        {
            get
            {
                Contract.EnsureNotDisposed(this, Disposed);

                SDL_GetWindowMaximumSize(ptr, out var w, out var h);
                return new Size(w, h);
            }
            set
            {
                Contract.EnsureNotDisposed(this, Disposed);

                SDL_SetWindowMaximumSize(ptr, value.Width, value.Height);
            }
        }

        /// <inheritdoc/>
        public Boolean SynchronizeWithVerticalRetrace { get; set; } = true;

        /// <inheritdoc/>
        public Boolean Active =>
            (windowStatus & WindowStatusFlags.Focused) == WindowStatusFlags.Focused &&
            (windowStatus & WindowStatusFlags.Minimized) != WindowStatusFlags.Minimized;

        /// <inheritdoc/>
        public Boolean Visible
        {
            get
            {
                Contract.EnsureNotDisposed(this, Disposed);

                var flags = SDL_GetWindowFlags(ptr);
                return (flags & SDL_WINDOW_SHOWN) == SDL_WINDOW_SHOWN;
            }
            set
            {
                Contract.EnsureNotDisposed(this, Disposed);

                if (value)
                {
                    SDL_ShowWindow(ptr);
                }
                else
                {
                    SDL_HideWindow(ptr);
                }
            }
        }

        /// <inheritdoc/>
        public Boolean Resizable
        {
            get
            {
                Contract.EnsureNotDisposed(this, Disposed);

                var flags = SDL_GetWindowFlags(ptr);
                return (flags & SDL_WINDOW_RESIZABLE) == SDL_WINDOW_RESIZABLE;
            }
        }

        /// <inheritdoc/>
        public Boolean Borderless
        {
            get
            {
                Contract.EnsureNotDisposed(this, Disposed);

                var flags = SDL_GetWindowFlags(ptr);
                return (flags & SDL_WINDOW_BORDERLESS) == SDL_WINDOW_BORDERLESS;
            }
        }

        /// <inheritdoc/>
        public Boolean Native { get; }

        /// <inheritdoc/>
        public Boolean GrabsMouseWhenWindowed
        {
            get => grabsMouseWhenWindowed;
            set
            {
                Contract.EnsureNotDisposed(this, Disposed);

                grabsMouseWhenWindowed = value;
                UpdateMouseGrab();
            }
        }

        /// <inheritdoc/>
        public Boolean GrabsMouseWhenFullscreenWindowed
        {
            get => grabsMouseWhenFullscreenWindowed;
            set
            {
                Contract.EnsureNotDisposed(this, Disposed);

                grabsMouseWhenFullscreenWindowed = value;
                UpdateMouseGrab();
            }
        }

        /// <inheritdoc/>
        public Boolean GrabsMouseWhenFullscreen
        {
            get => grabsMouseWhenFullscreen;
            set
            {
                Contract.EnsureNotDisposed(this, Disposed);

                grabsMouseWhenFullscreen = value;
                UpdateMouseGrab();
            }
        }

        /// <inheritdoc/>
        public Single Opacity
        {
            get
            {
                Contract.EnsureNotDisposed(this, Disposed);

                Single opacity;
                SDL_GetWindowOpacity(ptr, &opacity);
                return opacity;
            }
            set
            {
                Contract.EnsureNotDisposed(this, Disposed);

                value = MathUtility.Clamp(value, 0.0f, 1.0f);
                SDL_SetWindowOpacity(ptr, value);
            }
        }

        /// <inheritdoc/>
        public Surface2D Icon
        {
            get => icon;
            set
            {
                Contract.EnsureNotDisposed(this, Disposed);

                SetIcon(value ?? DefaultWindowIcon);
                icon = value;
            }
        }

        /// <inheritdoc/>
        public Compositor Compositor { get; private set; }

        /// <inheritdoc/>
        public IFrameworkDisplay Display
        {
            get
            {
                Contract.EnsureNotDisposed(this, Disposed);

                var index = SDL_GetWindowDisplayIndex(ptr);
                var platform = FrameworkContext.GetPlatform();
                if (platform != null)
                    return FrameworkContext.GetPlatform().Displays[index];

                return null;
            }
        }

        /// <summary>
        /// Occurs when the window is shown.
        /// </summary>
        public event FrameworkWindowEventHandler Shown;

        /// <summary>
        /// Occurs when the window is hidden.
        /// </summary>
        public event FrameworkWindowEventHandler Hidden;

        /// <summary>
        /// Occurs when the window is minimized.
        /// </summary>
        public event FrameworkWindowEventHandler Minimized;

        /// <summary>
        /// Occurs when the window is maximized.
        /// </summary>
        public event FrameworkWindowEventHandler Maximized;

        /// <summary>
        /// Occurs when the window is restored.
        /// </summary>
        public event FrameworkWindowEventHandler Restored;

        /// <summary>
        /// Occurs when the window is rendered.
        /// </summary>
        public event FrameworkWindowDrawingEventHandler Drawing;

        /// <summary>
        /// Occurs when the window is drawing its UI layer.
        /// </summary>
        public event FrameworkWindowDrawingEventHandler DrawingUI;

        /// <summary>
        /// Gets or sets a value indicating whether this is the current window.
        /// </summary>
        internal Boolean IsCurrentWindow
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the window is bound for rendering.
        /// </summary>
        internal Boolean IsBoundForRendering
        {
            get => (windowStatus & WindowStatusFlags.BoundForRendering) == WindowStatusFlags.BoundForRendering;
            set
            {
                if (value)
                {
                    windowStatus |= WindowStatusFlags.BoundForRendering;
                    if ((windowStatus & WindowStatusFlags.Unshown) == WindowStatusFlags.Unshown)
                    {
                        windowStatus &= ~WindowStatusFlags.Unshown;
                        SDL_ShowWindow(ptr);
                    }
                }
                else
                {
                    windowStatus &= ~WindowStatusFlags.BoundForRendering;
                }
            }
        }

        /// <inheritdoc/>
        protected override void Dispose(Boolean disposing)
        {
            if (disposing)
            {
                Compositor?.Dispose();
            }
            SDL_DestroyWindow(ptr);
            base.Dispose(disposing);
        }

        /// <summary>
        /// Loads the default window icon.
        /// </summary>
        /// <param name="context">The Sedulous context.</param>
        /// <returns>The default window icon.</returns>
        private static Surface2D LoadDefaultWindowIcon(FrameworkContext context)
        {
            Contract.Require(context, nameof(context));

            return IconLoader.Create().LoadIcon();
        }

        /// <summary>
        /// Retrieves the low word of a message parameter.
        /// </summary>
        private static Int32 LOWORD(Int32 word) => (word & 0xffff);

        /// <summary>
        /// Retrieves the high word of a message parameter.
        /// </summary>
        private static Int32 HIWORD(Int32 word) => (word >> 16) & 0xffff;

        /// <summary>
        /// Fixes issues specific to the current platform.
        /// </summary>
        private void FixPlatformSpecificIssues()
        {
            SDL_SysWMinfo sysInfo;
            SDL_VERSION(&sysInfo.version);

            if (!SDL_GetWindowWMInfo(ptr, &sysInfo))
                return;

            switch (sysInfo.subsystem)
            {
                case SDL_SYSWM_WINDOWS:
                    FixPlatformSpecificIssues_Windows(ref sysInfo);
                    break;
            }
        }

        /// <summary>
        /// Fixes issues specific to the Windows platform.
        /// </summary>
        /// <param name="sysinfo">The current system information.</param>
        private void FixPlatformSpecificIssues_Windows(ref SDL_SysWMinfo sysinfo)
        {
            var windowHWnd = sysinfo.info.win.window;

            // NOTE: This fix prevents Windows from playing the "ding" sound when
            // a key binding involving the Alt modifier is pressed.
            wndProc = new Win32Native.WndProcDelegate((hWnd, msg, wParam, lParam) =>
            {
                if (hWnd == windowHWnd)
                {
                    const Int32 WM_SYSCOMMAND = 0x0112;
                    const Int32 WM_DPICHANGED = 0x02E0;
                    const Int32 SC_KEYMENU = 0xF100;
                    if (msg == WM_SYSCOMMAND && (wParam.ToInt64() & 0xfff0) == SC_KEYMENU)
                    {
                        return IntPtr.Zero;
                    }
                    if (msg == WM_DPICHANGED)
                    {
                        // NOTE: This one isn't actually a "fix," it just lets us detect if the user
                        // decides to change a display's DPI on Windows.
                        var dpi = LOWORD(wParam.ToInt32());
                        var scale = dpi / 96f;
                        HandleDpiChanged(scale);
                    }
                }
                return Win32Native.CallWindowProc(wndProcPrev, hWnd, msg, wParam, lParam);
            });

            const int GWLP_WNDPROC = -4;
            wndProcPrev = Win32Native.SetWindowLongPtr(sysinfo.info.win.window, GWLP_WNDPROC,
                Marshal.GetFunctionPointerForDelegate(wndProc));
        }

        /// <summary>
        /// Sets the window's fullscreen display mode.
        /// </summary>
        /// <param name="displayMode">The fullscreen display mode to set, or null to use the desktop display mode.</param>
        private void SetFullscreenDisplayModeInternal(DisplayMode displayMode)
        {
            if (displayMode == null)
            {
                SetDesktopDisplayMode();
            }
            else
            {
                SDL_DisplayMode sdlMode;
                sdlMode.w = displayMode.Width;
                sdlMode.h = displayMode.Height;
                sdlMode.refresh_rate = displayMode.RefreshRate;
                switch (displayMode.BitsPerPixel)
                {
                    case 15:
                        sdlMode.format = SDL_PIXELFORMAT_RGB555;
                        break;

                    case 16:
                        sdlMode.format = SDL_PIXELFORMAT_RGB565;
                        break;

                    default:
                        sdlMode.format = SDL_PIXELFORMAT_RGB888;
                        break;
                }

                var wasFullscreen = windowMode == WindowMode.Fullscreen;
                if (wasFullscreen)
                    SetWindowMode(WindowMode.Windowed);

                if (SDL_SetWindowDisplayMode(ptr, &sdlMode) < 0)
                    throw new Sdl2Exception();

                if (wasFullscreen)
                {
                    if (displayMode.DisplayIndex.HasValue)
                    {
                        ChangeDisplay(displayMode.DisplayIndex.Value);
                    }
                    SetWindowMode(WindowMode.Fullscreen);
                }

                if (SDL_GetWindowDisplayMode(ptr, &sdlMode) < 0)
                    throw new Sdl2Exception();

                int bpp;
                uint Rmask, Gmask, Bmask, Amask;
                SDL_PixelFormatEnumToMasks((uint)sdlMode.format, &bpp, &Rmask, &Gmask, &Bmask, &Amask);

                var displayIndex = displayMode.DisplayIndex;
                if (displayIndex.HasValue)
                {
                    if (displayIndex < 0 || displayIndex >= FrameworkContext.GetPlatform().Displays.Count)
                        displayIndex = null;
                }

                displayMode = new DisplayMode(sdlMode.w, sdlMode.h, bpp, sdlMode.refresh_rate, displayIndex);
            }
            this.displayMode = displayMode;
        }

        /// <summary>
        /// Sets the window's icon.
        /// </summary>
        /// <param name="surface">The surface that contains the icon to set.</param>
        private void SetIcon(Surface2D surface)
        {
            var surfptr = (surface == null) ? null : ((Sdl2Surface2D)surface).NativePtr;
            SDL_SetWindowIcon(ptr, (IntPtr)surfptr);
        }

        /// <summary>
        /// Raises the Drawing event.
        /// </summary>
        /// <param name="time">Time elapsed since the last call to Draw.</param>
        private void OnDrawing(FrameworkTime time) =>
            Drawing?.Invoke(this, time);

        /// <summary>
        /// Raises the DrawingUI event.
        /// </summary>
        /// <param name="time">Time elapsed since the last call to Draw.</param>
        private void OnDrawingUI(FrameworkTime time) =>
            DrawingUI?.Invoke(this, time);

        /// <summary>
        /// Raises the Shown event.
        /// </summary>
        private void OnShown() =>
            Shown?.Invoke(this);

        /// <summary>
        /// Raises the Hidden event.
        /// </summary>
        private void OnHidden() =>
            Hidden?.Invoke(this);

        /// <summary>
        /// Raises the Maximized event.
        /// </summary>
        private void OnMaximized() =>
            Maximized?.Invoke(this);

        /// <summary>
        /// Raises the Minimized event.
        /// </summary>
        private void OnMinimized() =>
            Minimized?.Invoke(this);

        /// <summary>
        /// Raises the Restored event.
        /// </summary>
        private void OnRestored() =>
            Restored?.Invoke(this);

        /// <summary>
        /// Called when the window's DPI changes.
        /// </summary>
        private void HandleDpiChanged(Single? reportedScale = null)
        {
            // Inform our display that it needs to re-query DPI information.
            ((Sdl2FrameworkDisplay)Display)?.RefreshDensityInformation();

            // On Windows, resize the window to match the new scale.
            if (FrameworkContext.Platform == FrameworkPlatform.Windows && FrameworkContext.Properties.SupportsHighDensityDisplayModes)
            {
                var factor = (reportedScale ?? Display.DensityScale) / WindowScale;

                SDL_GetWindowPosition(ptr, out var windowX, out var windowY);
                SDL_GetWindowSize(ptr, out var windowW, out var windowH);
                
                var size = new Size((Int32)(windowW * factor), (Int32)(windowH * factor));
                var bounds = new Rectangle(windowX, windowY, windowW, windowH);
                bounds = Rectangle.Inflate(bounds, (Int32)Math.Ceiling((size.Width - windowW) / 2.0), 0);

                WindowedPosition = bounds.Location;
                WindowedClientSize = size;
            }
            WindowScale = (reportedScale ?? Display.DensityScale);

            // Inform the rest of the system that this window's DPI has changed.
            var messageData = FrameworkContext.Messages.CreateMessageData<WindowDensityChangedMessageData>();
            messageData.Window = this;
            FrameworkContext.Messages.Publish(FrameworkMessages.WindowDensityChanged, messageData);
        }

        /// <summary>
        /// Updates the window's windowed position, if it is currently in the correct mode and state.
        /// </summary>
        /// <param name="position">The new windowed position.</param>
        private void UpdateWindowedPosition(Point position)
        {
            if (windowedPosition == null || (GetWindowState() == WindowState.Normal && GetWindowMode() == WindowMode.Windowed))
            {
                windowedPosition = position;
            }
        }

        /// <summary>
        /// Updates the window's windowed client size, if it is currently in the correct mode and state.
        /// </summary>
        /// <param name="size">The new windowed client size.</param>
        private void UpdateWindowedClientSize(Size size)
        {
            if (windowedClientSize == null || (GetWindowState() == WindowState.Normal && GetWindowMode() == WindowMode.Windowed))
            {
                windowedClientSize = size;
            }
        }

        /// <summary>
        /// Updates the window's mouse grab state.
        /// </summary>
        private void UpdateMouseGrab()
        {
            switch (windowMode)
            {
                case WindowMode.Windowed:
                    SDL_SetWindowGrab(ptr, grabsMouseWhenWindowed);
                    break;

                case WindowMode.Fullscreen:
                    SDL_SetWindowGrab(ptr, grabsMouseWhenFullscreen);
                    break;

                case WindowMode.FullscreenWindowed:
                    SDL_SetWindowGrab(ptr, grabsMouseWhenFullscreenWindowed);
                    break;
            }
        }

        /// <summary>
        /// Sets the window to use the desktop display mode for its current display.
        /// </summary>
        private void SetDesktopDisplayMode()
        {
            SDL_DisplayMode mode;
            if (SDL_GetDesktopDisplayMode(Display.Index, &mode) < 0)
                throw new Sdl2Exception();

            if (SDL_SetWindowDisplayMode(ptr, &mode) < 0)
                throw new Sdl2Exception();
        }

        /// <summary>
        /// Retrieves the HWND value for this window on the Windows platform.
        /// </summary>
        private Boolean GetHwnd(out IntPtr hwnd)
        {
            if (FrameworkContext.Platform != FrameworkPlatform.Windows)
                throw new NotSupportedException();

            SDL_SysWMinfo sysInfo;
            SDL_VERSION(&sysInfo.version);

            if (!SDL_GetWindowWMInfo(ptr, &sysInfo))
            {
                hwnd = IntPtr.Zero;
                return false;
            }
            hwnd = sysInfo.info.win.window;
            return true;
        }

        /// <summary>
        /// Applies a fix to the window's styles on the Windows platform which addresses
        /// the flickering which has been observed when using ALT+TAB while the application
        /// is in fullscreen windowed mode.
        /// </summary>
        private Boolean ApplyWin32FullscreenWindowedFix_Windowed()
        {
            if (FrameworkContext.Platform != FrameworkPlatform.Windows || win32CachedStyle == IntPtr.Zero)
                return false;

            IntPtr hwnd;
            if (!GetHwnd(out hwnd))
                return false;

            Win32Native.SetWindowLongPtr(hwnd, Win32Native.GWL_STYLE, win32CachedStyle);
            return true;
        }

        /// <summary>
        /// Applies a fix to the window's styles on the Windows platform which addresses
        /// the flickering which has been observed when using ALT+TAB while the application
        /// is in fullscreen windowed mode.
        /// </summary>
        private Boolean ApplyWin32FullscreenWindowedFix_FullscreenWindowed()
        {
            if (FrameworkContext.Platform != FrameworkPlatform.Windows)
                return false;

            IntPtr hwnd;
            if (!GetHwnd(out hwnd))
                return false;

            win32CachedStyle = Win32Native.GetWindowLongPtr(hwnd, Win32Native.GWL_STYLE);
            if (Environment.Is64BitProcess)
            {
                var style = (UInt64)win32CachedStyle & ~(Win32Native.WS_DLGFRAME | Win32Native.WS_BORDER);
                Win32Native.SetWindowLongPtr(hwnd, Win32Native.GWL_STYLE, new IntPtr((void*)style));
            }
            else
            {
                var style = (UInt32)win32CachedStyle & ~(Win32Native.WS_DLGFRAME | Win32Native.WS_BORDER);
                Win32Native.SetWindowLongPtr(hwnd, Win32Native.GWL_STYLE, new IntPtr((void*)style));
            }

            return true;
        }

        // A custom Win32 windows procedure used to override SDL2's built-in functionality.
        private Win32Native.WndProcDelegate wndProc;
        private IntPtr wndProcPrev;

        // Property values.
        private Point? windowedPosition;
        private Size? windowedClientSize;
        private Boolean grabsMouseWhenWindowed;
        private Boolean grabsMouseWhenFullscreenWindowed;
        private Boolean grabsMouseWhenFullscreen;
        private Surface2D icon;

        // State values.
        private readonly IntPtr ptr;
        private WindowMode windowMode = WindowMode.Windowed;
        private WindowStatusFlags windowStatus = WindowStatusFlags.None;
        private DisplayMode displayMode;
        
        // HACK: Cached style from before entering fullscreen windowed mode.
        private IntPtr win32CachedStyle;

        // The default window icon.
        private static readonly FrameworkSingleton<Surface2D> DefaultWindowIcon = new FrameworkSingleton<Surface2D>(
            uv => LoadDefaultWindowIcon(uv));
    }
}
