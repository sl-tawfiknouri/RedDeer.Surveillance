using Domain.Core.Financial.Assets;
using NUnit.Framework;

namespace Domain.Core.Tests.Financial.Assets
{
    [TestFixture]
    public class InstrumentIdentifiersTests
    {
        [Test]
        public void Null_IsNotEqualTo_Null()
        {
            var nullOne = InstrumentIdentifiers.Null();
            var nullTwo = InstrumentIdentifiers.Null();

            Assert.AreNotEqual(nullOne, nullTwo);
        }

        [Test]
        public void InstrumentIdentifiers_PopulatesCorrectFieldsFrom_Arguments()
        {
            var id = "ii-id";
            var reddeerId = "red-id";
            var reddeerEnrichmentId = "red-enrich-id";
            var clientIdentifier = "client-id";
            var sedol = "sedol";
            var isin = "isin";
            var figi = "figi";
            var cusip = "cusip";
            var exchangeSymbol = "exch symbol";
            var lei = "lei";
            var bloombergTicker = "bloomberg";
            var underlyingSedol = "u-sedol";
            var underlyingIsin = "u-isin";
            var underlyingFigi = "u-figi";
            var underlyingCusip = "u-cusip";
            var underlyingLei = "u-lei";
            var underlyingExchangeSymbol = "u-exch";
            var underlyingBloombergTicker = "u-bloomberg";
            var underlyingClientIdentifier = "u-client";

            var instrumentIdentifiers = new InstrumentIdentifiers(
                id,
                reddeerId,
                reddeerEnrichmentId,
                clientIdentifier,
                sedol,
                isin,
                figi,
                cusip,
                exchangeSymbol,
                lei,
                bloombergTicker,
                underlyingSedol,
                underlyingIsin,
                underlyingFigi,
                underlyingCusip,
                underlyingLei,
                underlyingExchangeSymbol,
                underlyingBloombergTicker,
                underlyingClientIdentifier);

            Assert.AreEqual(instrumentIdentifiers.Id, id);
            Assert.AreEqual(instrumentIdentifiers.ReddeerId, reddeerId);
            Assert.AreEqual(instrumentIdentifiers.ReddeerEnrichmentId, reddeerEnrichmentId);
            Assert.AreEqual(instrumentIdentifiers.ClientIdentifier, clientIdentifier);
            Assert.AreEqual(instrumentIdentifiers.Sedol, sedol);
            Assert.AreEqual(instrumentIdentifiers.Isin, isin);
            Assert.AreEqual(instrumentIdentifiers.Figi, figi);
            Assert.AreEqual(instrumentIdentifiers.Cusip, cusip);
            Assert.AreEqual(instrumentIdentifiers.ExchangeSymbol, exchangeSymbol);
            Assert.AreEqual(instrumentIdentifiers.Lei, lei);
            Assert.AreEqual(instrumentIdentifiers.BloombergTicker, bloombergTicker);
            Assert.AreEqual(instrumentIdentifiers.UnderlyingSedol, underlyingSedol);
            Assert.AreEqual(instrumentIdentifiers.UnderlyingIsin, underlyingIsin);
            Assert.AreEqual(instrumentIdentifiers.UnderlyingFigi, underlyingFigi);
            Assert.AreEqual(instrumentIdentifiers.UnderlyingCusip, underlyingCusip);
            Assert.AreEqual(instrumentIdentifiers.UnderlyingLei, underlyingLei);

            Assert.AreEqual(instrumentIdentifiers.UnderlyingExchangeSymbol, underlyingExchangeSymbol);
            Assert.AreEqual(instrumentIdentifiers.UnderlyingBloombergTicker, underlyingBloombergTicker);
            Assert.AreEqual(instrumentIdentifiers.UnderlyingClientIdentifier, underlyingClientIdentifier);
        }

        [TestCase]
        public void GetHashCode_EqualForSame_ReddeerId()
        {
            var identifiers1 = InstrumentIdentifiers.Null();
            identifiers1.ReddeerId = "abc";
            var hashCode1 = identifiers1.GetHashCode();

            var identifiers2 = InstrumentIdentifiers.Null();
            identifiers2.ReddeerId = "abc";
            var hashCode2 = identifiers2.GetHashCode();

            Assert.AreEqual(hashCode1, hashCode2);
        }

        [TestCase]
        public void GetHashCode_EqualForNull_ReddeerId()
        {
            var identifiers1 = InstrumentIdentifiers.Null();
            identifiers1.ReddeerId = null;
            var hashCode1 = identifiers1.GetHashCode();

            var identifiers2 = InstrumentIdentifiers.Null();
            identifiers2.ReddeerId = string.Empty;
            var hashCode2 = identifiers2.GetHashCode();

            Assert.AreEqual(hashCode1, hashCode2);
        }

