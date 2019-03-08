using System.Linq;
using Domain.Surveillance.Rules;
using NUnit.Framework;

namespace Domain.Surveillance.Tests.Rules
{
    [TestFixture]
    public class LiveRulesServiceTests
    {
        [TestCase(Scheduling.Rules.HighVolume, true)]
        [TestCase(Scheduling.Rules.CancelledOrders, true)]
        [TestCase(Scheduling.Rules.WashTrade, true)]
        [TestCase(Scheduling.Rules.FixedIncomeHighProfits, true)]
        [TestCase(Scheduling.Rules.FixedIncomeHighVolumeIssuance, true)]
        [TestCase(Scheduling.Rules.UniverseFilter, false)]
        [TestCase(Scheduling.Rules.TrashAndCash, false)]
        [TestCase(Scheduling.Rules.PumpAndDump, false)]
        public void RuleIsLive_LiveRule_IsTrue(Scheduling.Rules rule, bool expectation)
        {
            var liveRulesService = new ActiveRulesService();

            var result = liveRulesService.RuleIsEnabled(rule);

            Assert.AreEqual(expectation, result);
        }

        [Test]
        public void LiveRules_Contains_ExpectedRules()
        {
            var liveRulesService = new ActiveRulesService();

            var result = liveRulesService.EnabledRules();

            Assert.IsTrue(result.Contains(Scheduling.Rules.HighVolume));
            Assert.IsTrue(result.Contains(Scheduling.Rules.FixedIncomeWashTrades));
            Assert.IsTrue(result.Contains(Scheduling.Rules.HighProfits));
            Assert.IsTrue(result.Contains(Scheduling.Rules.WashTrade));
            Assert.IsTrue(result.Contains(Scheduling.Rules.CancelledOrders));

            Assert.IsFalse(result.Contains(Scheduling.Rules.UniverseFilter));
            Assert.IsFalse(result.Contains(Scheduling.Rules.PumpAndDump));
            Assert.IsFalse(result.Contains(Scheduling.Rules.TrashAndCash));
            Assert.IsFalse(result.Contains(Scheduling.Rules.CrossAssetManipulation));
            Assert.IsFalse(result.Contains(Scheduling.Rules.PaintingTheTape));
        }

        [Test]
        public void UnLiveRules_Contains_ExpectedRules()
        {
            var liveRulesService = new ActiveRulesService();

            var result = liveRulesService.DisabledRules();

            Assert.IsTrue(result.Contains(Scheduling.Rules.UniverseFilter));
            Assert.IsTrue(result.Contains(Scheduling.Rules.PumpAndDump));
            Assert.IsTrue(result.Contains(Scheduling.Rules.TrashAndCash));
            Assert.IsTrue(result.Contains(Scheduling.Rules.CrossAssetManipulation));
            Assert.IsTrue(result.Contains(Scheduling.Rules.PaintingTheTape));

            Assert.IsFalse(result.Contains(Scheduling.Rules.HighVolume));
            Assert.IsFalse(result.Contains(Scheduling.Rules.FixedIncomeWashTrades));
            Assert.IsFalse(result.Contains(Scheduling.Rules.HighProfits));
            Assert.IsFalse(result.Contains(Scheduling.Rules.WashTrade));
            Assert.IsFalse(result.Contains(Scheduling.Rules.CancelledOrders));
        }
    }
}
