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

        public struct SecurityId
        {
            public SecurityId(string id)
            {
                Id = id ?? string.Empty;
            }

            public string Id { get; }

            public override int GetHashCode()
            {
                return Id?.GetHashCode() ?? 0;
            }

            public override bool Equals(object obj)
            {
                if (!(obj is SecurityId))
                {
                    return false;
                }

                var otherId = (SecurityId)obj;

                return string.Equals(Id, otherId.Id, System.StringComparison.InvariantCultureIgnoreCase);
            }
        }
    }
}