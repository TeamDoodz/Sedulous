using System;
using Newtonsoft.Json;
using NUnit.Framework;
using Sedulous.TestFramework;

namespace Sedulous.Tests
{
    [TestFixture]
    public class ColorTests : FrameworkTestFramework
    {
        [Test]
        public void Color_IsConstructedProperly_FromPackedARGB()
        {
            var result = Color.FromArgb(0xFFABCDEF);

            TheResultingValue(result)
                .ShouldHaveArgbComponents(255, 171, 205, 239);
        }

        [Test]
        public void Color_IsConstructedProperly_FromSingleRGB()
        {
            var result = new Color(0.1f, 0.2f, 0.3f);

            TheResultingValue(result)
                .ShouldHaveArgbComponents(255, 25, 51, 76);
        }

        [Test]
        public void Color_IsConstructedProperly_FromSingleRGBA()
        {
            var result = new Color(0.1f, 0.2f, 0.3f, 0.4f);

            TheResultingValue(result)
                .ShouldHaveArgbComponents(102, 25, 51, 76);
        }

        [Test]
        public void Color_IsConstructedProperly_FromInt32RGB()
        {
            var result = new Color(12, 34, 56);

            TheResultingValue(result)
                .ShouldHaveArgbComponents(255, 12, 34, 56);
        }

        [Test]
        public void Color_IsConstructedProperly_FromInt32RGBA()
        {
            var result = new Color(12, 34, 56, 78);

            TheResultingValue(result)
                .ShouldHaveArgbComponents(78, 12, 34, 56);
        }

        [Test]
        public void Color_OpEquality()
        {
            var color1 = Color.White;
            var color2 = Color.White;
            var color3 = Color.Red;

            TheResultingValue(color1 == color2).ShouldBe(true);
            TheResultingValue(color1 == color3).ShouldBe(false);
        }

        [Test]
        public void Color_OpInequality()
        {
            var color1 = Color.White;
            var color2 = Color.White;
            var color3 = Color.Red;

            TheResultingValue(color1 != color2).ShouldBe(false);
            TheResultingValue(color1 != color3).ShouldBe(true);
        }

        [Test]
        public void Color_OpMultiply()
        {
            var colorOriginal = Color.Red;
            var colorAlpha = 0.5f;
            var colorMultiplied = colorOriginal * colorAlpha;

            TheResultingValue(colorMultiplied)
                .ShouldHaveArgbComponents(127, 127, 0, 0);
        }

        [Test]
        public void Color_EqualsObject()
        {
            var color1 = Color.Red;
            var color2 = Color.Red;

            TheResultingValue(color1.Equals((Object)color2)).ShouldBe(true);
            TheResultingValue(color1.Equals("This is a test")).ShouldBe(false);
        }

        [Test]
        public void Color_EqualsColor()
        {
            var color1 = Color.White;
            var color2 = Color.White;
            var color3 = Color.Red;

            TheResultingValue(color1.Equals(color2)).ShouldBe(true);
            TheResultingValue(color1.Equals(color3)).ShouldBe(false);
        }

        [Test]
        public void Color_SerializesToJson()
        {
            var color = Color.CornflowerBlue;
            
            var json = JsonConvert.SerializeObject(color,
                FrameworkJsonSerializerSettings.Instance);

            TheResultingString(json).ShouldBe($"[{color.R},{color.G},{color.B},{color.A}]");
        }

        [Test]
        public void Color_SerializesToJson_WhenNullable()
        {
            var color = Color.CornflowerBlue;

            var json = JsonConvert.SerializeObject((Color?)color, 
                FrameworkJsonSerializerSettings.Instance);

            TheResultingString(json).ShouldBe($"[{color.R},{color.G},{color.B},{color.A}]");
        }

        [Test]
        public void Color_DeserializesFromJson()
        {
            const String json = "[255,0,255,255]";
            
            var color = JsonConvert.DeserializeObject<Color>(json, 
                FrameworkJsonSerializerSettings.Instance);

            TheResultingValue(color).ShouldHavePackedValue(Color.Magenta.PackedValue);
        }

        [Test]
        public void Color_DeserializesFromJson_WhenNullable()
        {
            const String json1 = "[255,0,255,255]";

            var color1 = JsonConvert.DeserializeObject<Color?>(json1, 
                FrameworkJsonSerializerSettings.Instance);

            TheResultingValue(color1.Value)
                .ShouldHavePackedValue(Color.Magenta.PackedValue);

            const String json2 = "null";

            var color2 = JsonConvert.DeserializeObject<Color?>(json2, 
                FrameworkJsonSerializerSettings.Instance);

            TheResultingValue(color2.HasValue)
                .ShouldBe(false);
        }
    }
}
