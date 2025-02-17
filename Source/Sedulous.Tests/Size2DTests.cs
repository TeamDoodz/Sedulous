﻿using System;
using Newtonsoft.Json;
using NUnit.Framework;
using Sedulous.TestFramework;

namespace Sedulous.Tests
{
    [TestFixture]
    public class Size2DTests : FrameworkTestFramework
    {
        [Test]
        public void Size2D_IsConstructedProperly()
        {
            var result = new Size2D(123.45, 456.78);

            TheResultingValue(result)
                .ShouldBe(123.45, 456.78);
        }

        [Test]
        public void Size2D_OpEquality()
        {
            var size1 = new Size2D(123.45, 456.78);
            var size2 = new Size2D(123.45, 456.78);
            var size3 = new Size2D(123.45, 555.55);
            var size4 = new Size2D(222.22, 456.78);

            TheResultingValue(size1 == size2).ShouldBe(true);
            TheResultingValue(size1 == size3).ShouldBe(false);
            TheResultingValue(size1 == size4).ShouldBe(false);
        }

        [Test]
        public void Size2D_OpInequality()
        {
            var size1 = new Size2D(123.45, 456.78);
            var size2 = new Size2D(123.45, 456.78);
            var size3 = new Size2D(123.45, 555.55);
            var size4 = new Size2D(222.22, 456.78);

            TheResultingValue(size1 != size2).ShouldBe(false);
            TheResultingValue(size1 != size3).ShouldBe(true);
            TheResultingValue(size1 != size4).ShouldBe(true);
        }

        [Test]
        public void Size2D_EqualsObject()
        {
            var size1 = new Size2D(123.45, 456.78);
            var size2 = new Size2D(123.45, 456.78);

            TheResultingValue(size1.Equals((Object)size2)).ShouldBe(true);
            TheResultingValue(size1.Equals("This is a test")).ShouldBe(false);
        }

        [Test]
        public void Size2D_EqualsSize2D()
        {
            var size1 = new Size2D(123.45, 456.78);
            var size2 = new Size2D(123.45, 456.78);
            var size3 = new Size2D(123.45, 555.55);
            var size4 = new Size2D(222.22, 456.78);

            TheResultingValue(size1.Equals(size2)).ShouldBe(true);
            TheResultingValue(size1.Equals(size3)).ShouldBe(false);
            TheResultingValue(size1.Equals(size4)).ShouldBe(false);
        }
        
        [Test]
        public void Size2D_Area_IsCalculatedCorrectly()
        {
            var size1 = new Size2D(123.45, 456.78);
            TheResultingValue(size1.Area).ShouldBe(123.45 * 456.78);

            var size2 = new Size2D(222.22, 55555.55);
            TheResultingValue(size2.Area).ShouldBe(222.22 * 55555.55);
        }

        [Test]
        public void Size2D_SerializesToJson()
        {
            var size = new Size2D(1.2, 2.3);
            var json = JsonConvert.SerializeObject(size,
                FrameworkJsonSerializerSettings.Instance);

            TheResultingString(json).ShouldBe(@"{""width"":1.2,""height"":2.3}");
        }

        [Test]
        public void Size2D_SerializesToJson_WhenNullable()
        {
            var size = new Size2D(1.2, 2.3);
            var json = JsonConvert.SerializeObject((Size2D?)size,
                FrameworkJsonSerializerSettings.Instance);

            TheResultingString(json).ShouldBe(@"{""width"":1.2,""height"":2.3}");
        }

        [Test]
        public void Size2D_DeserializesFromJson()
        {
            const String json = @"{""width"":1.2,""height"":2.3}";
            
            var size = JsonConvert.DeserializeObject<Size2D>(json,
                FrameworkJsonSerializerSettings.Instance);

            TheResultingValue(size)
                .ShouldBe(1.2, 2.3);
        }

        [Test]
        public void Size2D_DeserializesFromJson_WhenNullable()
        {
            const String json1 = @"{""width"":1.2,""height"":2.3}";

            var size1 = JsonConvert.DeserializeObject<Size2D?>(json1,
                FrameworkJsonSerializerSettings.Instance);

            TheResultingValue(size1.Value)
                .ShouldBe(1.2, 2.3);

            const String json2 = @"null";

            var size2 = JsonConvert.DeserializeObject<Size2D?>(json2,
                FrameworkJsonSerializerSettings.Instance);

            TheResultingValue(size2.HasValue)
                .ShouldBe(false);
        }
    }
}
