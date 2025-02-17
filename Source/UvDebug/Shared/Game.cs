﻿using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Sedulous;
using Sedulous.Audio;
using Sedulous.Bass;
using Sedulous.Content;
using Sedulous.Core;
using Sedulous.Core.Text;
using Sedulous.Fmod;
using Sedulous.FreeType2;
using Sedulous.Graphics;
using Sedulous.OpenGL;
using Sedulous.OpenGL.Bindings;
using Sedulous.Platform;
using Sedulous.Presentation;
using Sedulous.Presentation.Styles;
using Sedulous.Sdl2;
using UvDebug.Input;
using UvDebug.UI;

namespace UvDebug
{
    public static class GlobalSongID
    {
        public static AssetId DeepHaze;
        public static AssetId Sample;
    }

    /// <summary>
    /// Represents the main application object.
    /// </summary>
    public partial class Game : FrameworkApplication
    {
        /// <summary>
        /// Initializes a new instance of the Game 
        /// </summary>
        public Game() : base("Sedulous", "UvDebug")
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
            contextConfig.EnableServiceMode = ShouldRunInServiceMode();
            contextConfig.WatchViewFilesForChanges = ShouldDynamicallyReloadContent();
            contextConfig.Plugins.Add(new OpenGLGraphicsPlugin(graphicsConfig));
            //contextConfig.Plugins.Add(new BASSAudioPlugin());
            contextConfig.Plugins.Add(new FmodAudioPlugin());
            contextConfig.Plugins.Add(new FreeTypeFontPlugin());
            contextConfig.Plugins.Add(new PresentationFoundationPlugin());
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
            if (!SetFileSourceFromManifestIfExists("UvDebug.Content.uvarc"))
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

            if (FrameworkContext.IsRunningInServiceMode)
            {
                LoadPresentation();
                CompileContent();
                CompileBindingExpressions();
                Environment.Exit(0);
            }
            else
            {
                LoadLocalizationPlugins();
                LoadLocalizationDatabases();
                LoadInputBindings();
                LoadContentManifests();
                LoadPresentation();
                LoadTestGeometry();


                this.song = this.content.Load<Song>(GlobalSongID.Sample);
                this.songPlayer = SongPlayer.Create();
                this.songPlayer.Play(this.song);

                this.screenService = new UIScreenService(content);

                GC.Collect(2);
            }
            
            base.OnLoadingContent();
        }

        /// <summary>
        /// Loads the application's localization plugins.
        /// </summary>
        protected void LoadLocalizationPlugins()
        {
            var fss = FileSystemService.Create();
            var plugins = content.GetAssetFilePathsInDirectory(Path.Combine("Localization", "Plugins"), "*.dll");
            foreach (var plugin in plugins)
            {
                try
                {
                    var asm = Assembly.Load(plugin);
                    Localization.LoadPlugins(asm);
                }
                catch (Exception e) when (e is BadImageFormatException || e is FileLoadException) { }
            }
        }

        /// <summary>
        /// Loads the application's localization databases.
        /// </summary>
        protected void LoadLocalizationDatabases()
        {
            var fss = FileSystemService.Create();
            var databases = content.GetAssetFilePathsInDirectory("Localization", "*.xml");
            foreach (var database in databases)
            {
                using (var stream = fss.OpenRead(database))
                {
                    Localization.Strings.LoadFromStream(stream);
                }
            }
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

            FrameworkContext.GetContent().Manifests["Global"]["Songs"].PopulateAssetLibrary(typeof(GlobalSongID));
        }

        /// <summary>
        /// Loads files necessary for the Presentation Foundation.
        /// </summary>
        protected void LoadPresentation()
        {
#if !IMGUI
            var upf = FrameworkContext.GetUI().GetPresentationFoundation();
            upf.RegisterKnownTypes(GetType().Assembly);

            if (!ShouldRunInServiceMode())
            {
                globalStyleSheet = GlobalStyleSheet.Create();
                globalStyleSheet.Append(content, "UI/DefaultUIStyles");
                globalStyleSheet.Append(content, "UI/GameStyles");
                upf.SetGlobalStyleSheet(globalStyleSheet);

                CompileBindingExpressions();
                upf.LoadCompiledExpressions();
            }
#endif
        }

