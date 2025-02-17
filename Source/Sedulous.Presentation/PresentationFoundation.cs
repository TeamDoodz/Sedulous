﻿using System;
using System.Runtime.CompilerServices;
using Sedulous.Core;
using Sedulous.Presentation.Animations;
using Sedulous.Presentation.Input;

namespace Sedulous.Presentation
{
    /// <summary>
    /// Represents the state of the Sedulous Presentation Foundation.
    /// </summary>
    public sealed partial class PresentationFoundation : FrameworkResource
    {
        /// <summary>
        /// Initializes the <see cref="PresentationFoundation"/> type.
        /// </summary>
        static PresentationFoundation() => CommandManager.RegisterValueResolvers();
        
        /// <summary>
        /// Initializes a new instance of the <see cref="PresentationFoundation"/> class.
        /// </summary>
        private PresentationFoundation(FrameworkContext context)
            : base(context)
        {
            RuntimeHelpers.RunClassConstructor(typeof(Tweening).TypeHandle);
            RuntimeHelpers.RunClassConstructor(typeof(SimpleClockPool).TypeHandle);
            RuntimeHelpers.RunClassConstructor(typeof(StoryboardClockPool).TypeHandle);

            RegisterCoreTypes();

            this.outOfBandRenderer = context.IsRunningInServiceMode ? null : new OutOfBandRenderer(context);

            this.styleQueue = new LayoutQueue(InvalidateStyle, false);
            this.measureQueue = new LayoutQueue(InvalidateMeasure);
            this.arrangeQueue = new LayoutQueue(InvalidateArrange);          
        }

        /// <summary>
        /// Modifies the specified <see cref="FrameworkConfiguration"/> instance so that the Sedulous
        /// Presentation Foundation will be registered as the context's view provider.
        /// </summary>
        /// <param name="frameworkConfig">The <see cref="FrameworkConfiguration"/> instance to modify.</param>
        /// <param name="presentationConfig">Configuration settings for the Sedulous Presentation Foundation.</param>
        public static void Configure(FrameworkConfiguration frameworkConfig, PresentationFoundationConfiguration presentationConfig = null)
        {
            Contract.Require(frameworkConfig, nameof(frameworkConfig));

            frameworkConfig.ViewProviderConfiguration = presentationConfig;
        }
        
        /// <summary>
        /// Gets the singleton instance of the Presentation Foundation.
        /// </summary>
        internal static PresentationFoundation Instance => instance;

        /// <summary>
        /// Gets the identifier of the current digest cycle.
        /// </summary>
        internal Int64 DigestCycleID => digestCycleID;

        /// <summary>
        /// Gets the identifier of the last digest cycle during which a layout occurred.
        /// </summary>
        internal Int64 DigestCycleIDOfLastLayout => digestCycleIDOfLastLayout;

        /// <summary>
        /// Gets the renderer which is used to draw elements out-of-band.
        /// </summary>
        internal OutOfBandRenderer OutOfBandRenderer => outOfBandRenderer;

        /// <inheritdoc/>
        protected override void Dispose(Boolean disposing)
        {
            if (disposing)
            {
                SafeDispose.Dispose(outOfBandRenderer);
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Called when the Sedulous context blah blah blah
        /// </summary>
        /// <param name="context"></param>
        private void OnFrameStart(FrameworkContext context)
        {
            PerformanceStats.OnFrameStart();
        }

        /// <summary>
        /// Called when the Sedulous context is about to update its subsystems.
        /// </summary>
        /// <param name="context">The Sedulous context.</param>
        /// <param name="time">Time elapsed since the last call to <see cref="FrameworkContext.Update(FrameworkTime)"/>.</param>
        private void OnUpdatingSubsystems(FrameworkContext context, FrameworkTime time)
        {
            digestCycleID++;
        }

        /// <summary>
        /// Called when the Sedulous UI subsystem is being updated.
        /// </summary>
        /// <param name="subsystem">The Sedulous subsystem.</param>
        /// <param name="time">Time elapsed since the last call to <see cref="FrameworkContext.Update(FrameworkTime)"/>.</param>
        private void OnUpdatingUI(IFrameworkSubsystem subsystem, FrameworkTime time)
        {
            PerformanceStats.BeginUpdate();

            PerformLayout();

            if (OutOfBandRenderer != null)
                OutOfBandRenderer.Update();

            PerformanceStats.EndUpdate();
        }

        /// <summary>
        /// Called when the Sedulous context is about to draw a frame.
        /// </summary>
        /// <param name="context">The Sedulous context.</param>
        /// <param name="time">Time elapsed since the last call to <see cref="FrameworkContext.Draw(FrameworkTime)"/>.</param>
        private void OnDrawing(FrameworkContext context, FrameworkTime time)
        {
            if (OutOfBandRenderer != null)
                OutOfBandRenderer.DrawRenderTargets(time);
        }
        
        // The singleton instance of the Sedulous Presentation Foundation.
        private static readonly FrameworkSingleton<PresentationFoundation> instance =
            new FrameworkSingleton<PresentationFoundation>(uv =>
            {
                var instance = new PresentationFoundation(uv);
                uv.FrameStart += instance.OnFrameStart;
                uv.UpdatingSubsystems += instance.OnUpdatingSubsystems;
                uv.GetUI().Updating += instance.OnUpdatingUI;
                uv.Drawing += instance.OnDrawing;
                return instance;
            });
        
        // The out-of-band element renderer.
        private readonly OutOfBandRenderer outOfBandRenderer;

        // The identifier of the current digest cycle.
        private Int64 digestCycleID = 1;
        private Int64 digestCycleIDOfLastLayout;
    }
}
