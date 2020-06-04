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

        [TestFixture]
        public class ClosestPointOnLine
        {
            [Test]
            public void OffLine()
            {
                var pointA = Vector2.Zero;
                var pointB = new Vector2(0, 2);
                var pointC = new Vector2(1.1f, 1);
                var result = Mathf.ClosestPointOnLine(pointA, pointB, pointC);
                result.Should().Be(new Vector2(0, 1));
            }

            [Test]
            public void OnLine()
            {
                var pointA = Vector2.Zero;
                var pointB = new Vector2(2, 2);
                var pointC = new Vector2(1, 1);
                var result = Mathf.ClosestPointOnLine(pointA, pointB, pointC);
                result.Should().Be(pointC);
            }
        }
    }
}