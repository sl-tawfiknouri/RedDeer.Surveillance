namespace Domain.Core.Tests.Financial.Assets
{
    using Domain.Core.Financial.Assets;

    using NUnit.Framework;

    [TestFixture]
    public class FinancialInstrumentTests
    {
        /// <summary>
        ///     Not a great test but this is a value type (DDD) i.e. just holds data
        /// </summary>
        [Test]
        public void FinancialInstruments_Ctor_AssignsValuesCorrectly()
        {
            var instrument = InstrumentTypes.Bond;
            var identifiers = InstrumentIdentifiers.Null();
            var name = "test-instrument";
            var cfi = "a-cfi";
            var currency = "GBP";
            var issuerId = "ISO-1000";
            var underlyingName = "u-name";
            var underlyingCfi = "u-cfi";
            var underlyingIssuerId = "u-issuer-id";
            var sectorCode = "sect-100";
            var indCode = "ind-100";
            var regCode = "reg-100";
            var countryCode = "count-100";

            var fi = new FinancialInstrument(
                instrument,
                identifiers,
                name,
                cfi,
                currency,
                issuerId,
                underlyingName,
                underlyingCfi,
                underlyingIssuerId,
                sectorCode,
                indCode,
                regCode,
                countryCode);

            Assert.AreEqual(fi.Type, instrument);

            Assert.AreNotEqual(fi.Identifiers, identifiers);
            Assert.AreEqual(fi.Name, name);
            Assert.AreEqual(fi.Cfi, cfi);
            Assert.AreEqual(fi.SecurityCurrency, currency);
            Assert.AreEqual(fi.IssuerIdentifier, issuerId);

            Assert.AreEqual(fi.UnderlyingName, underlyingName);
            Assert.AreEqual(fi.UnderlyingCfi, underlyingCfi);
            Assert.AreEqual(fi.UnderlyingIssuerIdentifier, underlyingIssuerId);
            Assert.AreEqual(fi.SectorCode, sectorCode);
            Assert.AreEqual(fi.IndustryCode, indCode);
            Assert.AreEqual(fi.RegionCode, regCode);
            Assert.AreEqual(fi.CountryCode, countryCode);
        }

        [Test]
        public void FinancialInstruments_MustHaveA_ParameterLessConstructor()
        {
            // do not remove this

            // ReSharper disable once ObjectCreationAsStatement
            Assert.DoesNotThrow(() => new FinancialInstrument());
        }
    }
}