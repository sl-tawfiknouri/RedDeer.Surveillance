namespace Domain.Core.Tests.Markets.Timebars
{
    using Domain.Core.Markets.Timebars;

    using NUnit.Framework;

    [TestFixture]
    public class VolumeTests
    {
        [Test]
        public void Ctor_AssignsVariable_Correctly()
        {
            var vol = new Volume(100);

            Assert.AreEqual(100, vol.Traded);
        }
    }
}