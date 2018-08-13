namespace Domain.Equity.Market
{
    /// <summary>
    /// Stock exchanges LSE
    /// </summary>
    public class StockExchange : Market
    {
        public StockExchange(MarketId id, string name) 
            : base(id, name)
        {
        }
    }
}
