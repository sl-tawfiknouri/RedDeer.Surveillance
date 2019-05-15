using FluentAssertions;
using NUnit.Framework;
using Surveillance.Api.DataAccess.Abstractions.Services;
using Surveillance.Api.DataAccess.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Surveillance.Api.Tests.Tests
{
    [TestFixture]
    public class TimeZoneServiceTests
    {
        private ITimeZoneService _timeZoneService;

        [SetUp]
        public void SetUp()
        {
            _timeZoneService = new TimeZoneService();
        }

        [Test]
        public void CanGenerateOffsetSegments_LosAngeles()
        {
            // arrange
            var from = new DateTime(2018, 01, 01, 00, 00, 00, DateTimeKind.Utc);
            var to = new DateTime(2019, 01, 01, 00, 00, 00, DateTimeKind.Utc);
            var tzName = "America/Los_Angeles";

            // act
            var segments = _timeZoneService.GetOffsetSegments(from, to, tzName).ToList();

            // assert
            segments.Should().HaveCount(3);

            segments[0].FromUtc.Should().Be(new DateTime(2018, 01, 01, 00, 00, 00, DateTimeKind.Utc));
            segments[0].FromIncluding.Should().Be(true);
            segments[0].ToUtc.Should().Be(new DateTime(2018, 03, 11, 10, 00, 00, DateTimeKind.Utc));
            segments[0].ToIncluding.Should().Be(false);
            segments[0].HourOffset.Should().Be(-8);

            segments[1].FromUtc.Should().Be(new DateTime(2018, 03, 11, 10, 00, 00, DateTimeKind.Utc));
            segments[1].FromIncluding.Should().Be(true);
            segments[1].ToUtc.Should().Be(new DateTime(2018, 11, 04, 09, 00, 00, DateTimeKind.Utc));
            segments[1].ToIncluding.Should().Be(false);
            segments[1].HourOffset.Should().Be(-7);

            segments[2].FromUtc.Should().Be(new DateTime(2018, 11, 04, 09, 00, 00, DateTimeKind.Utc));
            segments[2].FromIncluding.Should().Be(true);
            segments[2].ToUtc.Should().Be(new DateTime(2019, 01, 01, 00, 00, 00, DateTimeKind.Utc));
            segments[2].ToIncluding.Should().Be(true);
            segments[2].HourOffset.Should().Be(-8);
        }

        [Test]
        public void CanGenerateOffsetSegments_Phoenix()
        {
            // arrange
            var from = new DateTime(2018, 01, 01, 00, 00, 00, DateTimeKind.Utc);
            var to = new DateTime(2019, 01, 01, 00, 00, 00, DateTimeKind.Utc);
            var tzName = "America/Phoenix";

            // act
            var segments = _timeZoneService.GetOffsetSegments(from, to, tzName).ToList();

            // assert
            segments.Should().HaveCount(1);

            segments[0].FromUtc.Should().Be(new DateTime(2018, 01, 01, 00, 00, 00, DateTimeKind.Utc));
            segments[0].FromIncluding.Should().Be(true);
            segments[0].ToUtc.Should().Be(new DateTime(2019, 01, 01, 00, 00, 00, DateTimeKind.Utc));
            segments[0].ToIncluding.Should().Be(true);
            segments[0].HourOffset.Should().Be(-7);
        }

        [Test]
        public void CanGenerateOffsetSegments_London()
        {
            // arrange
            var from = new DateTime(2019, 06, 01, 00, 00, 00, DateTimeKind.Utc);
            var to = new DateTime(2020, 01, 01, 00, 00, 00, DateTimeKind.Utc);
            var tzName = "Europe/London";

            // act
            var segments = _timeZoneService.GetOffsetSegments(from, to, tzName).ToList();

            // assert
            segments.Should().HaveCount(2);

            segments[0].FromUtc.Should().Be(new DateTime(2019, 06, 01, 00, 00, 00, DateTimeKind.Utc));
            segments[0].FromIncluding.Should().Be(true);
            segments[0].ToUtc.Should().Be(new DateTime(2019, 10, 27, 01, 00, 00, DateTimeKind.Utc));
            segments[0].ToIncluding.Should().Be(false);
            segments[0].HourOffset.Should().Be(1);

            segments[1].FromUtc.Should().Be(new DateTime(2019, 10, 27, 01, 00, 00, DateTimeKind.Utc));
            segments[1].FromIncluding.Should().Be(true);
            segments[1].ToUtc.Should().Be(new DateTime(2020, 01, 01, 00, 00, 00, DateTimeKind.Utc));
            segments[1].ToIncluding.Should().Be(true);
            segments[1].HourOffset.Should().Be(0);
        }
    }
}
