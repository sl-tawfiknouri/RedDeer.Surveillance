namespace Domain.Core.Tests.Extensions
{
    using Domain.Core.Extensions;

    using NUnit.Framework;

    [TestFixture]
    public class CultureInfoConstantsTests
    {
        [Test]
        public void DefaultCultureInfo_Is_EnGb()
        {
            var defaultCulture = CultureInfoConstants.DefaultCultureInfo;

            Assert.AreEqual(defaultCulture, "en-GB");
        }
    }
}