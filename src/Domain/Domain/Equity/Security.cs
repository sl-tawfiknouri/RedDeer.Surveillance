namespace Domain.Equity
{
    /// <summary>
    /// A fungible, negotiable financial instrument
    /// </summary>
    public class Security
    {
        public Security(
            SecurityIdentifiers identifiers,
            string name)
        {
            Identifiers = identifiers;
            Name = name ?? string.Empty;
        }

        /// <summary>
        /// FIGI | ISIN | SEDOL | Client Identifier
        /// </summary>
        public SecurityIdentifiers Identifiers { get; }

        /// <summary>
        /// Long form name
        /// </summary>
        public string Name { get; }
    }
}