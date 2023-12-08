using System;
using System.Linq;
using Sedulous.Core;
using Sedulous.Platform;
using Sedulous.Sdl2.Platform;
using static Sedulous.Sdl2.Native.SDLNative;

namespace Sedulous.Sdl2
{
    /// <summary>
    /// Represents the SDL2 implementation of the <see cref="IPlatformSubsystem"/> interface.
    /// </summary>
    public sealed class Sdl2PlatformSubsystem : FrameworkResource, IPlatformSubsystem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Sdl2PlatformSubsystem"/> class.
        /// </summary>
        /// <param name="context">The Sedulous context.</param>
        /// <param name="configuration">The Sedulous Framework's configuration settings.</param>
        public Sdl2PlatformSubsystem(FrameworkContext context, FrameworkConfiguration configuration)
            : base(context)
        {
            this.clipboard = ClipboardService.Create();
            this.messageBoxService = MessageBoxService.Create();
            this.windows = new Sdl2FrameworkWindowInfoOpenGL(context, configuration);
            this.displays = new Sdl2FrameworkDisplayInfo(context);
            this.isCursorVisible = SDL_ShowCursor(SDL_QUERY) != 0;
        }

        /// <inheritdoc/>
        public void Update(FrameworkTime time)
        {
            Contract.EnsureNotDisposed(this, Disposed);

            this.displays.Update(time);
            this.windows.Update(time);

            OnUpdating(time);
        }

        /// <inheritdoc/>
        public void InitializePrimaryWindow(FrameworkConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            if (IsPrimaryWindowInitialized)
                throw new InvalidOperationException();

            this.windows.InitializePrimaryWindow(FrameworkContext, configuration);
            this.IsPrimaryWindowInitialized = true;
        }

        /// <inheritdoc/>
        public void ShowMessageBox(MessageBoxType type, String title, String message, IFrameworkWindow parent = null)
        {
            Contract.EnsureNotDisposed(this, Disposed);

            if (parent == null)
                parent = Windows.GetPrimary();

            var window = (parent == null) ? IntPtr.Zero : (IntPtr)((Sdl2FrameworkWindow)parent);
            messageBoxService.ShowMessageBox(type, title, message, window);
        }

        /// <inheritdoc/>
        public Boolean IsPrimaryWindowInitialized
        {
            get;
            private set;
        }

        /// <inheritdoc/>
        public Boolean IsCursorVisible
        {
            get { return isCursorVisible; }
            set
            {
                if (value != isCursorVisible)
                {
                    var result = SDL_ShowCursor(value ? SDL_ENABLE : SDL_DISABLE);
                    if (result < 0)
                        throw new Sdl2Exception();

                    isCursorVisible = SDL_ShowCursor(SDL_QUERY) != 0;
                }
            }
        }

        /// <inheritdoc/>
        public Cursor Cursor
        {
            get => cursor;
            set
            {
                Contract.EnsureNotDisposed(this, Disposed);

                cursor = value;

                unsafe
                {
                    var sdlCursor = (value is Sdl2Cursor sdl2cursor) ? sdl2cursor.Native : null;
                    if (sdlCursor == null)
                        sdlCursor = SDL_GetDefaultCursor();

                    SDL_SetCursor(sdlCursor);
                }
            }
        }

        /// <inheritdoc/>
        public ClipboardService Clipboard => clipboard;

        /// <inheritdoc/>
        public IFrameworkWindowInfo Windows => windows;

        /// <inheritdoc/>
        public IFrameworkDisplayInfo Displays => displays;

        /// <inheritdoc/>
        public event FrameworkSubsystemUpdateEventHandler Updating;

        /// <inheritdoc/>
        protected override void Dispose(Boolean disposing)
        {
            if (disposing && !Disposed)
            {
                windows.DesignateCurrent(null, IntPtr.Zero);
                foreach (Sdl2FrameworkWindow window in windows.ToList())
                {
                    windows.Destroy(window);
                }
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Raises the Updating event.
        /// </summary>
        /// <param name="time">Time elapsed since the last call to Update.</param>
        private void OnUpdating(FrameworkTime time) =>
            Updating?.Invoke(this, time);

        // Property values.
        private Boolean isCursorVisible = true;
        private Cursor cursor;
        private readonly ClipboardService clipboard;
        private readonly MessageBoxService messageBoxService;
        private readonly Sdl2FrameworkWindowInfoOpenGL windows;
        private readonly Sdl2FrameworkDisplayInfo displays;
    }
}
