using NUnit.Framework;
using Sedulous.Core.TestFramework;

namespace Sedulous.Core.Tests
{
    [TestFixture]
    public class MathUtilTest : CoreTestFramework
    {
        [Test]
        public void MathUtil_ClampByte()
        {
            var result1 = MathUtility.Clamp((byte)123, (byte)0, (byte)44);
            TheResultingValue(result1).ShouldBe(44);

            var result2 = MathUtility.Clamp((byte)0, (byte)22, (byte)255);
            TheResultingValue(result2).ShouldBe(22);
        }

        [Test]
        public void MathUtil_ClampInt16()
        {
            var result1 = MathUtility.Clamp((short)123, (short)0, (short)44);
            TheResultingValue(result1).ShouldBe(44);

            var result2 = MathUtility.Clamp((short)0, (short)22, (short)255);
            TheResultingValue(result2).ShouldBe(22);

            var result3 = MathUtility.Clamp((short)-44, (short)-33, (short)255);
            TheResultingValue(result3).ShouldBe(-33);

            var result4 = MathUtility.Clamp((short)123, (short)-1000, (short)-25);
            TheResultingValue(result4).ShouldBe(-25);
        }

        [Test]
        public void MathUtil_ClampInt32()
        {
            var result1 = MathUtility.Clamp(123, 0, 44);
            TheResultingValue(result1).ShouldBe(44);

            var result2 = MathUtility.Clamp(0, 22, 255);
            TheResultingValue(result2).ShouldBe(22);

            var result3 = MathUtility.Clamp(-44, -33, 255);
            TheResultingValue(result3).ShouldBe(-33);

            var result4 = MathUtility.Clamp(123, -1000, -25);
            TheResultingValue(result4).ShouldBe(-25);
        }

        [Test]
        public void MathUtil_ClampInt64()
        {
            var result1 = MathUtility.Clamp(123, 0, (long)44);
            TheResultingValue(result1).ShouldBe(44);

            var result2 = MathUtility.Clamp(0, 22, (long)255);
            TheResultingValue(result2).ShouldBe(22);

            var result3 = MathUtility.Clamp(-44, -33, (long)255);
            TheResultingValue(result3).ShouldBe(-33);

            var result4 = MathUtility.Clamp(123, -1000, (long)-25);
            TheResultingValue(result4).ShouldBe(-25);
        }

        [Test]
        public void MathUtil_ClampUInt16()
        {
            var result1 = MathUtility.Clamp((ushort)123, (ushort)0, (ushort)44);
            TheResultingValue(result1).ShouldBe(44);

            var result2 = MathUtility.Clamp((ushort)0, (ushort)22, (ushort)255);
            TheResultingValue(result2).ShouldBe(22);
        }

        [Test]
        public void MathUtil_ClampUInt32()
        {
            var result1 = MathUtility.Clamp(123, 0, (uint)44);
            TheResultingValue(result1).ShouldBe(44);

            var result2 = MathUtility.Clamp(0, 22, (uint)255);
            TheResultingValue(result2).ShouldBe(22);
        }

        [Test]
        public void MathUtil_ClampUInt64()
        {
            var result1 = MathUtility.Clamp(123, 0, (ulong)44);
            TheResultingValue(result1).ShouldBe(44);

            var result2 = MathUtility.Clamp(0, 22, (ulong)255);
            TheResultingValue(result2).ShouldBe(22);
        }

        [Test]
        public void MathUtil_ClampSingle()
        {
            var result1 = MathUtility.Clamp(123f, 0f, 44f);
            TheResultingValue(result1).ShouldBe(44f);

            var result2 = MathUtility.Clamp(0f, 22f, 255f);
            TheResultingValue(result2).ShouldBe(22f);

            var result3 = MathUtility.Clamp(-44f, -33f, 255f);
            TheResultingValue(result3).ShouldBe(-33f);

            var result4 = MathUtility.Clamp(123f, -1000f, -25f);
            TheResultingValue(result4).ShouldBe(-25f);
        }

        [Test]
        public void MathUtil_ClampDouble()
        {
            var result1 = MathUtility.Clamp(123.0, 0.0, 44.0);
            TheResultingValue(result1).ShouldBe(44.0);

            var result2 = MathUtility.Clamp(0.0, 22.0, 255.0);
            TheResultingValue(result2).ShouldBe(22.0);

            var result3 = MathUtility.Clamp(-44.0, -33.0, 255.0);
            TheResultingValue(result3).ShouldBe(-33.0);

            var result4 = MathUtility.Clamp(123.0, -1000.0, -25.0);
            TheResultingValue(result4).ShouldBe(-25.0);
        }

