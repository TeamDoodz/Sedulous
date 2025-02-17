using System;
using System.IO;
using Sample14_LoadingImageDataWithSurfaces.Assets;
using Sample14_LoadingImageDataWithSurfaces.Input;
using Sedulous;
using Sedulous.BASS;
using Sedulous.Content;
using Sedulous.Graphics;
using Sedulous.Graphics.Graphics2D;
using Sedulous.OpenGL;
using Sedulous.SDL2;

namespace Sample14_LoadingImageDataWithSurfaces
{
    public partial class Game : FrameworkApplication
    {
        public Game()
            : base("Sedulous", "Sample 14 - Loading Image Data with Surfaces")
        { }

        protected override FrameworkContext OnCreatingFrameworkContext()
        {
            var configuration = new SDL2FrameworkConfiguration();
            configuration.Plugins.Add(new OpenGLGraphicsPlugin());
            configuration.Plugins.Add(new BASSAudioPlugin());

            return new SDL2FrameworkContext(this, configuration);
        }

        protected override void OnInitialized()
        {
            UsePlatformSpecificFileSource();
            LoadInputBindings();

            base.OnInitialized();
        }

        protected override void OnShutdown()
        {
            SaveInputBindings();

            base.OnShutdown();
        }

        protected override void OnLoadingContent()
        {
            this.content = ContentManager.Create("Content");
            LoadContentManifests();

            this.spriteBatch = SpriteBatch.Create();

            // Texture2D lives in GPU memory, so we can't read data out of it once it's been uploaded to the device.
            // Surface2D lives in CPU memory, so we can directly manipulate it without involving the graphics driver.
            // In this sample, we just load its color data into an array for later use in OnDrawing().
            this.surface = this.content.Load<Surface2D>("Data/Face");
            this.texture = this.surface.CreateTexture();
            this.data = new Color[surface.Width * surface.Height];
            surface.GetData(this.data);

            base.OnLoadingContent();
        }

        protected override void OnUpdating(FrameworkTime time)
        {
            if (FrameworkContext.GetInput().GetActions().ExitApplication.IsPressed())
            {
                Exit();
            }

            base.OnUpdating(time);
        }

        protected override void OnDrawing(FrameworkTime time)
        {
            // We've loaded our surface data into the 'data' array, and we'll use it now to draw an image to the screen.
            // We'll draw each pixel as a string of text representing that pixel's color.

            spriteBatch.Begin(SpriteSortMode.Deferred, null);

            const Int32 CellWidth = 64;
            const Int32 CellHeight = 24;

            var totalWidth = surface.Width * CellWidth;
            var totalHeight = surface.Height * CellHeight;

            var compositor = FrameworkContext.GetPlatform().Windows.GetPrimary().Compositor;
            var origX = (compositor.Width - totalWidth) / 2;
            var origY = (compositor.Height - totalHeight) / 2;

            var cx = origX;
            var cy = origY;

            var font = content.Load<SpriteFont>(GlobalFontID.SegoeUI);

            for (int y = 0; y < surface.Width; y++)
            {
                for (int x = 0; x < surface.Height; x++)
                {
                    var cellColor = data[(y * surface.Width) + x];
                    var cellText = $"{cellColor:x}";
                    var cellPosition = new Vector2(cx, cy);

                    spriteBatch.DrawString(font, cellText, cellPosition, cellColor);

                    cx = cx + CellWidth;
                }
                cx = origX;
                cy = cy + CellHeight;
            }

            spriteBatch.End();

            base.OnDrawing(time);
        }

        protected override void Dispose(Boolean disposing)
        {
            if (disposing)
            {
                if (this.content != null)
                    this.content.Dispose();

                if (this.surface != null)
                    this.surface.Dispose();

                if (this.texture != null)
                    this.texture.Dispose();

                if (this.spriteBatch != null)
                    this.spriteBatch.Dispose();
            }
            base.Dispose(disposing);
        }

        private String GetInputBindingsPath()
        {
            return Path.Combine(GetRoamingApplicationSettingsDirectory(), "InputBindings.xml");
        }

        private void LoadInputBindings()
        {
            FrameworkContext.GetInput().GetActions().Load(GetInputBindingsPath(), throwIfNotFound: false);
        }

        private void SaveInputBindings()
        {
            FrameworkContext.GetInput().GetActions().Save(GetInputBindingsPath());
        }

        private void LoadContentManifests()
        {
            var uvContent = FrameworkContext.GetContent();

            var contentManifestFiles = this.content.GetAssetFilePathsInDirectory("Manifests");
            uvContent.Manifests.Load(contentManifestFiles);

            FrameworkContext.GetContent().Manifests["Global"]["Fonts"].PopulateAssetLibrary(typeof(GlobalFontID));
        }

        // Application resources
        private ContentManager content;
        private SpriteBatch spriteBatch;
        private Surface2D surface;
        private Texture2D texture;
        private Color[] data;
    }
}
