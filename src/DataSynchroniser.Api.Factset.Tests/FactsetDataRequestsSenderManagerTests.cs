using System;
using System.Reflection;
using DataSynchroniser.Api.Factset.Factset;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using RedDeer.Contracts.SurveillanceService.Api.FactsetSecurityDaily;
using SharedKernel.Contracts.Markets;
using Surveillance.Reddeer.ApiClient.FactsetMarketData.Interfaces;

namespace DataSynchroniser.Api.Factset.Tests
{
    public class FactsetDataRequestsSenderManagerTests
    {
        [Test]
        public void Project_When_NoUniverseEventTimes_Then_DatesAreUtc()
        {
            // arrange
            Type[] parameterTypes = new Type[] {
            typeof(MarketDataRequest)};
            var method = typeof(FactsetDataRequestsApiManager).GetMethod("Project", BindingFlags.Instance | BindingFlags.NonPublic, null, parameterTypes, new ParameterModifier[] { });
            var underTest = new FactsetDataRequestsApiManager(
                A.Fake<IFactsetDailyBarApi>(),
                A.Fake<ILogger<FactsetDataRequestsApiManager>>());
            var fakeRequest = A.Fake<MarketDataRequest>();
            
            var inputParams = new object[] { fakeRequest };

            // act
            var before = DateTime.UtcNow;
            FactsetSecurityRequestItem actual = (FactsetSecurityRequestItem)method.Invoke(underTest, parameters: inputParams);
            var after = DateTime.UtcNow;

            // assert
            Assert.IsTrue(actual.From >= before);
            Assert.AreEqual(DateTimeKind.Utc, actual.From.Kind);
            Assert.IsTrue(actual.To <= after);
            Assert.AreEqual(DateTimeKind.Utc, actual.To.Kind);
        }
    }
}
