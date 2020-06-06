using Crimson;
using FluentAssertions;
using NUnit.Framework;
using Microsoft.Xna.Framework;

namespace Crimson.Tests
{
    [TestFixture]
    public class MathfTests
    {
        [TestCase(0, ExpectedResult = 1)]
        [TestCase(-101, ExpectedResult = 3)]
        [TestCase(1234567890, ExpectedResult = 10)]
        public int DigitsTest(int num)
        {
            return num.Digits();
        }
        
        [TestCase(true, false, ExpectedResult = -1)]
        [TestCase(false, true, ExpectedResult = 1)]
        [TestCase(false, false, ExpectedResult = 0)]
        [TestCase(true, true, ExpectedResult = 0)]
        public int AxisTest(bool neg, bool pos)
        {
            return Mathf.Axis(neg, pos);
        }
        
        [TestCase(7, ExpectedResult = 8)]
        [TestCase(139, ExpectedResult = 256)]
        [TestCase(256, ExpectedResult = 256)]
        public int NextPowerOfTwoTest(int value)
        {
            return Mathf.NextPowerOfTwo(value);
        }
        
        [TestCase(7, ExpectedResult = false)]
        [TestCase(32, ExpectedResult = true)]
        public bool IsPowerOfTwoTest(int value)
        {
            return Mathf.IsPowerOfTwo(value);
        }
        
        [TestCase(7, ExpectedResult = 8)]
        [TestCase(19, ExpectedResult = 16)]
        public int ClosestPowerOfTwoTest(int value)
        {
            return Mathf.ClosestPowerOfTwo(value);
        }
    }
}