using FluentAssertions;
using NUnit.Framework;

namespace Crimson.Tests
{
    [TestFixture]
    public class MathfTests
    {
        [TestFixture]
        public class Digits
        {
            [Test]
            public void Zero()
            {
                var result = 0.Digits();
                result.Should().Be(1);
            }

            [Test]
            public void Negative()
            {
                var result = (-101).Digits();
                result.Should().Be(3);
            }

            [Test]
            public void Large()
            {
                var result = 1234567890.Digits();
                result.Should().Be(10);
            }
        }

        [TestFixture]
        public class Axis
        {
            [Test]
            public void Negative()
            {
                var result = Mathf.Axis(true, false);
                result.Should().Be(-1);
            }

            [Test]
            public void Positive()
            {
                var result = Mathf.Axis(false, true);
                result.Should().Be(1);
            }

            [Test]
            public void Both()
            {
                var result = Mathf.Axis(true, true);
                result.Should().Be(0);
            }

            [Test]
            public void Neither()
            {
                var result = Mathf.Axis(false, false);
                result.Should().Be(0);
            }
        }
    }
}