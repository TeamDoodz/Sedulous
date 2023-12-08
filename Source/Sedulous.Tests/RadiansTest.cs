using System;
using Newtonsoft.Json;
using NUnit.Framework;
using Sedulous.TestFramework;

namespace Sedulous.Tests
{
    [TestFixture]
    public class RadiansTest : FrameworkTestFramework
    {
        [Test]
        public void Radians_SerializesToJson()
        {
            var radians = 1.234f;
            var json = JsonConvert.SerializeObject(radians);

            TheResultingString(json).ShouldBe(@"1.234");
        }

        [Test]
        public void Radians_SerializesToJson_WhenNullable()
        {
            var radians = 1.234f;
            var json = JsonConvert.SerializeObject((Radians?)radians, 
                FrameworkJsonSerializerSettings.Instance);

            TheResultingString(json).ShouldBe(@"1.234");
        }

        [Test]
        public void Radians_DeserializesFromJson()
        {
            const String json = @"1.234";
            
            var radians = JsonConvert.DeserializeObject<Radians>(json, 
                FrameworkJsonSerializerSettings.Instance);

            TheResultingValue(radians)
                .ShouldBe(1.234f);
        }

        [Test]
        public void Radians_DeserializesFromJson_WhenNullable()
        {
            const String json1 = @"1.234";

            var radians1 = JsonConvert.DeserializeObject<Radians?>(json1, 
                FrameworkJsonSerializerSettings.Instance);

            TheResultingValue(radians1.Value)
                .ShouldBe(1.234f);

            const String json2 = @"null";

            var radians2 = JsonConvert.DeserializeObject<Radians?>(json2, 
                FrameworkJsonSerializerSettings.Instance);

            TheResultingValue(radians2.HasValue)
                .ShouldBe(false);
        }
    }
}