        /// <summary>
        /// Loads 3D geometry used for testing.
        /// </summary>
        protected void LoadTestGeometry()
        {
            /*
            vertexBuffer = VertexBuffer.Create<VertexPositionColorTexture>(5);
            vertexBuffer.SetData(new VertexPositionColorTexture[]
            {
                new VertexPositionColorTexture { Position = new Vector3(-1f,   0f, -1f), Color = Color.Red, TextureCoordinate = new Vector2(1, 0) },
                new VertexPositionColorTexture { Position = new Vector3( 1f,   0f, -1f), Color = Color.Lime, TextureCoordinate = new Vector2(0, 1) },
                new VertexPositionColorTexture { Position = new Vector3( 1f,   0f,  1f), Color = Color.Blue },
                new VertexPositionColorTexture { Position = new Vector3(-1f,   0f,  1f), Color = Color.Yellow },
                new VertexPositionColorTexture { Position = new Vector3( 0f, 1.5f,  0f), Color = Color.Magenta, TextureCoordinate = new Vector2(0, 0) },
            });
            */

            vertexBuffer = VertexBuffer.Create<VertexPositionNormalTexture>(36);
            vertexBuffer.SetData(new VertexPositionNormalTexture[]
            {
                new VertexPositionNormalTexture { Position = new Vector3( 1f, 0f,  1f), Normal = new Vector3(0, -1f, 0) },
                new VertexPositionNormalTexture { Position = new Vector3( 1f, 0f, -1f), Normal = new Vector3(0, -1f, 0) },
                new VertexPositionNormalTexture { Position = new Vector3(-1f, 0f, -1f), Normal = new Vector3(0, -1f, 0) },

                new VertexPositionNormalTexture { Position = new Vector3(-1f, 0f, -1f), Normal = new Vector3(0, -1f, 0) },
                new VertexPositionNormalTexture { Position = new Vector3(-1f, 0f,  1f), Normal = new Vector3(0, -1f, 0) },
                new VertexPositionNormalTexture { Position = new Vector3( 1f, 0f,  1f), Normal = new Vector3(0, -1f, 0) },

                new VertexPositionNormalTexture { Position = new Vector3( 1f, 2f, -1f), Normal = new Vector3(0, 0f, -1f) },
                new VertexPositionNormalTexture { Position = new Vector3(-1f, 2f, -1f), Normal = new Vector3(0, 0f, -1f) },
                new VertexPositionNormalTexture { Position = new Vector3(-1f, 0f, -1f), Normal = new Vector3(0, 0f, -1f) },

                new VertexPositionNormalTexture { Position = new Vector3(-1f, 0f, -1f), Normal = new Vector3(0, 0f, -1f) },
                new VertexPositionNormalTexture { Position = new Vector3( 1f, 0f, -1f), Normal = new Vector3(0, 0f, -1f) },
                new VertexPositionNormalTexture { Position = new Vector3( 1f, 2f, -1f), Normal = new Vector3(0, 0f, -1f) },

                new VertexPositionNormalTexture { Position = new Vector3( 1f, 0f, -1f), Normal = new Vector3(1f, 0f, 0f) },
                new VertexPositionNormalTexture { Position = new Vector3( 1f, 2f,  1f), Normal = new Vector3(1f, 0f, 0f) },
                new VertexPositionNormalTexture { Position = new Vector3( 1f, 2f, -1f), Normal = new Vector3(1f, 0f, 0f) },

                new VertexPositionNormalTexture { Position = new Vector3( 1f, 0f,  1f), Normal = new Vector3(1f, 0f, 0f) },
                new VertexPositionNormalTexture { Position = new Vector3( 1f, 2f,  1f), Normal = new Vector3(1f, 0f, 0f) },
                new VertexPositionNormalTexture { Position = new Vector3( 1f, 0f, -1f), Normal = new Vector3(1f, 0f, 0f) },

                new VertexPositionNormalTexture { Position = new Vector3(-1f, 0f,  1f), Normal = new Vector3(0, 0f, 1f) },
                new VertexPositionNormalTexture { Position = new Vector3(-1f, 2f,  1f), Normal = new Vector3(0, 0f, 1f) },
                new VertexPositionNormalTexture { Position = new Vector3( 1f, 2f,  1f), Normal = new Vector3(0, 0f, 1f) },

                new VertexPositionNormalTexture { Position = new Vector3( 1f, 2f,  1f), Normal = new Vector3(0, 0f, 1f) },
                new VertexPositionNormalTexture { Position = new Vector3( 1f, 0f,  1f), Normal = new Vector3(0, 0f, 1f) },
                new VertexPositionNormalTexture { Position = new Vector3(-1f, 0f,  1f), Normal = new Vector3(0, 0f, 1f) },

                new VertexPositionNormalTexture { Position = new Vector3(-1f, 2f, -1f), Normal = new Vector3(-1f, 0f, 0f) },
                new VertexPositionNormalTexture { Position = new Vector3(-1f, 2f,  1f), Normal = new Vector3(-1f, 0f, 0f) },
                new VertexPositionNormalTexture { Position = new Vector3(-1f, 0f, -1f), Normal = new Vector3(-1f, 0f, 0f) },

                new VertexPositionNormalTexture { Position = new Vector3(-1f, 0f, -1f), Normal = new Vector3(-1f, 0f, 0f) },
                new VertexPositionNormalTexture { Position = new Vector3(-1f, 2f,  1f), Normal = new Vector3(-1f, 0f, 0f) },
                new VertexPositionNormalTexture { Position = new Vector3(-1f, 0f,  1f), Normal = new Vector3(-1f, 0f, 0f) },

                new VertexPositionNormalTexture { Position = new Vector3(-1f, 2f, -1f), Normal = new Vector3(0, 1f, 0) },
                new VertexPositionNormalTexture { Position = new Vector3( 1f, 2f, -1f), Normal = new Vector3(0, 1f, 0) },
                new VertexPositionNormalTexture { Position = new Vector3( 1f, 2f,  1f), Normal = new Vector3(0, 1f, 0) },

                new VertexPositionNormalTexture { Position = new Vector3( 1f, 2f,  1f), Normal = new Vector3(0, 1f, 0) },
                new VertexPositionNormalTexture { Position = new Vector3(-1f, 2f,  1f), Normal = new Vector3(0, 1f, 0) },
                new VertexPositionNormalTexture { Position = new Vector3(-1f, 2f, -1f), Normal = new Vector3(0, 1f, 0) },
            });

            //indexBuffer = IndexBuffer.Create(IndexBufferElementType.Int16, 18);
            //indexBuffer.SetData(new Int16[] { 2, 1, 0, 0, 3, 2, 0, 1, 4, 1, 2, 4, 2, 3, 4, 3, 0, 4 });

            geometryStream = GeometryStream.Create();
            geometryStream.Attach(vertexBuffer);
            //geometryStream.Attach(indexBuffer);

            effect = BasicEffect.Create();
            effect.EnableStandardLighting();

            rasterizerStateSolid = RasterizerState.Create();
            rasterizerStateSolid.CullMode = CullMode.CullCounterClockwiseFace;
            rasterizerStateSolid.FillMode = FillMode.Solid;

            rasterizerStateWireframe = RasterizerState.Create();
            rasterizerStateWireframe.CullMode = CullMode.CullCounterClockwiseFace;
            rasterizerStateWireframe.FillMode = FillMode.Wireframe;

            texture = content.Load<Texture2D>(@"Textures\Triangle");
        }