        [Test]
        public void MathUtil_LerpByte()
        {
            var result1 = MathUtility.Lerp((byte)0, (byte)100, 0.25f);
            TheResultingValue(result1).ShouldBe(25);

            var result2 = MathUtility.Lerp((byte)0, (byte)100, 0.75f);
            TheResultingValue(result2).ShouldBe(75);
        }

        [Test]
        public void MathUtil_LerpInt16()
        {
            var result1 = MathUtility.Lerp((short)0, (short)100, 0.25f);
            TheResultingValue(result1).ShouldBe(25);

            var result2 = MathUtility.Lerp((short)0, (short)100, 0.75f);
            TheResultingValue(result2).ShouldBe(75);

            var result3 = MathUtility.Lerp((short)-50, (short)50, 0.25f);
            TheResultingValue(result3).ShouldBe(-25);

            var result4 = MathUtility.Lerp((short)-50, (short)50, 0.75f);
            TheResultingValue(result4).ShouldBe(25);
        }

        [Test]
        public void MathUtil_LerpInt32()
        {
            var result1 = MathUtility.Lerp(0, 100, 0.25f);
            TheResultingValue(result1).ShouldBe(25);

            var result2 = MathUtility.Lerp(0, 100, 0.75f);
            TheResultingValue(result2).ShouldBe(75);

            var result3 = MathUtility.Lerp(-50, 50, 0.25f);
            TheResultingValue(result3).ShouldBe(-25);

            var result4 = MathUtility.Lerp(-50, 50, 0.75f);
            TheResultingValue(result4).ShouldBe(25);
        }

        [Test]
        public void MathUtil_LerpInt64()
        {
            var result1 = MathUtility.Lerp(0, (long)100, 0.25f);
            TheResultingValue(result1).ShouldBe(25);

            var result2 = MathUtility.Lerp(0, (long)100, 0.75f);
            TheResultingValue(result2).ShouldBe(75);

            var result3 = MathUtility.Lerp(-50, (long)50, 0.25f);
            TheResultingValue(result3).ShouldBe(-25);

            var result4 = MathUtility.Lerp(-50, (long)50, 0.75f);
            TheResultingValue(result4).ShouldBe(25);
        }

        [Test]
        public void MathUtil_LerpUInt16()
        {
            var result1 = MathUtility.Lerp((ushort)0, (ushort)100, 0.25f);
            TheResultingValue(result1).ShouldBe(25);

            var result2 = MathUtility.Lerp((ushort)0, (ushort)100, 0.75f);
            TheResultingValue(result2).ShouldBe(75);
        }

        [Test]
        public void MathUtil_LerpUInt32()
        {
            var result1 = MathUtility.Lerp(0, (uint)100, 0.25f);
            TheResultingValue(result1).ShouldBe(25);

            var result2 = MathUtility.Lerp(0, (uint)100, 0.75f);
            TheResultingValue(result2).ShouldBe(75);
        }

        [Test]
        public void MathUtil_LerpUInt64()
        {
            var result1 = MathUtility.Lerp(0, (ulong)100, 0.25f);
            TheResultingValue(result1).ShouldBe(25);

            var result2 = MathUtility.Lerp(0, (ulong)100, 0.75f);
            TheResultingValue(result2).ShouldBe(75);
        }

        [Test]
        public void MathUtil_LerpSingle()
        {
            var result1 = MathUtility.Lerp(0f, 100f, 0.25f);
            TheResultingValue(result1).ShouldBe(25f);

            var result2 = MathUtility.Lerp(0f, 100f, 0.75f);
            TheResultingValue(result2).ShouldBe(75f);

            var result3 = MathUtility.Lerp(-50f, 50f, 0.25f);
            TheResultingValue(result3).ShouldBe(-25f);

            var result4 = MathUtility.Lerp(-50f, 50f, 0.75f);
            TheResultingValue(result4).ShouldBe(25f);
        }

        [Test]
        public void MathUtil_LerpDouble()
        {
            var result1 = MathUtility.Lerp(0.0, 100.0, 0.25f);
            TheResultingValue(result1).ShouldBe(25f);

            var result2 = MathUtility.Lerp(0.0, 100.0, 0.75f);
            TheResultingValue(result2).ShouldBe(75f);

            var result3 = MathUtility.Lerp(-50.0, 50.0, 0.25f);
            TheResultingValue(result3).ShouldBe(-25f);

            var result4 = MathUtility.Lerp(-50.0, 50.0, 0.75f);
            TheResultingValue(result4).ShouldBe(25f); 
        }
    }
}
