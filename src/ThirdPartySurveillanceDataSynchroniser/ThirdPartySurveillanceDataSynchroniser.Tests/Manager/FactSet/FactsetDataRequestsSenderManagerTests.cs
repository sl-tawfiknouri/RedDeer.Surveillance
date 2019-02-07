﻿using System;
using System.Reflection;

using Microsoft.Extensions.Logging;

using NUnit.Framework;
using FakeItEasy;

using ThirdPartySurveillanceDataSynchroniser.Manager;
using ThirdPartySurveillanceDataSynchroniser.Manager.Factset;
using RedDeer.Contracts.SurveillanceService.Api.FactsetSecurityDaily;
using Surveillance.DataLayer.Api.FactsetMarketData.Interfaces;

namespace ThirdPartySurveillanceDataSynchroniser.Tests.Manager.FactSet
{
    public class FactsetDataRequestsSenderManagerTests
    {
        [Test]
        public void Project_When_NoUniverseEventTimes_Then_DatesAreUtc()
        {
            // arrange
            Type[] parameterTypes = new Type[] {
            typeof(MarketDataRequestDataSource)};
            var method = typeof(FactsetDataRequestsSenderManager).GetMethod("Project", BindingFlags.Instance | BindingFlags.NonPublic, null, parameterTypes, new ParameterModifier[] { });
            var underTest = new FactsetDataRequestsSenderManager(
                A.Fake<IFactsetDailyBarApiRepository>(),
                A.Fake<ILogger<FactsetDataRequestsSenderManager>>());
            var fakeRequest = A.Fake<MarketDataRequestDataSource>();
            
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
