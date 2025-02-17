﻿using System.Drawing;
using NUnit.Framework;
using Sedulous.Graphics;
using Sedulous.Graphics.Graphics2D;
using Sedulous.Platform;
using Sedulous.TestApplication;

namespace Sedulous.Tests.Graphics
{
    [TestFixture]
    public class Surface2DTests : FrameworkApplicationTestFramework
    {
        [Test]
        [Category("Rendering")]
        [Description("Ensures that Surface2D produces the correct texture data when the surface is flipped.")]
        public void Surface2D_RendersCorrectly_WhenFlipped()
        {
            var sbatch = default(SpriteBatch);
            var surface = default(Surface2D);
            var texture0 = default(Texture2D);
            var texture1 = default(Texture2D);
            var texture2 = default(Texture2D);
            var texture3 = default(Texture2D);

            var result = GivenAFrameworkApplication()
                .WithContent(content =>
                {
                    sbatch = SpriteBatch.Create();
                    surface = content.Load<Surface2D>("Surfaces/Test");

                    surface.ProcessAlpha(true, null);
                    TheResultingValue(surface.IsFlippedHorizontally).ShouldBe(false);
                    TheResultingValue(surface.IsFlippedVertically).ShouldBe(false);
                    //texture0 = surface.CreateTexture();
                    texture0 = Texture2D.CreateFromSurface2D(surface);

                    surface.Flip(SurfaceFlipDirection.Horizontal);
                    TheResultingValue(surface.IsFlippedHorizontally).ShouldBe(true);
                    TheResultingValue(surface.IsFlippedVertically).ShouldBe(false);
                    //texture1 = surface.CreateTexture();
                    texture1 = Texture2D.CreateFromSurface2D(surface);

                    surface.Flip(SurfaceFlipDirection.Vertical);
                    TheResultingValue(surface.IsFlippedHorizontally).ShouldBe(true);
                    TheResultingValue(surface.IsFlippedVertically).ShouldBe(true);
                    //texture2 = surface.CreateTexture();
                    texture2 = Texture2D.CreateFromSurface2D(surface);

                    surface.Flip(SurfaceFlipDirection.Horizontal);
                    TheResultingValue(surface.IsFlippedHorizontally).ShouldBe(false);
                    TheResultingValue(surface.IsFlippedVertically).ShouldBe(true);
                    //texture3 = surface.CreateTexture();
                    texture3 = Texture2D.CreateFromSurface2D(surface);
                })
                .Render(uv =>
                {
                    uv.GetGraphics().Clear(Color.CornflowerBlue);

                    sbatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque);

                    sbatch.Draw(texture0, new RectangleF(0, 0, 64, 64), Color.White);
                    sbatch.Draw(texture1, new RectangleF(64, 0, 64, 64), Color.White);
                    sbatch.Draw(texture2, new RectangleF(64, 64, 64, 64), Color.White);
                    sbatch.Draw(texture3, new RectangleF(0, 64, 64, 64), Color.White);

                    sbatch.End();
                });

            TheResultingImage(result)
                .ShouldMatch(@"Resources/Expected/Graphics/Surface2D_RendersCorrectly_WhenFlipped.png");
        }

        [Test]
        [Category("Rendering")]
        [Description("Ensures that Surface2D produces the correct texture data when the surface is flipped during a blit operation.")]
        public void Surface2D_RendersCorrectly_WhenFlippedDuringBlit()
        {
            var sbatch = default(SpriteBatch);
            var surfaceOutput = default(Surface2D);
            var surfaceInput = default(Surface2D);
            var texture = default(Texture2D);

            var result = GivenAFrameworkApplication()
                .WithContent(content =>
                {
                    sbatch = SpriteBatch.Create();

                    surfaceOutput = Surface2D.Create(128, 128);
                    surfaceOutput.Clear(Color.Lime);

                    surfaceInput = content.Load<Surface2D>("Surfaces/Test");
                    surfaceInput.ProcessAlpha(true, null);
                    surfaceInput.Blit(surfaceOutput, new Point(0, 0), SurfaceFlipDirection.None);
                    surfaceInput.Blit(surfaceOutput, new Point(64, 0), SurfaceFlipDirection.Horizontal);
                    surfaceInput.Blit(surfaceOutput, new Point(0, 64), SurfaceFlipDirection.Vertical);

                    //texture = surfaceOutput.CreateTexture();
                    texture = Texture2D.CreateFromSurface2D(surfaceOutput);
                })
                .Render(uv =>
                {
                    uv.GetGraphics().Clear(Color.CornflowerBlue);

                    sbatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

                    sbatch.Draw(texture, new RectangleF(0, 0, 128, 128), Color.White);

                    sbatch.End();
                });

            TheResultingImage(result)
                .ShouldMatch(@"Resources/Expected/Graphics/Surface2D_RendersCorrectly_WhenFlippedDuringBlit.png");
        }
    }
}
