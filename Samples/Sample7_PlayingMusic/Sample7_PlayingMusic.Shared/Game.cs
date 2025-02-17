using System;
using System.IO;
using System.Text;
using Sample7_PlayingMusic.Assets;
using Sample7_PlayingMusic.Input;
using Sedulous;
using Sedulous.Audio;
using Sedulous.BASS;
using Sedulous.Content;
using Sedulous.Core.Text;
using Sedulous.Graphics;
using Sedulous.Graphics.Graphics2D;
using Sedulous.Graphics.Graphics2D.Text;
using Sedulous.OpenGL;
using Sedulous.SDL2;

namespace Sample7_PlayingMusic
{
    public partial class Game : FrameworkApplication
    {
        public Game()
            : base("Sedulous", "Sample 7 - Playing Music")
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

            this.spriteFont = this.content.Load<SpriteFont>(GlobalFontID.SegoeUI);
            this.spriteBatch = SpriteBatch.Create();
            this.stringBuffer = new StringBuilder();
            this.stringFormatter = new StringFormatter();
            this.textRenderer = new TextRenderer();
            this.textLayoutCommands = new TextLayoutCommandStream();

            this.song = this.content.Load<Song>(GlobalSongID.DeepHaze);
            this.songPlayer = SongPlayer.Create();
            this.songPlayer.Play(this.song);

            base.OnLoadingContent();
        }

        protected override void OnUpdating(FrameworkTime time)
        {
            songPlayer.Update(time);

            if (FrameworkContext.GetInput().GetActions().ExitApplication.IsPressed())
            {
                Exit();
            }

            base.OnUpdating(time);
        }

        protected override void OnDrawing(FrameworkTime time)
        {
            var window = FrameworkContext.GetPlatform().Windows.GetPrimary();
            var width = window.DrawableSize.Width;
            var height = window.DrawableSize.Height;

            stringFormatter.Reset();
            stringFormatter.AddArgument(songPlayer.Position.Minutes);
            stringFormatter.AddArgument(songPlayer.Position.Seconds);
            stringFormatter.AddArgument(songPlayer.Duration.Minutes);
            stringFormatter.AddArgument(songPlayer.Duration.Seconds);
            stringFormatter.Format("{0:pad:2}:{1:pad:2} / {2:pad:2}:{3:pad:2}", stringBuffer);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            var attribution =
                "|c:FFFFFF00|Now Playing|c|\n\n" +
                "\"|c:FFFFFF00|Deep Haze|c|\" by Kevin MacLeod (incompetech.com)\n" +
                "Licensed under Creative Commons: By Attribution 3.0\n" +
                "|c:FF808080|http://creativecommons.org/licenses/by/3.0/|c|\n\n\n";
            var settings = new TextLayoutSettings(spriteFont, width, height, TextFlags.AlignMiddle | TextFlags.AlignCenter);
            textRenderer.CalculateLayout(attribution, textLayoutCommands, settings);
            textRenderer.Draw(spriteBatch, textLayoutCommands, Vector2.Zero, Color.White);

            var timerSize = spriteFont.Regular.MeasureString(stringBuffer);
            var timerPosition = new Vector2(
                (Int32)(textLayoutCommands.Bounds.Left + ((textLayoutCommands.Bounds.Width - timerSize.Width) / 2f)),
                (Int32)(textLayoutCommands.Bounds.Bottom - timerSize.Height));
            spriteBatch.DrawString(spriteFont.Regular, stringBuffer, timerPosition, Color.White);

            spriteBatch.End();

            base.OnDrawing(time);
        }

        protected override void Dispose(Boolean disposing)
        {
            if (disposing)
            {
                if (this.content != null)
                    this.content.Dispose();

                if (this.spriteBatch != null)
                    this.spriteBatch.Dispose();

                if (this.songPlayer != null)
                    this.songPlayer.Dispose();
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
            FrameworkContext.GetContent().Manifests["Global"]["Songs"].PopulateAssetLibrary(typeof(GlobalSongID));
        }

        private ContentManager content;
        private SpriteFont spriteFont;
        private SpriteBatch spriteBatch;
        private StringBuilder stringBuffer;
        private StringFormatter stringFormatter;
        private TextRenderer textRenderer;
        private TextLayoutCommandStream textLayoutCommands;
        private Song song;
        private SongPlayer songPlayer;
    }
}
