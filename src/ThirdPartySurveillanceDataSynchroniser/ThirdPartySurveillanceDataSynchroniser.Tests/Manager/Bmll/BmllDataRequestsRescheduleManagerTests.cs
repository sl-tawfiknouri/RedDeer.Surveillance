using System;
using NUnit.Framework;
using ThirdPartySurveillanceDataSynchroniser.Manager.Bmll;

namespace ThirdPartySurveillanceDataSynchroniser.Tests.Manager.Bmll
{
    [TestFixture]
    public class BmllDataRequestsRescheduleManagerTests
    {
        [Test]
        public void Ctor_Throws_For_Null_Logger()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new BmllDataRequestsRescheduleManager(null));
        }
    }
}
