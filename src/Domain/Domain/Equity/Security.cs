namespace Domain.Equity
{
    /// <summary>
    /// A fungible, negotiable financial instrument.
    /// The security properties are not considered time dependent
    /// </summary>
    public class Security
    {
        public Security(
            SecurityIdentifiers identifiers,
            string name,
            string cfi,
            string issuerIdentifier)
        {
            Identifiers = identifiers;
            Name = name ?? string.Empty;
            Cfi = cfi ?? string.Empty;
            IssuerIdentifier = issuerIdentifier ?? string.Empty;
        }

        /// <summary>
        /// Exchange Symbol | CUSIP | FIGI | ISIN | SEDOL | Client Identifier | LEI | Bloomberg Ticker
        /// </summary>
        public SecurityIdentifiers Identifiers { get; }

        /// <summary>
        /// Long form name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Classification of Financial Instruments codes (norm ISO 10962:2015)
        /// This describes an asset in terms such as BOND|EQUITY VOTING STOCK
        /// https://en.wikipedia.org/wiki/ISO_10962
        /// </summary>
        public string Cfi { get; }

        /// <summary>
        /// An identifier for the organisation issuing this security.
        /// </summary>
        public string IssuerIdentifier { get; }
    }
}