        [TestCase]
        public void GetHashCode_NotEqualForDifferent_ReddeerId()
        {
            var identifiers1 = InstrumentIdentifiers.Null();
            identifiers1.ReddeerId = "abc";
            var hashCode1 = identifiers1.GetHashCode();

            var identifiers2 = InstrumentIdentifiers.Null();
            identifiers2.ReddeerId = "def";
            var hashCode2 = identifiers2.GetHashCode();

            Assert.AreNotEqual(hashCode1, hashCode2);
        }

        [TestCase]
        public void GetHashCode_EqualForDifferentCased_ReddeerId()
        {
            var identifiers1 = InstrumentIdentifiers.Null();
            identifiers1.ReddeerId = "abc";
            var hashCode1 = identifiers1.GetHashCode();

            var identifiers2 = InstrumentIdentifiers.Null();
            identifiers2.ReddeerId = "aBc";
            var hashCode2 = identifiers2.GetHashCode();

            Assert.AreEqual(hashCode1, hashCode2);
        }

        [TestCase]
        public void ToString_ReturnsExpectedString()
        {
            var id = "ii-id";
            var reddeerId = "red-id";
            var reddeerEnrichmentId = "red-enrich-id";
            var clientIdentifier = "client-id";
            var sedol = "sedol";
            var isin = "isin";
            var figi = "figi";
            var cusip = "cusip";
            var exchangeSymbol = "exch symbol";
            var lei = "lei";
            var bloombergTicker = "bloomberg";
            var underlyingSedol = "u-sedol";
            var underlyingIsin = "u-isin";
            var underlyingFigi = "u-figi";
            var underlyingCusip = "u-cusip";
            var underlyingLei = "u-lei";
            var underlyingExchangeSymbol = "u-exch";
            var underlyingBloombergTicker = "u-bloomberg";
            var underlyingClientIdentifier = "u-client";

            var instrumentIdentifiers = new InstrumentIdentifiers(
                id,
                reddeerId,
                reddeerEnrichmentId,
                clientIdentifier,
                sedol,
                isin,
                figi,
                cusip,
                exchangeSymbol,
                lei,
                bloombergTicker,
                underlyingSedol,
                underlyingIsin,
                underlyingFigi,
                underlyingCusip,
                underlyingLei,
                underlyingExchangeSymbol,
                underlyingBloombergTicker,
                underlyingClientIdentifier);

            var outputStr = instrumentIdentifiers.ToString();

            Assert.AreEqual(outputStr, "Client Id: client-id | Sedol sedol | Isin isin | Figi figi | Cusip cusip | Exchange Symbol exch symbol | Lei lei | Bloomberg Ticker bloomberg");
        }

        [TestCase("", "client-1", "", "", "", "", "", false)]
        [TestCase("", "client-id", "", "", "", "", "", true)]
        [TestCase("", "", "sedol-1", "", "", "", "", false)]
        [TestCase("", "", "sedol", "", "", "", "", true)]
        [TestCase("", "client-9", "sedol", "", "", "", "", true)]
        [TestCase("", "", "", "figi-1", "", "", "", false)]
        [TestCase("", "", "", "figi", "", "", "", true)]
        [TestCase("", "", "", "", "cusip-1", "", "", false)]
        [TestCase("", "", "", "", "cusip", "", "", true)]
        [TestCase("", "", "", "", "", "isin-1", "", false)]
        [TestCase("", "", "", "", "", "isin", "", true)]
        [TestCase("", "", "sedol", "", "", "isin-3", "", true)]
        [TestCase("", "", "", "", "", "", "bloomberg-1", false)]
        [TestCase("", "", "", "", "", "", "bloomberg", true)]
        public void Equals_ReturnsTrueFor_MatchingSecurities(
            string reddeeridArg,
            string clientidArg,
            string sedolArg,
            string figiArg,
            string cusipArg,
            string isinArg,
            string bloombergArg,
            bool isMatch)
        {
            var id = "ii-id";
            var reddeerId = "red-id";
            var reddeerEnrichmentId = "red-enrich-id";
            var clientIdentifier = "client-id";
            var sedol = "sedol";
            var isin = "isin";
            var figi = "figi";
            var cusip = "cusip";
            var exchangeSymbol = "exch symbol";
            var lei = "lei";
            var bloombergTicker = "bloomberg";

            var instrumentIdentifiers = new InstrumentIdentifiers(
                id,
                reddeerId,
                reddeerEnrichmentId,
                clientIdentifier,
                sedol,
                isin,
                figi,
                cusip,
                exchangeSymbol,
                lei,
                bloombergTicker);

            var instrumentIdentifiers2 = new InstrumentIdentifiers(
                id,
                reddeeridArg,
                "some-other-reddeer-enrichment-id",
                clientidArg,
                sedolArg,
                isinArg,
                figiArg,
                cusipArg,
                "some-other-exchange-symbol",
                "some-other-lei",
                bloombergArg);

            if (isMatch)
            {
                Assert.AreEqual(instrumentIdentifiers, instrumentIdentifiers2);
            }
            else
            {
                Assert.AreNotEqual(instrumentIdentifiers, instrumentIdentifiers2);
            }
        }
    }
}
