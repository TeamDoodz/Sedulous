
namespace Sedulous
{
    /// <summary>
    /// Represents the standard set of Sedulous Framework events.
    /// </summary>
    public static partial class FrameworkMessages
    {
        /// <summary>
        /// An event indicating that the application should exit.
        /// </summary>
        public static readonly FrameworkMessageId Quit = FrameworkMessageId.Acquire(nameof(Quit));

        /// <summary>
        /// An event indicating that the screen orientation has changed.
        /// </summary>
        public static readonly FrameworkMessageId OrientationChanged = FrameworkMessageId.Acquire(nameof(OrientationChanged));

        /// <summary>
        /// An event indicating that the application has been created by the operating system.
        /// </summary>
        public static readonly FrameworkMessageId ApplicationCreated = FrameworkMessageId.Acquire(nameof(ApplicationCreated));

        /// <summary>
        /// An event indicating that the application is being terminated by the operating system.
        /// </summary>
        public static readonly FrameworkMessageId ApplicationTerminating = FrameworkMessageId.Acquire(nameof(ApplicationTerminating));

        /// <summary>
        /// An event indicating that the application is about to be suspended.
        /// </summary>
        public static readonly FrameworkMessageId ApplicationSuspending = FrameworkMessageId.Acquire(nameof(ApplicationSuspending));

        /// <summary>
        /// An event indicating that the application was suspended.
        /// </summary>
        public static readonly FrameworkMessageId ApplicationSuspended = FrameworkMessageId.Acquire(nameof(ApplicationSuspended));

        /// <summary>
        /// An event indicating that the application is about to resume after being suspended.
        /// </summary>
        public static readonly FrameworkMessageId ApplicationResuming = FrameworkMessageId.Acquire(nameof(ApplicationResuming));

        /// <summary>
        /// An event indicating that the application was resumed after being suspended.
        /// </summary>
        public static readonly FrameworkMessageId ApplicationResumed = FrameworkMessageId.Acquire(nameof(ApplicationResumed));

        /// <summary>
        /// An event indicating that the operation system is low on memory.
        /// </summary>
        public static readonly FrameworkMessageId LowMemory = FrameworkMessageId.Acquire(nameof(LowMemory));

        /// <summary>
        /// An event indicating that the software keyboard was shown.
        /// </summary>
        public static readonly FrameworkMessageId SoftwareKeyboardShown = FrameworkMessageId.Acquire(nameof(SoftwareKeyboardShown));

        /// <summary>
        /// An event indicating that the software keyboard was hidden.
        /// </summary>
        public static readonly FrameworkMessageId SoftwareKeyboardHidden = FrameworkMessageId.Acquire(nameof(SoftwareKeyboardHidden));

        /// <summary>
        /// An event indicating that the text input region has been changed.
        /// </summary>
        public static readonly FrameworkMessageId TextInputRegionChanged = FrameworkMessageId.Acquire(nameof(TextInputRegionChanged));

        /// <summary>
        /// An event indicating that the density settings for a particular display were changed.
        /// </summary>
        public static readonly FrameworkMessageId DisplayDensityChanged = FrameworkMessageId.Acquire(nameof(DisplayDensityChanged));

        /// <summary>
        /// An event indicating that a window was moved to a display with a different density.
        /// </summary>
        public static readonly FrameworkMessageId WindowDensityChanged = FrameworkMessageId.Acquire(nameof(WindowDensityChanged));

        /// <summary>
        /// An event indicating that the file source for content assets has changed.
        /// </summary>
        public static readonly FrameworkMessageId FileSourceChanged = FrameworkMessageId.Acquire(nameof(FileSourceChanged));
    }
}
