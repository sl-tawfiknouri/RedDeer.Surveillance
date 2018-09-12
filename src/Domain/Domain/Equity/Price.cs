namespace Domain.Equity
{
    /// <summary>
    /// A typed decimal for the price of an asset
    /// </summary>
    public struct Price
    {
        public Price(decimal value, string currency)
        {
            Value = value;
            Currency = currency ?? string.Empty;
        }

        public decimal Value { get; }

        /// <summary>
        /// ISO 4217 currency codes | GBP | GBX
        /// </summary>
        public string Currency { get; }
    }
}
