namespace Domain.Equity
{
    /// <summary>
    /// A typed decimal for the price of an asset
    /// </summary>
    public struct Price
    {
        public Price(decimal value)
        {
            Value = value;
        }

        public decimal Value { get; }
    }
}
