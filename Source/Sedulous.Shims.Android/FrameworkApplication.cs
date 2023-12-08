﻿using Android.App;
using Android.OS;
using Android.Text;
using Org.Libsdl.App;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml;
using Sedulous.Content;
using Sedulous.Core;
using Sedulous.Core.Messages;
using Sedulous.Messages;
using Sedulous.Platform;
using Sedulous.Shims.Android.Platform;
using Android.Content;
using Sedulous.Graphics;
using Sedulous.Input;
using Sedulous.Shims.Android.Input;
using Sedulous.Shims.Android.Graphics;

namespace Sedulous
{
    /// <summary>
    /// Represents an <see cref="Activity"/> which hosts and runs an Sedulous application.
    /// </summary>
    public abstract class FrameworkApplication : SDLActivity,
        IMessageSubscriber<FrameworkMessageId>,
        IFrameworkComponent,
        IFrameworkHost,
        IDisposable
    {
        /// <summary>
        /// Initializes the <see cref="FrameworkApplication"/> type.
        /// </summary>
        static FrameworkApplication()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
            {
                var dataDir = Android.App.Application.Context.ApplicationContext.DataDir.AbsolutePath;
                Directory.SetCurrentDirectory(dataDir);
            }
            else
            {
                var pkgManager = Android.App.Application.Context.PackageManager;
                var pkgName = Android.App.Application.Context.PackageName;
                var pkgInfo = pkgManager.GetPackageInfo(pkgName, 0);
                var dataDir = pkgInfo.ApplicationInfo.DataDir;
                Directory.SetCurrentDirectory(dataDir);
            }
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="FrameworkApplication"/> class.
		/// </summary>
		/// <param name="developerName">The name of the company or developer who built this application.</param>
		/// <param name="applicationName">The name of the application </param>
		/// <param name="settings">The settings for this application, or null if this application should load settings on its own.</param>
		protected FrameworkApplication(String developerName, String applicationName, FrameworkApplicationSettings settings = null)
        {
            Contract.RequireNotEmpty(developerName, nameof(developerName));
            Contract.RequireNotEmpty(applicationName, nameof(applicationName));

            PreserveApplicationSettings = true;

            this.settings = settings;

            this.DeveloperName = developerName;
            this.ApplicationName = applicationName;
        }

        /// <inheritdoc/>
        void IMessageSubscriber<FrameworkMessageId>.ReceiveMessage(FrameworkMessageId type, MessageData data)
        {
            OnReceivedMessage(type, data);
        }

        /// <inheritdoc/>
        public override void OnConfigurationChanged(Android.Content.Res.Configuration newConfig)
        {
            if (FrameworkContext != null && !FrameworkContext.Disposed)
            {
                var display = FrameworkContext.GetPlatform().Displays[0];
                var rotation = (ScreenRotation)WindowManager.DefaultDisplay.Rotation;

                if (rotation != display.Rotation)
                {
                    AndroidScreenRotationService.UpdateScreenRotation(rotation);

                    var messageData = FrameworkContext.Messages.CreateMessageData<OrientationChangedMessageData>();
                    messageData.Display = display;
                    FrameworkContext.Messages.Publish(FrameworkMessages.OrientationChanged, messageData);
                }
            }

            base.OnConfigurationChanged(newConfig);
        }

        /// <summary>
        /// Runs the Sedulous application.
        /// </summary>
        public void Run()
        {
            Contract.EnsureNotDisposed(this, disposed);

            OnInitializing();

            CreateFrameworkContext();

            InitializeFrameworkContext();

            OnInitialized();

            WarnIfFileSystemSourceIsMissing();

            if(settings is null) {
                LoadSettings();
            }

            OnLoadingContent();

            running = true;
            while (running)
            {
                if (IsSuspended)
                {
                    timingLogic.RunOneTickSuspended();
                }
                else
                {
                    timingLogic.RunOneTick();
                }
                Thread.Yield();
            }

            timingLogic.Cleanup();

            context.WaitForPendingTasks(true);
        }

        /// <summary>
        /// Exits the application.
        /// </summary>
        public void Exit()
        {
            Contract.EnsureNotDisposed(this, disposed);

            running = false;
            finished = true;
        }

        /// <summary>
        /// Gets the Sedulous context.
        /// </summary>
        public FrameworkContext FrameworkContext
        {
            get
            {
                Contract.EnsureNotDisposed(this, disposed);
                Contract.Ensure(created, FrameworkStrings.ContextMissing);

                return context;
            }
        }

        /// <inheritdoc/>
        public String DeveloperName { get; }

        /// <inheritdoc/>
        public String ApplicationName { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the application's primary window is synchronized
        /// to the vertical retrace when rendering (i.e., whether vsync is enabled).
        /// </summary>
        public Boolean SynchronizeWithVerticalRetrace
        {
            get
            {
                Contract.EnsureNotDisposed(this, disposed);

                if (primary == null)
                    throw new InvalidOperationException(FrameworkStrings.NoPrimaryWindow);

                return primary.SynchronizeWithVerticalRetrace;
            }
            set
            {
                Contract.EnsureNotDisposed(this, disposed);

                if (primary == null)
                    throw new InvalidOperationException(FrameworkStrings.NoPrimaryWindow);

                primary.SynchronizeWithVerticalRetrace = value;
            }
        }

        /// <inheritdoc/>
        public Boolean IsActive
        {
            get
            {
                Contract.EnsureNotDisposed(this, disposed);

                lock (stateSyncObject)
                    return !suspended;
            }
        }

        /// <inheritdoc/>
        public Boolean IsSuspended
        {
            get
            {
                Contract.EnsureNotDisposed(this, disposed);

                lock (stateSyncObject)
                    return suspended;
            }
        }

        /// <inheritdoc/>
        public Boolean IsFixedTimeStep
        {
            get
            {
                Contract.EnsureNotDisposed(this, disposed);

                return this.isFixedTimeStep;
            }
            set
            {
                Contract.EnsureNotDisposed(this, disposed);

                this.isFixedTimeStep = value;
                if (timingLogic != null)
                {
                    timingLogic.IsFixedTimeStep = value;
                }
            }
        }

        /// <inheritdoc/>
        public TimeSpan TargetElapsedTime
        {
            get
            {
                Contract.EnsureNotDisposed(this, disposed);

                return this.targetElapsedTime;
            }
            set
            {
                Contract.EnsureNotDisposed(this, disposed);
                Contract.EnsureRange(value.TotalMilliseconds >= 0, nameof(value));

                this.targetElapsedTime = value;
                if (timingLogic != null)
                {
                    timingLogic.TargetElapsedTime = value;
                }
            }
        }

        /// <inheritdoc/>
        public TimeSpan InactiveSleepTime
        {
            get
            {
                Contract.EnsureNotDisposed(this, disposed);

                return this.inactiveSleepTime;
            }
            set
            {
                Contract.EnsureNotDisposed(this, disposed);

                this.inactiveSleepTime = value;
                if (timingLogic != null)
                {
                    timingLogic.InactiveSleepTime = value;
                }
            }
        }

        /// <summary>
        /// Gets the current instance of the <see cref="FrameworkApplication"/> class.
        /// </summary>
        internal static FrameworkApplication Instance
        {
            get { return MSingleton as FrameworkApplication; }
        }

        /// <summary>
        /// Gets or sets the activity's current keyboard type.
        /// </summary>
        internal InputTypes KeyboardInputType
        {
            get { return (InputTypes)MCurrentInputType; }
            set { MCurrentInputType = (Int32)value; }
        }

        /// <summary>
        /// Called when the application is creating its Sedulous context.
        /// </summary>
        /// <returns>The Sedulous context.</returns>
        protected abstract FrameworkContext OnCreatingFrameworkContext();

        /// <inheritdoc/>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            var uv = FrameworkContext.RequestCurrent();
            if (uv != null && !uv.Disposed)
            {
                /*TODO
                var data = uv.Messages.CreateMessageData<AndroidLifecycleMessageData>();
                data.Activity = this;
                uv.Messages.Publish(SedulousMessages.AndroidActivityCreate, data);
                */
            }
            base.OnCreate(savedInstanceState);
        }

        /// <inheritdoc/>
        protected override void OnStart()
        {
            var uv = FrameworkContext.RequestCurrent();
            if (uv != null && !uv.Disposed)
            {
                /*TODO
                var data = uv.Messages.CreateMessageData<AndroidLifecycleMessageData>();
                data.Activity = this;
                uv.Messages.Publish(SedulousMessages.AndroidActivityStart, data);
                */
            }
            base.OnStart();
        }

        /// <inheritdoc/>
        protected override void OnResume()
        {
            var uv = FrameworkContext.RequestCurrent();
            if (uv != null && !uv.Disposed)
            {
                /*TODO
                var data = uv.Messages.CreateMessageData<AndroidLifecycleMessageData>();
                data.Activity = this;
                uv.Messages.Publish(SedulousMessages.AndroidActivityResume, data);
                */
            }
            base.OnResume();
        }

        /// <inheritdoc/>
        protected override void OnPause()
        {
            var uv = FrameworkContext.RequestCurrent();
            if (uv != null && !uv.Disposed)
            {
                /*TODO
                var data = uv.Messages.CreateMessageData<AndroidLifecycleMessageData>();
                data.Activity = this;
                uv.Messages.Publish(SedulousMessages.AndroidActivityPause, data);
                */
            }
            base.OnPause();
        }

        /// <inheritdoc/>
        protected override void OnStop()
        {
            var uv = FrameworkContext.RequestCurrent();
            if (uv != null && !uv.Disposed)
            {
                /* TODO
                var data = uv.Messages.CreateMessageData<AndroidLifecycleMessageData>();
                data.Activity = this;
                uv.Messages.Publish(SedulousMessages.AndroidActivityStop, data);
                */
            }
            base.OnStop();
        }

        /// <inheritdoc/>
        protected override void OnDestroy()
        {
            var uv = FrameworkContext.RequestCurrent();
            if (uv != null && !uv.Disposed)
            {
                /*TODO
                var data = uv.Messages.CreateMessageData<AndroidLifecycleMessageData>();
                data.Activity = this;
                uv.Messages.Publish(SedulousMessages.AndroidActivityDestroy, data);
                */
            }
            base.OnDestroy();
        }

        /// <inheritdoc/>
        protected override void OnRestart()
        {
            var uv = FrameworkContext.RequestCurrent();
            if (uv != null && !uv.Disposed)
            {
                /*TODO
                var data = uv.Messages.CreateMessageData<AndroidLifecycleMessageData>();
                data.Activity = this;
                uv.Messages.Publish(SedulousMessages.AndroidActivityRestart, data);
                */
            }
            base.OnRestart();
        }

        /// <summary>
        /// Releases resources associated with the object.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> if the object is being disposed; <see langword="false"/> if the object is being finalized.</param>
        protected override void Dispose(Boolean disposing)
        {
            lock (stateSyncObject)
            {
                if (!disposed)
                {
                    if (disposing && context != null)
                    {
                        context.Messages.Unsubscribe(this);

                        if (primary != null)
                        {
                            primary.Drawing -= uv_Drawing;
                            primary = null;
                        }

                        context.Dispose();

                        context.Updating -= uv_Updating;
                        context.Shutdown -= uv_Shutdown;
                        context.WindowDrawing -= uv_WindowDrawing;
                        context.WindowDrawn -= uv_WindowDrawn;

                        timingLogic = null;
                    }
                    disposed = true;
                }
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Called when the application is initializing.
        /// </summary>
        protected virtual void OnInitializing()
        {

        }

        /// <summary>
        /// Called after the application has been initialized.
        /// </summary>
        protected virtual void OnInitialized()
        {

        }

        /// <summary>
        /// Called when the application is loading content.
        /// </summary>
        protected virtual void OnLoadingContent()
        {

        }

        /// <summary>
        /// Called when the application state is being updated.
        /// </summary>
        /// <param name="time">Time elapsed since the last call to <see cref="FrameworkContext.Update(FrameworkTime)"/>.</param>
        protected virtual void OnUpdating(FrameworkTime time)
        {

        }

        /// <summary>
        /// Called when the scene is being drawn.
        /// </summary>
        /// <param name="time">Time elapsed since the last call to <see cref="FrameworkContext.Draw(FrameworkTime)"/>.</param>
        protected virtual void OnDrawing(FrameworkTime time)
        {

        }

        /// <summary>
        /// Called when one of the application's windows is about to be drawn.
        /// </summary>
        /// <param name="time">Time elapsed since the last call to <see cref="FrameworkContext.Draw(FrameworkTime)"/>.</param>
        /// <param name="window">The window that is about to be drawn.</param>
        protected virtual void OnWindowDrawing(FrameworkTime time, IFrameworkWindow window)
        {

        }

        /// <summary>
        /// Called after one of the application's windows has been drawn.
        /// </summary>
        /// <param name="time">Time elapsed since the last call to <see cref="FrameworkContext.Draw(FrameworkTime)"/>.</param>
        /// <param name="window">The window that was just drawn.</param>
        protected virtual void OnWindowDrawn(FrameworkTime time, IFrameworkWindow window)
        {

        }

        /// <summary>
        /// Called when the application is about to be suspended.
        /// </summary>
        /// <remarks>When implementing this method, be aware that it can potentially be called
        /// from a thread other than the main Sedulous thread.</remarks>
        protected internal virtual void OnSuspending()
        {
        }

        /// <summary>
        /// Called when the application has been suspended.
        /// </summary>
        /// <remarks>When implementing this method, be aware that it can potentially be called
        /// from a thread other than the main Sedulous thread.</remarks>
        protected internal virtual void OnSuspended()
        {
            SaveSettings();
        }

        /// <summary>
        /// Called when the application is about to be resumed.
        /// </summary>
        /// <remarks>When implementing this method, be aware that it can potentially be called
        /// from a thread other than the main Sedulous thread.</remarks>
        protected internal virtual void OnResuming()
        {

        }

        /// <summary>
        /// Called when the application has been resumed.
        /// </summary>
        /// <remarks>When implementing this method, be aware that it can potentially be called
        /// from a thread other than the main Sedulous thread.</remarks>
        protected internal virtual void OnResumed()
        {

        }

        /// <summary>
        /// Called when the operating system is attempting to reclaim memory.
        /// </summary>
        /// <remarks>When implementing this method, be aware that it can potentially be called
        /// from a thread other than the main Sedulous thread.</remarks>
        protected internal virtual void OnReclaimingMemory()
        {

        }

        /// <summary>
        /// Called when the application is being shut down.
        /// </summary>
        protected virtual void OnShutdown()
        {

        }

        /// <summary>
        /// Occurs when the context receives a message from its queue.
        /// </summary>
        /// <param name="type">The message type.</param>
        /// <param name="data">The message data.</param>
        protected virtual void OnReceivedMessage(FrameworkMessageId type, MessageData data)
        {
            if (type == FrameworkMessages.ApplicationTerminating || type == FrameworkMessages.Quit)
            {
                running = false;
            }
            else if (type == FrameworkMessages.ApplicationSuspending)
            {
                OnSuspending();

                lock (stateSyncObject)
                    suspended = true;
            }
            else if (type == FrameworkMessages.ApplicationSuspended)
            {
                OnSuspended();
            }
            else if (type == FrameworkMessages.ApplicationResuming)
            {
                OnResuming();
            }
            else if (type == FrameworkMessages.ApplicationResumed)
            {
                timingLogic?.ResetElapsed();

                lock (stateSyncObject)
                    suspended = false;

                OnResumed();
            }
            else if (type == FrameworkMessages.LowMemory)
            {
                OnReclaimingMemory();
            }
        }

        /// <summary>
        /// Creates the timing logic for this host process.
        /// </summary>
        protected virtual IFrameworkTimingLogic CreateTimingLogic()
        {
            var timingLogic = new FrameworkTimingLogic(this);
            timingLogic.IsFixedTimeStep = this.IsFixedTimeStep;
            timingLogic.TargetElapsedTime = this.TargetElapsedTime;
            timingLogic.InactiveSleepTime = this.InactiveSleepTime;
            return timingLogic;
        }

        /// <inheritdoc/>
        protected sealed override void OnUltravioletRun()
        {
            Run();

            SafeDispose.DisposeRef(ref context);
            if (finished)
            {
                Finish();
            }
        }

        /// <summary>
        /// Ensures that the assembly which contains the specified type is linked on platforms
        /// which require ahead-of-time compilation.
        /// </summary>
        /// <typeparam name="T">One of the types defined by the assembly to link.</typeparam>
        protected void EnsureAssemblyIsLinked<T>()
        {
            Console.WriteLine("Touching '" + typeof(T).Assembly.FullName + "' to ensure linkage...");
        }

        /// <summary>
        /// Uses a file source which is appropriate to the current platform.
        /// </summary>
        /// <returns><see langword="true"/> if a platform-specific file source was used; otherwise, <see langword="false"/>.</returns>
        protected Boolean UsePlatformSpecificFileSource()
        {
            FileSystemService.Source = new AndroidAssetFileSource(Assets, GetType().Assembly);
            return true;
        }

        /// <summary>
        /// Sets the file system source to an archive file loaded from a manifest resource stream,
        /// if the specified manifest resource exists.
        /// </summary>
        /// <param name="name">The name of the manifest resource being loaded as the file system source.</param>
        /// <returns><see langword="true"/> if the file source was set; otherwise, <see langword="false"/>.</returns>
        protected Boolean SetFileSourceFromManifestIfExists(String name)
        {
            Contract.RequireNotEmpty(name, nameof(name));

            var asm = GetType().Assembly;
            if (asm.GetManifestResourceNames().Contains(name))
            {
                FileSystemService.Source = ContentArchive.FromArchiveFile(() =>
                {
                    return asm.GetManifestResourceStream(name);
                });
                return true;
            }

            return false;
        }

        /// <summary>
        /// Sets the file system source to an archive file loaded from a manifest resource stream.
        /// </summary>
        /// <param name="name">The name of the manifest resource being loaded as the file system source.</param>
        protected void SetFileSourceFromManifest(String name)
        {
            Contract.RequireNotEmpty(name, nameof(name));

            var asm = GetType().Assembly;
            if (!asm.GetManifestResourceNames().Contains(name))
                throw new FileNotFoundException(name);

            FileSystemService.Source = ContentArchive.FromArchiveFile(() =>
            {
                return asm.GetManifestResourceStream(name);
            });
        }

        /// <summary>
        /// Populates the specified Sedulous configuration with the application's initial values.
        /// </summary>
        /// <param name="configuration">The <see cref="FrameworkConfiguration"/> to populate.</param>
        protected void PopulateConfiguration(FrameworkConfiguration configuration)
        {
            Contract.Require(configuration, nameof(configuration));
            PopulateConfigurationFromSettings(configuration);
        }

        /// <summary>
        /// Gets the directory that contains the application's local configuration files.
        /// If the directory does not already exist, it will be created.
        /// </summary>
        /// <returns>The directory that contains the application's local configuration files.</returns>
        protected String GetLocalApplicationSettingsDirectory()
        {
            var path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), DeveloperName, ApplicationName);
            Directory.CreateDirectory(path);
            return path;
        }

        /// <summary>
        /// Gets the directory that contains the application's roaming configuration files.
        /// If the directory does not already exist, it will be created.
        /// </summary>
        /// <returns>The directory that contains the application's roaming configuration files.</returns>
        protected String GetRoamingApplicationSettingsDirectory()
        {
            var path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), DeveloperName, ApplicationName);
            Directory.CreateDirectory(path);
            return path;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the application's internal framework settings
        /// should be preserved between instances.
        /// </summary>
        protected Boolean PreserveApplicationSettings
        {
            get;
            set;
        }

