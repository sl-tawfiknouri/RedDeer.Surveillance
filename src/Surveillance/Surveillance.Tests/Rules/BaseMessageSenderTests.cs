using System;
using System.Reflection;

using Microsoft.Extensions.Logging;

using NUnit.Framework;
using FakeItEasy;

using Contracts.SurveillanceService.ComplianceCase;
using Surveillance.Rules;
using Surveillance.Rules.Interfaces;
using Surveillance.Rules.WashTrade;
using Surveillance.MessageBusIO.Interfaces;

namespace Surveillance.Tests.Rules
{
    public class BaseMessageSenderTests
    {
        [Test]
        public void CaseDataItem_When_ComplianceDateCaseDataItemDtoCreated_Then_DatesAreUtc()
        {
            // arrange
            Type[] parameterTypes = new Type[] {
            typeof(IRuleBreach),
            typeof(string)};
            var method = typeof(BaseMessageSender).GetMethod("CaseDataItem", BindingFlags.Instance | BindingFlags.NonPublic, null, parameterTypes, new ParameterModifier[] { });
            var underTest = (BaseMessageSender)new WashTradeRuleMessageSender(A.Fake<ILogger<WashTradeRuleMessageSender>>(), A.Fake<ICaseMessageSender>());
            var breach = A.Fake<IRuleBreach>();
            A.CallTo(() => breach.Trades).Returns(null);
            var inputParams = new object[] { breach, "Test description" };

            // act
            var before = DateTime.UtcNow;
            ComplianceCaseDataItemDto actual = (ComplianceCaseDataItemDto)method.Invoke(underTest, parameters: inputParams);
            var after = DateTime.UtcNow;

            // assert
            Assert.IsTrue(actual.ReportedOn >= before);
            Assert.IsTrue(actual.StatusUpdatedOn >= before);
            Assert.IsTrue(actual.StartOfPeriodUnderInvestigation >= before);
            Assert.IsTrue(actual.EndOfPeriodUnderInvestigation >= before);
            Assert.IsTrue(actual.ReportedOn <= after);
            Assert.IsTrue(actual.StatusUpdatedOn <= after);
            Assert.IsTrue(actual.StartOfPeriodUnderInvestigation <= after);
            Assert.IsTrue(actual.EndOfPeriodUnderInvestigation <= after);

            Assert.AreEqual(DateTimeKind.Utc, actual.ReportedOn.Kind);
            Assert.AreEqual(DateTimeKind.Utc, actual.StatusUpdatedOn.Kind);
            Assert.AreEqual(DateTimeKind.Utc, actual.StartOfPeriodUnderInvestigation.Kind);
            Assert.AreEqual(DateTimeKind.Utc, actual.EndOfPeriodUnderInvestigation.Kind);
        }
    }
}
