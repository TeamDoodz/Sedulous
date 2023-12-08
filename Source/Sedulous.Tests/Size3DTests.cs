﻿using System;
using Newtonsoft.Json;
using NUnit.Framework;
using Sedulous.TestFramework;

namespace Sedulous.Tests
{
    [TestFixture]
    public class Size3DTests : FrameworkTestFramework
    {
        [Test]
        public void Size3D_IsConstructedProperly()
        {
            var result = new Size3D(123.45, 456.78, 789.99);

            TheResultingValue(result)
                .ShouldBe(123.45, 456.78, 789.99);
        }

        [Test]
        public void Size3D_OpEquality()
        {
            var volume1 = new Size3D(123.45, 456.78, 789.99);
            var volume2 = new Size3D(123.45, 456.78, 789.99);
            var volume3 = new Size3D(123.45, 555, 789.99);
            var volume4 = new Size3D(222, 456.78, 789.99);
            var volume5 = new Size3D(123.45, 456.78, 999);

            TheResultingValue(volume1 == volume2).ShouldBe(true);
            TheResultingValue(volume1 == volume3).ShouldBe(false);
            TheResultingValue(volume1 == volume4).ShouldBe(false);
            TheResultingValue(volume1 == volume5).ShouldBe(false);
        }

        [Test]
        public void Size3D_OpInequality()
        {
            var volume1 = new Size3D(123.45, 456.78, 789.99);
            var volume2 = new Size3D(123.45, 456.78, 789.99);
            var volume3 = new Size3D(123.45, 555, 789.99);
            var volume4 = new Size3D(222, 456.78, 789.99);
            var volume5 = new Size3D(123.45, 456.78, 999);

            TheResultingValue(volume1 != volume2).ShouldBe(false);
            TheResultingValue(volume1 != volume3).ShouldBe(true);
            TheResultingValue(volume1 != volume4).ShouldBe(true);
            TheResultingValue(volume1 != volume5).ShouldBe(true);
        }

        [Test]
        public void Size3D_EqualsObject()
        {
            var volume1 = new Size3D(123.45, 456.78, 789.99);
            var volume2 = new Size3D(123.45, 456.78, 789.99);

            TheResultingValue(volume1.Equals((Object)volume2)).ShouldBe(true);
            TheResultingValue(volume1.Equals("This is a test")).ShouldBe(false);
        }

        [Test]
        public void Size3D_EqualsSize3D()
        {
            var volume1 = new Size3D(123.45, 456.78, 789.99);
            var volume2 = new Size3D(123.45, 456.78, 789.99);
            var volume3 = new Size3D(123.45, 555, 789.99);
            var volume4 = new Size3D(222, 456.78, 789.99);
            var volume5 = new Size3D(123.45, 456.78, 999);

            TheResultingValue(volume1.Equals(volume2)).ShouldBe(true);
            TheResultingValue(volume1.Equals(volume3)).ShouldBe(false);
            TheResultingValue(volume1.Equals(volume4)).ShouldBe(false);
            TheResultingValue(volume1.Equals(volume5)).ShouldBe(false);
        }
        
        [Test]
        public void Size3D_Volume_IsCalculatedCorrectly()
        {
            var volume1width  = 123.45;
            var volume1height = 456.78;
            var volume1depth  = 789.99;
            var volume1 = new Size3D(volume1width, volume1height, volume1depth);
            TheResultingValue(volume1.Volume).ShouldBe(volume1width * volume1height * volume1depth);

            var volume2width  = 222.22;
            var volume2height = 555.55;
            var volume2depth  = 999.99;
            var volume2       = new Size3D(volume2width, volume2height, volume2depth);
            TheResultingValue(volume2.Volume).ShouldBe(volume2width * volume2height * volume2depth);
        }
        
        [Test]
        public void Size3D_SerializesToJson()
        {
            var size = new Size3D(1.2, 2.3, 3.4);
            var json = JsonConvert.SerializeObject(size,
                FrameworkJsonSerializerSettings.Instance);

            TheResultingString(json).ShouldBe(@"{""width"":1.2,""height"":2.3,""depth"":3.4}");
        }

        [Test]
        public void Size3D_SerializesToJson_WhenNullable()
        {
            var size = new Size3D(1.2, 2.3, 3.4);
            var json = JsonConvert.SerializeObject((Size3D?)size,
                FrameworkJsonSerializerSettings.Instance);

            TheResultingString(json).ShouldBe(@"{""width"":1.2,""height"":2.3,""depth"":3.4}");
        }

        [Test]
        public void Size3D_DeserializesFromJson()
        {
            const String json = @"{""width"":1.2,""height"":2.3,""depth"":3.4}";
            
            var size = JsonConvert.DeserializeObject<Size3D>(json,
                FrameworkJsonSerializerSettings.Instance);

            TheResultingValue(size)
                .ShouldBe(1.2, 2.3, 3.4);
        }

        [Test]
        public void Size3D_DeserializesFromJson_WhenNullable()
        {
            const String json1 = @"{""width"":1.2,""height"":2.3,""depth"":3.4}";

            var size1 = JsonConvert.DeserializeObject<Size3D?>(json1,
                FrameworkJsonSerializerSettings.Instance);

            TheResultingValue(size1.Value)
                .ShouldBe(1.2, 2.3, 3.4);

            const String json2 = @"null";

            var size2 = JsonConvert.DeserializeObject<Size3D?>(json2,
                FrameworkJsonSerializerSettings.Instance);

            TheResultingValue(size2.HasValue)
                .ShouldBe(false);
        }
    }
}
