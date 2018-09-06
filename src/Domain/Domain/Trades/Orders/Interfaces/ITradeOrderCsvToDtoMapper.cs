namespace Domain.Trades.Orders.Interfaces
{
    public interface ITradeOrderCsvToDtoMapper
    {
        int FailedParseTotal { get; set; }

        TradeOrderFrame Map(TradeOrderFrameCsv csv);
    }
}