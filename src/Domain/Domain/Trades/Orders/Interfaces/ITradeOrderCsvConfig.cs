namespace Domain.Trades.Orders.Interfaces
{
    public interface ITradeOrderCsvConfig
    {
        string StatusChangedOnFieldName { get; }
        string MarketIdFieldName { get; }
        string MarketAbbreviationFieldName { get; }
        string MarketNameFieldName { get; }
        string SecurityClientIdentifierFieldName { get; }
        string SecuritySedolFieldName { get; }
        string SecurityIsinFieldName { get; }
        string SecurityFigiFieldName { get; }
        string SecurityNameFieldName { get; }
        string OrderTypeFieldName { get; }
        string OrderDirectionFieldName { get; }
        string OrderStatusFieldName { get; }
        string VolumeFieldName { get; }
        string LimitPriceFieldName { get; }
    }
}