        /// <summary>
        /// Creates the application's Sedulous context.
        /// </summary>
        private void CreateFrameworkContext()
        {
            LoadSettings();

            context = FrameworkContext.EnsureSuccessfulCreation(OnCreatingFrameworkContext);
            if (context == null)
                throw new InvalidOperationException(FrameworkStrings.ContextNotCreated);

            this.created = true;
        }

        private void InitializeFrameworkContext()
        {
            context.ConfigureFactory((factory) =>
            {
                factory.SetFactoryMethod<SurfaceSourceFactory>((stream) => new AndroidSurfaceSource(stream));
                factory.SetFactoryMethod<SurfaceSaverFactory>(() => new AndroidSurfaceSaver());
                factory.SetFactoryMethod<IconLoaderFactory>(() => new AndroidIconLoader());
                factory.SetFactoryMethod<FileSystemServiceFactory>(() => new FileSystemService());
                factory.SetFactoryMethod<ScreenRotationServiceFactory>((display) => new AndroidScreenRotationService(display));
                factory.SetFactoryMethod<ScreenDensityServiceFactory>((display) => new AndroidScreenDensityService(display));
                factory.SetFactoryMethod<AssemblyLoaderServiceFactory>(() => new AndroidAssemblyLoaderService());

                var softwareKeyboardService = new AndroidSoftwareKeyboardService();
                factory.SetFactoryMethod<SoftwareKeyboardServiceFactory>(() => softwareKeyboardService);
            });

            context.Initialize();

            ApplySettings();

            this.timingLogic = CreateTimingLogic();
            if (this.timingLogic == null)
                throw new InvalidOperationException(FrameworkStrings.InvalidTimingLogic);

            this.context.Messages.Subscribe(this,
                FrameworkMessages.ApplicationTerminating,
                FrameworkMessages.ApplicationSuspending,
                FrameworkMessages.ApplicationSuspended,
                FrameworkMessages.ApplicationResuming,
                FrameworkMessages.ApplicationResumed,
                FrameworkMessages.LowMemory,
                FrameworkMessages.Quit);
            this.context.Updating += uv_Updating;
            this.context.Shutdown += uv_Shutdown;
            this.context.WindowDrawing += uv_WindowDrawing;
            this.context.WindowDrawn += uv_WindowDrawn;

            this.context.GetPlatform().Windows.PrimaryWindowChanging += uv_PrimaryWindowChanging;
            this.context.GetPlatform().Windows.PrimaryWindowChanged += uv_PrimaryWindowChanged;
            HookPrimaryWindowEvents();
        }

