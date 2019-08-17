using System;
using System.Linq;
using Domain.Core.Extensions;
using NUnit.Framework;

namespace Domain.Core.Tests.Extensions
{
    [TestFixture]
    public class EnumExtensionsTests
    {
        [Test]
        public void GetDescription_ThrowsArgumentException_ForNonEnum()
        {
            var structs = new TestStruct();

            Assert.Throws<ArgumentException>(() => structs.GetDescription());
        }

        [Test]
        public void GetDescription_ReturnsExpectedEnumDescriptionForNoDescription()
        {
            var enumVal = TestEnum.Test0;
            var desc = enumVal.GetDescription();

            Assert.AreEqual(desc, "Test0");
        }

        [Test]
        public void GetDescription_ReturnsExpectedEnumDescriptionForHasADescription()
        {
            var enumVal = TestEnum.Test1;
            var desc = enumVal.GetDescription();

            Assert.AreEqual(desc, "crazy test");
        }

        [Test]
        public void GetEnumPermutations_Returns_AllEnumValues()
        {
            var enumPermutations = TestEnum.Test0.GetEnumPermutations();

            Assert.AreEqual(enumPermutations.Count, 2);
            Assert.AreEqual(enumPermutations.First(), TestEnum.Test0);
            Assert.AreEqual(enumPermutations.Skip(1).First(), TestEnum.Test1);
        }

        [Test]
        public void TryParseEnumPermutation_ExactMatch_Success()
        {
            var parsePermutation = "Test0";

            var success = EnumExtensions.TryParsePermutations(parsePermutation, out TestEnum result);

            Assert.IsTrue(success);
            Assert.AreEqual(result, TestEnum.Test0);
        }

        [Test]
        public void TryParseEnumPermutation_InExactMatch_Success()
        {
            var parsePermutation = "test0";

            var success = EnumExtensions.TryParsePermutations(parsePermutation, out TestEnum result);

            Assert.IsTrue(success);
            Assert.AreEqual(result, TestEnum.Test0);
        }

        [Test]
        public void TryParseEnumPermutation_InExactMatchOther_Success()
        {
            var parsePermutation = "test1";

            var success = EnumExtensions.TryParsePermutations(parsePermutation, out TestEnum result);

            Assert.IsTrue(success);
            Assert.AreEqual(result, TestEnum.Test1);
        }
        
        private struct TestStruct
        {
        }

        private enum TestEnum
        {
            Test0,
            [System.ComponentModel.Description("crazy test")]
            Test1
        }
    }
}
