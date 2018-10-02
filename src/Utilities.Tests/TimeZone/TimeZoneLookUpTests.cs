using System.Linq;
using NUnit.Framework;
using Utilities.TimeZone;

namespace Utilities.Tests.TimeZone
{
    [TestFixture]
    public class TimeZoneLookUpTests
    {
        private TimeZoneLookUp _lookup;

        [OneTimeSetUp]
        public void InitialSetUp()
        {
            _lookup = new TimeZoneLookUp();
        }

        [Test]
        public void GetLinuxTimeZoneFromMicrosoft_Returns_Expected()
        {
            var result = _lookup.GetLinuxTimeZoneFromMicrosoft("Central Standard Time");

            Assert.IsTrue(result.Contains("America/Chicago"));
        }

        [Test]
        public void GetMicrosoftTimeZoneFromLinux_Returns_Expected()
        {
            var result = _lookup.GetMicrosoftTimeZoneFromLinux("America/Chicago");

            Assert.IsTrue(result.Contains("Central Standard Time"));
        }
    }
}