        /// <summary>
        /// Hooks into the primary window's events.
        /// </summary>
        private void HookPrimaryWindowEvents()
        {
            if (primary != null)
            {
                primary.Drawing -= uv_Drawing;
            }

            primary = context.GetPlatform().Windows.GetPrimary();

            if (primary != null)
            {
                primary.Drawing += uv_Drawing;
            }
        }

        /// <summary>
        /// Loads the application's settings.
        /// </summary>
        private void LoadSettings()
        {
            lock (stateSyncObject)
            {
                if (!PreserveApplicationSettings)
                    return;

                var directory = GetLocalApplicationSettingsDirectory();
                var path = Path.Combine(directory, "SedulousSettings.xml");

                try
                {
                    var settings = FrameworkApplicationSettings.Load(path);
                    if (settings == null)
                        return;

                    this.settings = settings;
                }
                catch (FileNotFoundException) { }
                catch (DirectoryNotFoundException) { }
                catch (XmlException) { }
            }
        }

        /// <summary>
        /// Saves the application's settings.
        /// </summary>
        private void SaveSettings()
        {
            lock (stateSyncObject)
            {
                if (!PreserveApplicationSettings)
                    return;

                var directory = GetLocalApplicationSettingsDirectory();
                var path = Path.Combine(directory, "SedulousSettings.xml");

                this.settings = FrameworkApplicationSettings.FromCurrentSettings(FrameworkContext);
                FrameworkApplicationSettings.Save(path, settings);
            }
        }

