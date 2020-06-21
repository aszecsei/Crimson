using FluentAssertions;
using Microsoft.Xna.Framework;
using NUnit.Framework;

namespace Crimson.Tests.Spatial
{
    [TestFixture]
    public class BoundsTests
    {
        public static Bounds GetTestBounds1() => new Bounds(new Vector3(1, 1, 1), new Vector3(2, 4, 6));
        public static Bounds GetTestBounds2() => new Bounds(new Vector3(1, 2, 1), new Vector3(2, 4, 6));
        public static Bounds GetTestBounds3() => new Bounds(new Vector3(10, -5, 9), new Vector3(2, 4, 6));

        [TestFixture]
        public class ClosestPoint
        {
            [Test]
            public void Inside()
            {
                var res = GetTestBounds1();
                var point = new Vector3(1.5f, 2f, 3f);
                var closest = res.ClosestPoint(point);
                closest.Should().Be(point);
            }

            [Test]
            public void MatchTwoDims()
            {
                var res = GetTestBounds1();
                var point = new Vector3(5f, 2f, 3f);
                var closest = res.ClosestPoint(point);
                closest.Should().Be(new Vector3(2, 2, 3));
            }

            [Test]
            public void MatchOneDim()
            {
                var res = GetTestBounds1();
                var point = new Vector3(5f, -3f, 3f);
                var closest = res.ClosestPoint(point);
                closest.Should().Be(new Vector3(2, -1, 3));
            }

            [Test]
            public void MatchNoDims()
            {
                var res = GetTestBounds1();
                var point = new Vector3(5f, -3f, 100f);
                var closest = res.ClosestPoint(point);
                closest.Should().Be(new Vector3(2, -1, 4));
            }
        }

        [TestFixture]
        public class Intersects
        {
            [Test]
            public void Intersects12()
            {
                GetTestBounds1().Intersects(GetTestBounds2()).Should().Be(true);
            }

            [Test]
            public void Intersects23()
            {
                GetTestBounds2().Intersects(GetTestBounds3()).Should().Be(false);
            }

            [Test]
            public void Intersects13()
            {
                GetTestBounds1().Intersects(GetTestBounds3()).Should().Be(false);
            }
        }
    }
}