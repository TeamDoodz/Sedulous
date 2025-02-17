﻿using Sandbox3D.Input;
using Sedulous;
using Sedulous.Bass;
using Sedulous.Content;
using Sedulous.Core;
using Sedulous.FreeType2;
using Sedulous.Graphics;
using Sedulous.Graphics.Graphics3D;
using Sedulous.OpenGL;
using Sedulous.Presentation;
using Sedulous.Presentation.Styles;
using Sedulous.Sdl2;
using System;
using System.IO;

namespace Sandbox3D
{
    /// <summary>
    /// Represents the main application object.
    /// </summary>
    public partial class Game : FrameworkApplication
    {
        /// <summary>
        /// Initializes a new instance of the Game 
        /// </summary>
        public Game() : base("Sedulous", "Sandbox3D")
        {
            Diagnostics.DrawDiagnosticsVisuals = true;
            PlatformSpecificInitialization();
        }



        /// <summary>
        /// Called when the application is creating its Sedulous context.
        /// </summary>
        /// <returns>The Sedulous context.</returns>
        protected override FrameworkContext OnCreatingFrameworkContext()
        {
            var graphicsConfig = OpenGLGraphicsConfiguration.Default;
            graphicsConfig.MultiSampleBuffers = 1;
            graphicsConfig.MultiSampleSamples = 8;
            graphicsConfig.SrgbBuffersEnabled = false;
            graphicsConfig.SrgbDefaultForTexture2D = false;

            var contextConfig = new Sdl2FrameworkConfiguration();
            contextConfig.SupportsHighDensityDisplayModes = true;
            contextConfig.EnableServiceMode = false;
            contextConfig.WatchViewFilesForChanges = ShouldDynamicallyReloadContent();
            contextConfig.Plugins.Add(new OpenGLGraphicsPlugin(graphicsConfig));
            contextConfig.Plugins.Add(new BassAudioPlugin());
            contextConfig.Plugins.Add(new FreeTypeFontPlugin());
            PopulateConfiguration(contextConfig);

#if DEBUG
            contextConfig.Debug = true;
            contextConfig.DebugLevels = DebugLevels.Error | DebugLevels.Warning;
            contextConfig.DebugCallback = (uv, level, message) =>
            {
                System.Diagnostics.Debug.WriteLine(message);
            };
#endif

            return new Sdl2FrameworkContext(this, contextConfig);
        }

        /// <summary>
        /// Called after the application has been initialized.
        /// </summary>
        protected override void OnInitialized()
        {
            if (!SetFileSourceFromManifestIfExists("Sandbox3D.Content.uvarc"))
                UsePlatformSpecificFileSource();

            base.OnInitialized();
        }

        /// <summary>
        /// Called when the application is loading content.
        /// </summary>
        protected override void OnLoadingContent()
        {
            ContentManager.GloballySuppressDependencyTracking = !ShouldDynamicallyReloadContent();
            this.content = ContentManager.Create("Content");

            LoadInputBindings();
            LoadContentManifests();


            rasterizerStateSolid = RasterizerState.Create();
            rasterizerStateSolid.CullMode = CullMode.CullClockwiseFace;
            rasterizerStateSolid.FillMode = FillMode.Solid;

            rasterizerStateWireframe = RasterizerState.Create();
            rasterizerStateWireframe.CullMode = CullMode.CullClockwiseFace;
            rasterizerStateWireframe.FillMode = FillMode.Wireframe;

            LoadSkinnedModel();

            GC.Collect(2);

            base.OnLoadingContent();
        }

        /// <summary>
        /// Loads the game's input bindings.
        /// </summary>
        protected void LoadInputBindings()
        {
            var inputBindingsPath = Path.Combine(GetRoamingApplicationSettingsDirectory(), "InputBindings.xml");
            FrameworkContext.GetInput().GetActions().Load(inputBindingsPath, throwIfNotFound: false);
        }

        /// <summary>
        /// Saves the game's input bindings.
        /// </summary>
        protected void SaveInputBindings()
        {
            var inputBindingsPath = Path.Combine(GetRoamingApplicationSettingsDirectory(), "InputBindings.xml");
            FrameworkContext.GetInput().GetActions().Save(inputBindingsPath);
        }

        /// <summary>
        /// Loads the game's content manifest files.
        /// </summary>
        protected void LoadContentManifests()
        {
            var uvContent = FrameworkContext.GetContent();

            var contentManifestFiles = this.content.GetAssetFilePathsInDirectory("Manifests");
            uvContent.Manifests.Load(contentManifestFiles);
        }

