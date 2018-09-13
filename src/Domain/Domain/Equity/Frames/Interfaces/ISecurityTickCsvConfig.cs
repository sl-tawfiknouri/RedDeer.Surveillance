namespace Domain.Equity.Frames.Interfaces
{
    public interface ISecurityTickCsvConfig
    {
        string SecurityTickTimestampFieldName { get; set; }
        string SecurityTickMarketIdentifierCodeFieldName { get; set; }
        string SecurityTickMarketNameFieldName { get; set; }
        string SecurityTickClientIdentifierFieldName { get; set; }
        string SecurityTickSedolFieldName { get; set; }
        string SecurityTickIsinFieldName { get; set; }
        string SecurityTickFigiFieldName { get; set; }
        string SecurityTickCfiFieldName { get; set; }
        string SecurityTickTickerSymbolFieldName { get; set; }
        string SecurityTickSecurityNameFieldName { get; set; }
        string SecurityTickSpreadAskFieldName { get; set; }
        string SecurityTickSpreadBidFieldName { get; set; }
        string SecurityTickSpreadPriceFieldName { get; set; }
        string SecurityTickVolumeTradedFieldName { get; set; }
        string SecurityTickMarketCapFieldName { get; set; }
        string SecurityTickSpreadCurrencyFieldName { get; set; }
    }
}