        /// <summary>
        /// Called when the application state is being updated.
        /// </summary>
        /// <param name="time">Time elapsed since the last call to Update.</param>
        protected override void OnUpdating(FrameworkTime time)
        {
            songPlayer.Update(time);
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
            var gfx = FrameworkContext.GetGraphics();
            var window = FrameworkContext.GetPlatform().Windows.GetCurrent();
            var aspectRatio = window.DrawableSize.Width / (Single)window.DrawableSize.Height;

            effect.World = Matrix.CreateRotationY((float)(2.0 * Math.PI * (time.TotalTime.TotalSeconds / 10.0)));
            effect.View = Matrix.CreateLookAt(new Vector3(0, 3, 6), new Vector3(0, 0.75f, 0), Vector3.Up);
            effect.Projection = Matrix.CreatePerspectiveFieldOfView((float)Math.PI / 4f, aspectRatio, 1f, 1000f);

            gfx.SetGeometryStream(geometryStream);

            void DrawGeometry(RasterizerState rasterizerState, DepthStencilState depthStencilState)
            {
                foreach (var pass in this.effect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    gfx.SetRasterizerState(rasterizerState);
                    gfx.SetDepthStencilState(depthStencilState);
                    gfx.DrawPrimitives(PrimitiveType.TriangleList, 0, vertexBuffer.VertexCount / 3);
                    //gfx.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, indexBuffer.IndexCount / 3);
                }
            }

            effect.FogColor = Color.Red;
            effect.PreferPerPixelLighting = true;
            effect.LightingEnabled = true;
            effect.SrgbColor = false;
            effect.VertexColorEnabled = false;
            effect.DiffuseColor = Color.White;
            effect.TextureEnabled = false;
            effect.Texture = texture;
            DrawGeometry(rasterizerStateSolid, DepthStencilState.Default);

            if (!GL.IsGLES)
            {
                effect.LightingEnabled = false;
                effect.FogEnabled = false;
                effect.VertexColorEnabled = false;
                effect.DiffuseColor = Color.Black;
                effect.TextureEnabled = false;
                DrawGeometry(rasterizerStateWireframe, DepthStencilState.None);
            }
            
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
                SafeDispose.DisposeRef(ref screenService);
                SafeDispose.DisposeRef(ref globalStyleSheet);
                SafeDispose.DisposeRef(ref content);

                if (this.songPlayer != null)
                    this.songPlayer.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Performs any platform-specific initialization.
        /// </summary>
        partial void PlatformSpecificInitialization();

        /// <summary>
        /// Gets a value indicating whether the game should run in service mode.
        /// </summary>
        /// <returns><see langword="true"/> if the game should run in service mode; otherwise, <see langword="false"/>.</returns>
        private Boolean ShouldRunInServiceMode()
        {
            return compileContent || compileExpressions;
        }

        /// <summary>
        /// Gets a value indicating whether the game should compile its content into an archive.
        /// </summary>
        /// <returns></returns>
        private Boolean ShouldCompileContent()
        {
            return compileContent;
        }

        /// <summary>
        /// Gets a value indicating whether the game should compile binding expressions.
        /// </summary>
        /// <returns><see langword="true"/> if the game should compile binding expressions; otherwise, <see langword="false"/>.</returns>
        private Boolean ShouldCompileBindingExpressions()
        {
#if DEBUG
            return true;
#else
            return compileExpressions || System.Diagnostics.Debugger.IsAttached;
#endif
        }

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

        /// <summary>
        /// Compiles the game's content into an archive file.
        /// </summary>
        private void CompileContent()
        {
            if (ShouldCompileContent())
            {
                if (FrameworkContext.Platform == FrameworkPlatform.Android || FrameworkContext.Platform == FrameworkPlatform.iOS)
                    throw new NotSupportedException();

                var archive = ContentArchive.FromFileSystem(new[] { "Content" });
                using (var stream = File.OpenWrite("Content.uvarc"))
                {
                    using (var writer = new BinaryWriter(stream))
                    {
                        archive.Save(writer);
                    }
                }
            }
        }

        /// <summary>
        /// Compiles the game's binding expressions.
        /// </summary>
        private void CompileBindingExpressions()
        {
#if !IMGUI
            if (ShouldCompileBindingExpressions())
            {
                var upf = FrameworkContext.GetUI().GetPresentationFoundation();

                var flags = CompileExpressionsFlags.None;

                if (resolveContent)
                    flags |= CompileExpressionsFlags.ResolveContentFiles;

                if (compileExpressions)
                    flags |= CompileExpressionsFlags.IgnoreCache;

                var sw = System.Diagnostics.Stopwatch.StartNew();
                upf.CompileExpressionsIfSupported("Content", flags);
                sw.Stop();
                Console.WriteLine(sw.Elapsed);
            }
#endif
        }        
        
        // The global content manager.  Manages any content that should remain loaded for the duration of the game's execution.
        private ContentManager content;

        // State values.
        private GlobalStyleSheet globalStyleSheet;
        private UIScreenService screenService;

        internal Boolean resolveContent;
        internal Boolean compileContent;
        internal Boolean compileExpressions;

        // 3D geometry testing.
        private GeometryStream geometryStream;
        private VertexBuffer vertexBuffer;
        private BasicEffect effect;
        private RasterizerState rasterizerStateSolid;
        private RasterizerState rasterizerStateWireframe;
        private Texture2D texture;

        // Audio
        private Song song;
        private SongPlayer songPlayer;
    }
}
