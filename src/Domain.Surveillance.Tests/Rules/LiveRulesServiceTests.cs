namespace Domain.Surveillance.Tests.Rules
{
    using System.Linq;

    using Domain.Surveillance.Rules;
    using Domain.Surveillance.Scheduling;

    using NUnit.Framework;

    [TestFixture]
    public class LiveRulesServiceTests
    {
        [Test]
        public void LiveRules_Contains_ExpectedRules()
        {
            var liveRulesService = new ActiveRulesService();

            var result = liveRulesService.EnabledRules();

            Assert.IsTrue(result.Contains(Rules.HighVolume));
            Assert.IsTrue(result.Contains(Rules.FixedIncomeWashTrades));
            Assert.IsTrue(result.Contains(Rules.HighProfits));
            Assert.IsTrue(result.Contains(Rules.WashTrade));
            Assert.IsTrue(result.Contains(Rules.CancelledOrders));
            Assert.IsTrue(result.Contains(Rules.PaintingTheTape));
            Assert.IsTrue(result.Contains(Rules.PlacingOrderWithNoIntentToExecute));

            Assert.IsFalse(result.Contains(Rules.UniverseFilter));
            Assert.IsFalse(result.Contains(Rules.PumpAndDump));
            Assert.IsFalse(result.Contains(Rules.TrashAndCash));
            Assert.IsFalse(result.Contains(Rules.CrossAssetManipulation));
        }

        [TestCase(Rules.HighVolume, true)]
        [TestCase(Rules.CancelledOrders, true)]
        [TestCase(Rules.WashTrade, true)]
        [TestCase(Rules.PlacingOrderWithNoIntentToExecute, true)]
        [TestCase(Rules.FixedIncomeHighProfits, true)]
        [TestCase(Rules.FixedIncomeHighVolumeIssuance, true)]
        [TestCase(Rules.UniverseFilter, false)]
        [TestCase(Rules.TrashAndCash, false)]
        [TestCase(Rules.PumpAndDump, false)]
        public void RuleIsLive_LiveRule_IsTrue(Rules rule, bool expectation)
        {
            var liveRulesService = new ActiveRulesService();

            var result = liveRulesService.RuleIsEnabled(rule);

            Assert.AreEqual(expectation, result);
        }

        [Test]
        public void UnLiveRules_Contains_ExpectedRules()
        {
            var liveRulesService = new ActiveRulesService();

            var result = liveRulesService.DisabledRules();

            Assert.IsTrue(result.Contains(Rules.UniverseFilter));
            Assert.IsTrue(result.Contains(Rules.PumpAndDump));
            Assert.IsTrue(result.Contains(Rules.TrashAndCash));
            Assert.IsTrue(result.Contains(Rules.CrossAssetManipulation));

            Assert.IsFalse(result.Contains(Rules.PlacingOrderWithNoIntentToExecute));
            Assert.IsFalse(result.Contains(Rules.HighVolume));
            Assert.IsFalse(result.Contains(Rules.FixedIncomeWashTrades));
            Assert.IsFalse(result.Contains(Rules.HighProfits));
            Assert.IsFalse(result.Contains(Rules.WashTrade));
            Assert.IsFalse(result.Contains(Rules.CancelledOrders));
            Assert.IsFalse(result.Contains(Rules.PaintingTheTape));
        }
    }
}