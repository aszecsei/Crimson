using FluentAssertions;
using Microsoft.Xna.Framework;
using NUnit.Framework;

namespace Crimson.Tests.Spatial
{
    [TestFixture]
    public class Bounds2DTests
    {
        public static Bounds2D GetTestBounds1() => new Bounds2D(new Vector2(1, 1), new Vector2(2, 4));
        public static Bounds2D GetTestBounds2() => new Bounds2D(new Vector2(1, 2), new Vector2(2, 4));
        public static Bounds2D GetTestBounds3() => new Bounds2D(new Vector2(10, -5), new Vector2(2, 4));

        [TestFixture]
        public class ClosestPoint
        {
            [Test]
            public void Inside()
            {
                var res = GetTestBounds1();
                var point = new Vector2(1.5f, 2f);
                var closest = res.ClosestPoint(point);
                closest.Should().Be(point);
            }

            [Test]
            public void MatchOneDim()
            {
                var res = GetTestBounds1();
                var point = new Vector2(5f, 2f);
                var closest = res.ClosestPoint(point);
                closest.Should().Be(new Vector2(2, 2));
            }

            [Test]
            public void MatchNoDims()
            {
                var res = GetTestBounds1();
                var point = new Vector2(5f, -3f);
                var closest = res.ClosestPoint(point);
                closest.Should().Be(new Vector2(2, -1));
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