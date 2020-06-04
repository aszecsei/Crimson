using System;
using FluentAssertions;
using NUnit.Framework;

namespace Crimson.Tests
{
    [TestFixture]
    public class UtilsTests
    {
        public enum TestEnum3
        {
            VariantA,
            VariantB,
            VariantC
        }
        
        public enum TestEnum0
        {
        }
        
        [TestFixture]
        public class EnumLength
        {
            [Test]
            public void TestLength0()
            {
                var result = Utils.EnumLength(typeof(TestEnum0));
                result.Should().Be(0);
            }

            [Test]
            public void TestLength3()
            {
                var result = Utils.EnumLength<TestEnum3>();
                result.Should().Be(3);
            }
        }

        [TestFixture]
        public class StringToEnum
        {
            [Test]
            public void StringToEnum0()
            {
                Action act = () => Utils.StringToEnum<TestEnum0>("hello, world");
                act.Should().Throw<Exception>();
            }
            
            [Test]
            public void StringToEnum3()
            {
                var res = Utils.StringToEnum<TestEnum3>("VariantB");
                res.Should().Be(TestEnum3.VariantB);
            }
        }

        [TestFixture]
        public class GetEnumValues
        {
            [Test]
            public void GetEnumValues0()
            {
                var res = Utils.GetEnumValues<TestEnum0>();
                res.Should().BeEmpty();
            }

            [Test]
            public void GetEnumValues3()
            {
                var res = Utils.GetEnumValues<TestEnum3>();
                res.Should().Equal(TestEnum3.VariantA, TestEnum3.VariantB, TestEnum3.VariantC);
            }
        }
    }
}