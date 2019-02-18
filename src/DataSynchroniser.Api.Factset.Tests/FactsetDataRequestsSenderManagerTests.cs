using System;
using System.Reflection;
using DataSynchroniser.Manager;
using DataSynchroniser.Manager.Factset;
using Domain.Markets;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using RedDeer.Contracts.SurveillanceService.Api.FactsetSecurityDaily;
using Surveillance.DataLayer.Api.FactsetMarketData.Interfaces;

namespace DataSynchroniser.Tests.Manager.FactSet
{
    public class FactsetDataRequestsSenderManagerTests
    {
        [Test]
        public void Project_When_NoUniverseEventTimes_Then_DatesAreUtc()
        {
            // arrange
            Type[] parameterTypes = new Type[] {
            typeof(MarketDataRequest)};
            var method = typeof(FactsetDataRequestsSenderManager).GetMethod("Project", BindingFlags.Instance | BindingFlags.NonPublic, null, parameterTypes, new ParameterModifier[] { });
            var underTest = new FactsetDataRequestsSenderManager(
                A.Fake<IFactsetDailyBarApiRepository>(),
                A.Fake<ILogger<FactsetDataRequestsSenderManager>>());
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
