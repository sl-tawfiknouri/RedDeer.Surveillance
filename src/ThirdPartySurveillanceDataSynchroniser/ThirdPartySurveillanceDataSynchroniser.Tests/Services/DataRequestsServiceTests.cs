using System;
using NUnit.Framework;
using ThirdPartySurveillanceDataSynchroniser.Services;

namespace ThirdPartySurveillanceDataSynchroniser.Tests.Services
{
    [TestFixture]
    public class DataRequestsServiceTests
    {
        [Test]
        public void Constructor_NullLogger_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new DataRequestsService(null));
        }
    }
}
