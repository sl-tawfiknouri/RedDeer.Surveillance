namespace Domain.Equity
{
    /// <summary>
    /// A fungible, negotiable financial instrument
    /// </summary>
    public class Security
    {
        public Security(SecurityId id, string name, string exchangeAbbr)
        {
            Id = id;
            Name = name ?? string.Empty;
            ExchangeAbbr = exchangeAbbr ?? string.Empty;
        }

        public SecurityId Id { get; }

        /// <summary>
        /// Long form name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// MSFT / STAN / ...
        /// </summary>
        public string ExchangeAbbr { get; }

        public class SecurityId
        {
            public SecurityId(string id)
            {
                Id = id ?? string.Empty;
            }

            public string Id { get; }
        }
    }
}