        private void LoadSkinnedModel()
        {
            SkinnedMaterial.SharedEffect.EnableStandardLighting();
            this.skinnedModel = this.content.Load<SkinnedModel>("Models/Fox.gltf");
            this.skinnedModelInstance = new SkinnedModelInstance(skinnedModel);

            this.texture = this.content.Load<Texture2D>("Models/Texture.png");

            skinnedModelAnimationTrack = skinnedModelInstance.PlayAnimation(SkinnedAnimationMode.Loop, "Walk");

            skinnedModelSceneRenderer = new SkinnedModelSceneRenderer();

        }


        /// <summary>
        /// Called when the application state is being updated.
        /// </summary>
        /// <param name="time">Time elapsed since the last call to Update.</param>
        protected override void OnUpdating(FrameworkTime time)
        {
            skinnedModelInstance.Update(time);
            if (FrameworkContext.GetInput().GetActions().ExitApplication.IsPressed())
            {
                Exit();
            }
            base.OnUpdating(time);
        }

        /// <summary>
        /// Called when the application's scene is being drawn.
        /// </summary>
        /// <param name="time">Time elapsed since the last call to Draw.</param>
        protected override void OnDrawing(FrameworkTime time)
        {
            if (perspectiveCamera == null)
            {
                perspectiveCamera = PerspectiveCamera.Create();
                perspectiveCamera.Position = new Vector3(0, 100, 200);
                perspectiveCamera.Target = new Vector3(0, 0, 0);
                perspectiveCamera.Update();
            }

            var gfx = FrameworkContext.GetGraphics();
            var window = FrameworkContext.GetPlatform().Windows.GetCurrent();
            var aspectRatio = window.DrawableSize.Width / (Single)window.DrawableSize.Height;

            var world = Matrix.CreateRotationY((float)(2.0 * Math.PI * (time.TotalTime.TotalSeconds / 10.0)));
            var view = Matrix.CreateLookAt(new Vector3(0, 3, 6), new Vector3(0, 0.75f, 0), Vector3.Up);
            var projection = Matrix.CreatePerspectiveFieldOfView((float)Math.PI / 4f, aspectRatio, 1f, 1000f);

            SkinnedMaterial.SharedEffect.World = world;
            SkinnedMaterial.SharedEffect.View = view;
            SkinnedMaterial.SharedEffect.Projection = projection;

            var worldMatrix = world;// Matrix.CreateRotationY(Radians.FromDegrees(90));
            bool renderWireFrame = false;
            gfx.SetRasterizerState(renderWireFrame ? rasterizerStateWireframe : rasterizerStateSolid);
            gfx.SetDepthStencilState(DepthStencilState.Default);

            skinnedModelSceneRenderer.Draw(skinnedModelInstance.Scenes.DefaultScene, perspectiveCamera, ref worldMatrix);

            base.OnDrawing(time);
        }

        /// <summary>
        /// Called when the application is being shut down.
        /// </summary>
        protected override void OnShutdown()
        {
            SaveInputBindings();

            base.OnShutdown();
        }

        /// <summary>
        /// Releases resources associated with the object.
        /// </summary>
        /// <param name="disposing">true if the object is being disposed; false if the object is being finalized.</param>
        protected override void Dispose(Boolean disposing)
        {
            if (disposing)
            {
                SafeDispose.DisposeRef(ref content);
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Performs any platform-specific initialization.
        /// </summary>
        partial void PlatformSpecificInitialization();

        /// <summary>
        /// Gets a value indicating whether the game should enable dynamic reloading of content assets.
        /// </summary>
        private Boolean ShouldDynamicallyReloadContent()
        {
#if DEBUG
            return true;
#else
            return false;
#endif
        }

        // The global content manager.  Manages any content that should remain loaded for the duration of the game's execution.
        private ContentManager content;

        // 3D geometry testing.

        private PerspectiveCamera perspectiveCamera;

        private SkinnedModelSceneRenderer skinnedModelSceneRenderer;
        private SkinnedModel skinnedModel;
        private SkinnedModelInstance skinnedModelInstance;
        private SkinnedAnimationTrack skinnedModelAnimationTrack;
        private Texture2D texture;

        private RasterizerState rasterizerStateSolid;
        private RasterizerState rasterizerStateWireframe;
    }
}
