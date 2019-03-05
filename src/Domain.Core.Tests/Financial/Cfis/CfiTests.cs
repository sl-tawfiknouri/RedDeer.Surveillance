using Domain.Core.Financial.Cfis;
using NUnit.Framework;

namespace Domain.Tests.Financial.Cfis
{
    [TestFixture]
    public class CfiTests
    {
        [TestCase("", CfiCategory.None, CfiGroup.None)]
        [TestCase("e", CfiCategory.Equities, CfiGroup.None)]
        [TestCase("esnspb", CfiCategory.Equities, CfiGroup.Shares)]
        [TestCase("epsspb", CfiCategory.Equities, CfiGroup.PreferredShares)]
        [TestCase("ernspb", CfiCategory.Equities, CfiGroup.PreferenceShares)]
        [TestCase("ecnspb", CfiCategory.Equities, CfiGroup.ConvertibleShares)]
        [TestCase("efnspb", CfiCategory.Equities, CfiGroup.PreferredConvertibleShares)]
        [TestCase("evnspb", CfiCategory.Equities, CfiGroup.PreferenceConvertibleShares)]
        [TestCase("eunspb", CfiCategory.Equities, CfiGroup.Units)]
        [TestCase("emnspb", CfiCategory.Equities, CfiGroup.Others)]
        [TestCase("d", CfiCategory.DebtInstrument, CfiGroup.None)]
        [TestCase("db", CfiCategory.DebtInstrument, CfiGroup.Bonds)]
        [TestCase("dcmlo", CfiCategory.DebtInstrument, CfiGroup.ConvertibleBonds)]
        [TestCase("dm", CfiCategory.DebtInstrument, CfiGroup.Others)]
        [TestCase("dt", CfiCategory.DebtInstrument, CfiGroup.MediumTermNotes)]
        [TestCase("dw", CfiCategory.DebtInstrument, CfiGroup.BondsWithWarrantsAttached)]
        [TestCase("dy", CfiCategory.DebtInstrument, CfiGroup.MoneyMarketInstruments)]
        [TestCase("m", CfiCategory.Others, CfiGroup.None)]
        [TestCase("mm", CfiCategory.Others, CfiGroup.Others)]
        [TestCase("mr", CfiCategory.Others, CfiGroup.ReferentialInstruments)]
        [TestCase("f", CfiCategory.Futures, CfiGroup.None)]
        [TestCase("fc", CfiCategory.Futures, CfiGroup.CommoditiesFutures)]
        [TestCase("ff", CfiCategory.Futures, CfiGroup.FinancialFutures)]
        [TestCase("o", CfiCategory.Options, CfiGroup.None)]
        [TestCase("oc", CfiCategory.Options, CfiGroup.CallOptions)]
        [TestCase("op", CfiCategory.Options, CfiGroup.PutOptions)]
        [TestCase("om", CfiCategory.Options, CfiGroup.Others)]
        [TestCase("r", CfiCategory.Entitlements, CfiGroup.None)]
        [TestCase("ra", CfiCategory.Entitlements, CfiGroup.AllotmentRights)]
        [TestCase("rm", CfiCategory.Entitlements, CfiGroup.Others)]
        [TestCase("rs", CfiCategory.Entitlements, CfiGroup.SubscriptionRights)]
        [TestCase("rw", CfiCategory.Entitlements, CfiGroup.Warrants)]
        public void Cfi_Correct_Result(string cfi, CfiCategory category, CfiGroup group)
        {
            var cfiItem = new Cfi(cfi);

            Assert.AreEqual(cfiItem.Value, cfi);
            Assert.AreEqual(cfiItem.CfiCategory, category);
            Assert.AreEqual(cfiItem.CfiGroup, group);
        }
    }
}