        /// <summary>
        /// Applies the application's settings.
        /// </summary>
        private void ApplySettings()
        {
            lock (stateSyncObject)
            {
                if (this.settings == null)
                    return;

                this.settings.Apply(context);
            }
        }

        /// <summary>
        /// Populates the Sedulous configuration from the application settings.
        /// </summary>
        private void PopulateConfigurationFromSettings(FrameworkConfiguration configuration)
        {
        }

        /// <summary>
        /// Handles the Sedulous window manager's PrimaryWindowChanging event.
        /// </summary>
        /// <param name="window">The primary window.</param>
        private void uv_PrimaryWindowChanging(IFrameworkWindow window)
        {
            SaveSettings();
        }

        /// <summary>
        /// Handles the Sedulous window manager's PrimaryWindowChanged event.
        /// </summary>
        /// <param name="window">The primary window.</param>
        private void uv_PrimaryWindowChanged(IFrameworkWindow window)
        {
            HookPrimaryWindowEvents();
        }

        /// <summary>
        /// Handles the Sedulous window's Drawing event.
        /// </summary>
        /// <param name="window">The window being drawn.</param>
        /// <param name="time">Time elapsed since the last call to <see cref="FrameworkContext.Draw(FrameworkTime)"/>.</param>
        private void uv_Drawing(IFrameworkWindow window, FrameworkTime time)
        {
            OnDrawing(time);
        }

