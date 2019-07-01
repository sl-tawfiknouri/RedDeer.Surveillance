using Domain.Core.Financial.Assets.Interfaces;

namespace Domain.Core.Financial.Assets
{
    public class FinancialInstrument : IFinancialInstrument
    {
        public FinancialInstrument()
        {
            // used for deserialisation
        }

        public FinancialInstrument(
            InstrumentTypes types,
            InstrumentIdentifiers identifiers,
            string name,
            string cfi,
            string securityCurrency,
            string issuerIdentifier,
            string underlyingName,
            string underlyingCfi,
            string underlyingIssuerIdentifier,
            string sectorCode,
            string industryCode,
            string regionCode,
            string countryCode)
        {
            Type = types;
            Identifiers = identifiers;
            Name = name;
            Cfi = cfi;
            SecurityCurrency = securityCurrency;
            IssuerIdentifier = issuerIdentifier;
            UnderlyingName = underlyingName;
            UnderlyingCfi = underlyingCfi;
            UnderlyingIssuerIdentifier = underlyingIssuerIdentifier;
            SectorCode = sectorCode;
            IndustryCode = industryCode;
            RegionCode = regionCode;
            CountryCode = countryCode;
        }

        public FinancialInstrument(
            InstrumentTypes types,
            InstrumentIdentifiers identifiers,
            string name,
            string cfi,
            string securityCurrency,
            string issuerIdentifier)
        {
            Type = types;
            Identifiers = identifiers;
            Name = name;
            Cfi = cfi;
            SecurityCurrency = securityCurrency;
            IssuerIdentifier = issuerIdentifier;
            UnderlyingName = string.Empty;
            UnderlyingCfi = string.Empty;
            UnderlyingIssuerIdentifier = string.Empty;
            SectorCode = string.Empty;
            IndustryCode = string.Empty;
            RegionCode = string.Empty;
            CountryCode = string.Empty;
        }

        public InstrumentTypes Type { get; set; }

        public InstrumentIdentifiers Identifiers { get; set; }

        /// <summary>
        /// Name of the instrument
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Classification Financial Instrument code
        /// </summary>
        public string Cfi { get; set; }

        public string SecurityCurrency { get; set; }

        public string IssuerIdentifier { get; set; }

        // derivatives
        public string UnderlyingName { get; set; }
        public string UnderlyingCfi { get; set; }
        public string UnderlyingIssuerIdentifier { get; set; }

        // reference data
        public string SectorCode { get; set; }
        public string IndustryCode { get; set; }
        public string RegionCode { get; set; }
        public string CountryCode { get; set; }
    }
}
