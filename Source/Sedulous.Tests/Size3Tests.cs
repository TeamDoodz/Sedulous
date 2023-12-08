﻿using System;
using Newtonsoft.Json;
using NUnit.Framework;
using Sedulous.TestFramework;

namespace Sedulous.Tests
{
    [TestFixture]
    public class Size3Tests : FrameworkTestFramework
    {
        [Test]
        public void Size3_IsConstructedProperly()
        {
            var result = new Size3(123, 456, 789);

            TheResultingValue(result)
                .ShouldBe(123, 456, 789);
        }

        [Test]
        public void Size3_OpEquality()
        {
            var volume1 = new Size3(123, 456, 789);
            var volume2 = new Size3(123, 456, 789);
            var volume3 = new Size3(123, 555, 789);
            var volume4 = new Size3(222, 456, 789);
            var volume5 = new Size3(123, 456, 999);

            TheResultingValue(volume1 == volume2).ShouldBe(true);
            TheResultingValue(volume1 == volume3).ShouldBe(false);
            TheResultingValue(volume1 == volume4).ShouldBe(false);
            TheResultingValue(volume1 == volume5).ShouldBe(false);
        }

        [Test]
        public void Size3_OpInequality()
        {
            var volume1 = new Size3(123, 456, 789);
            var volume2 = new Size3(123, 456, 789);
            var volume3 = new Size3(123, 555, 789);
            var volume4 = new Size3(222, 456, 789);
            var volume5 = new Size3(123, 456, 999);

            TheResultingValue(volume1 != volume2).ShouldBe(false);
            TheResultingValue(volume1 != volume3).ShouldBe(true);
            TheResultingValue(volume1 != volume4).ShouldBe(true);
            TheResultingValue(volume1 != volume5).ShouldBe(true);
        }

        [Test]
        public void Size3_EqualsObject()
        {
            var volume1 = new Size3(123, 456, 789);
            var volume2 = new Size3(123, 456, 789);

            TheResultingValue(volume1.Equals((Object)volume2)).ShouldBe(true);
            TheResultingValue(volume1.Equals("This is a test")).ShouldBe(false);
        }

        [Test]
        public void Size3_EqualsSize3()
        {
            var volume1 = new Size3(123, 456, 789);
            var volume2 = new Size3(123, 456, 789);
            var volume3 = new Size3(123, 555, 789);
            var volume4 = new Size3(222, 456, 789);
            var volume5 = new Size3(123, 456, 999);

            TheResultingValue(volume1.Equals(volume2)).ShouldBe(true);
            TheResultingValue(volume1.Equals(volume3)).ShouldBe(false);
            TheResultingValue(volume1.Equals(volume4)).ShouldBe(false);
            TheResultingValue(volume1.Equals(volume5)).ShouldBe(false);
        }
        
        [Test]
        public void Size3_TotalSize3_IsCalculatedCorrectly()
        {
            var volume1 = new Size3(123, 456, 789);
            TheResultingValue(volume1.Volume).ShouldBe(123 * 456 * 789);

            var volume2 = new Size3(222, 555, 999);
            TheResultingValue(volume2.Volume).ShouldBe(222 * 555 * 999);
        }
        
        [Test]
        public void Size3_SerializesToJson()
        {
            var size = new Size3(1, 2, 3);
            var json = JsonConvert.SerializeObject(size,
                FrameworkJsonSerializerSettings.Instance);

            TheResultingString(json).ShouldBe(@"{""width"":1,""height"":2,""depth"":3}");
        }

        [Test]
        public void Size3_SerializesToJson_WhenNullable()
        {
            var size = new Size3(1, 2, 3);
            var json = JsonConvert.SerializeObject((Size3?)size,
                FrameworkJsonSerializerSettings.Instance);

            TheResultingString(json).ShouldBe(@"{""width"":1,""height"":2,""depth"":3}");
        }

        [Test]
        public void Size3_DeserializesFromJson()
        {
            const String json = @"{""width"":1,""height"":2,""depth"":3}";
            
            var size = JsonConvert.DeserializeObject<Size3>(json,
                FrameworkJsonSerializerSettings.Instance);

            TheResultingValue(size)
                .ShouldBe(1, 2, 3);
        }

        [Test]
        public void Size3_DeserializesFromJson_WhenNullable()
        {
            const String json1 = @"{""width"":1,""height"":2,""depth"":3}";

            var size1 = JsonConvert.DeserializeObject<Size3?>(json1,
                FrameworkJsonSerializerSettings.Instance);

            TheResultingValue(size1.Value)
                .ShouldBe(1, 2, 3);

            const String json2 = @"null";

            var size2 = JsonConvert.DeserializeObject<Size3?>(json2,
                FrameworkJsonSerializerSettings.Instance);

            TheResultingValue(size2.HasValue)
                .ShouldBe(false);
        }
    }
}