        /// <summary>
        /// Handles the Sedulous context's Updating event.
        /// </summary>
        /// <param name="context">The Sedulous context.</param>
        /// <param name="time">Time elapsed since the last call to <see cref="FrameworkContext.Update(FrameworkTime)"/>.</param>
        private void uv_Updating(FrameworkContext context, FrameworkTime time)
        {
            OnUpdating(time);
        }

        /// <summary>
        /// Handles the Sedulous context's Shutdown event.
        /// </summary>
        /// <param name="context">The Sedulous context.</param>
        private void uv_Shutdown(FrameworkContext context)
        {
            OnShutdown();
        }

        /// <summary>
        /// Handles the Sedulous context's <see cref="FrameworkContext.WindowDrawing"/> event.
        /// </summary>
        private void uv_WindowDrawing(FrameworkContext context, FrameworkTime time, IFrameworkWindow window)
        {
            OnWindowDrawing(time, window);
        }

        /// <summary>
        /// Handles the Sedulous context's <see cref="FrameworkContext.WindowDrawn"/> event.
        /// </summary>
        private void uv_WindowDrawn(FrameworkContext context, FrameworkTime time, IFrameworkWindow window)
        {
            OnWindowDrawn(time, window);
        }

        /// <summary>
        /// Writes a warning to the debug output if no file system source has been specified.
        /// </summary>
        private void WarnIfFileSystemSourceIsMissing()
        {
            if (FileSystemService.Source == null)
            {
                System.Diagnostics.Debug.WriteLine("WARNING: No file system source has been specified.");
            }
        }

        // Property values.
        private FrameworkContext context;

        // State values.
        private readonly Object stateSyncObject = new Object();
        private IFrameworkTimingLogic timingLogic;
        private Boolean created;
        private Boolean running;
        private Boolean finished;
        private Boolean suspended;
        private Boolean disposed;
        private IFrameworkWindow primary;

        // The application's tick state.
        private Boolean isFixedTimeStep = FrameworkTimingLogic.DefaultIsFixedTimeStep;
        private TimeSpan targetElapsedTime = FrameworkTimingLogic.DefaultTargetElapsedTime;
        private TimeSpan inactiveSleepTime = FrameworkTimingLogic.DefaultInactiveSleepTime;

        // The application's settings.
        private FrameworkApplicationSettings settings;
    }
}