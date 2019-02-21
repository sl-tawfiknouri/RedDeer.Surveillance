using Domain.Financial.Interfaces;

namespace Domain.Financial
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
            string underlyingIssuerIdentifier)
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
    }
}
