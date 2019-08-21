namespace Domain.Core.Financial.Assets
{
    using Domain.Core.Financial.Assets.Interfaces;

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
            this.Type = types;
            this.Identifiers = identifiers;
            this.Name = name;
            this.Cfi = cfi;
            this.SecurityCurrency = securityCurrency;
            this.IssuerIdentifier = issuerIdentifier;
            this.UnderlyingName = underlyingName;
            this.UnderlyingCfi = underlyingCfi;
            this.UnderlyingIssuerIdentifier = underlyingIssuerIdentifier;
            this.SectorCode = sectorCode;
            this.IndustryCode = industryCode;
            this.RegionCode = regionCode;
            this.CountryCode = countryCode;
        }

        public FinancialInstrument(
            InstrumentTypes types,
            InstrumentIdentifiers identifiers,
            string name,
            string cfi,
            string securityCurrency,
            string issuerIdentifier)
        {
            this.Type = types;
            this.Identifiers = identifiers;
            this.Name = name;
            this.Cfi = cfi;
            this.SecurityCurrency = securityCurrency;
            this.IssuerIdentifier = issuerIdentifier;
            this.UnderlyingName = string.Empty;
            this.UnderlyingCfi = string.Empty;
            this.UnderlyingIssuerIdentifier = string.Empty;
            this.SectorCode = string.Empty;
            this.IndustryCode = string.Empty;
            this.RegionCode = string.Empty;
            this.CountryCode = string.Empty;
        }

        /// <summary>
        ///     Classification Financial Instrument code
        /// </summary>
        public string Cfi { get; set; }

        public string CountryCode { get; set; }

        public InstrumentIdentifiers Identifiers { get; set; }

        public string IndustryCode { get; set; }

        public string IssuerIdentifier { get; set; }

        /// <summary>
        ///     Name of the instrument
        /// </summary>
        public string Name { get; set; }

        public string RegionCode { get; set; }

        // reference data
        public string SectorCode { get; set; }

        public string SecurityCurrency { get; set; }

        public InstrumentTypes Type { get; set; }

        public string UnderlyingCfi { get; set; }

        public string UnderlyingIssuerIdentifier { get; set; }

        // derivatives
        public string UnderlyingName { get; set; }
    }
}