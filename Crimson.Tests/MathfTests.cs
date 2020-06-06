using Crimson;
using FluentAssertions;
using NUnit.Framework;
using Microsoft.Xna.Framework;

namespace Crimson.Tests
{
    [TestFixture]
    public class MathfTests
    {
        [TestFixture]
        public class Digits
        {
            [TestCase(0, 1)]
            [TestCase(-101, 3)]
            [TestCase(1234567890, 10)]
            public void CheckAgainstTruth(int num, int digits)
            {
                num.Digits().Should().Be(digits);
            }
        }

        [TestFixture]
        public class Axis
        {
            [TestCase(true, false, -1)]
            [TestCase(false, true, 1)]
            [TestCase(false, false, 0)]
            [TestCase(true, true, 0)]
            public void CheckAgainstTruth(bool neg, bool pos, int result)
            {
                Mathf.Axis(neg, pos).Should().Be(result);
            }
        }

        [TestFixture]
        public class NextPowerOfTwo
        {
            [TestCase(7, 8)]
            [TestCase(139, 256)]
            [TestCase(256, 256)]
            public void CheckAgainstTruth(int value, int nextPowerOfTwo)
            {
                Mathf.NextPowerOfTwo(value).Should().Be(nextPowerOfTwo);
            }
        }

        [TestFixture]
        public class IsPowerOfTwo
        {
            [TestCase(7, false)]
            [TestCase(32, true)]
            public void CheckAgainstTruth(int value, bool isPowerOfTwo)
            {
                Mathf.IsPowerOfTwo(value).Should().Be(isPowerOfTwo);
            }
        }

        [TestFixture]
        public class ClosestPowerOfTwo
        {
            [TestCase(7, 8)]
            [TestCase(19, 16)]
            public void CheckAgainstTruth(int value, int closestPowerOfTwo)
            {
                Mathf.ClosestPowerOfTwo(value).Should().Be(closestPowerOfTwo);
            }
        }
    }
}