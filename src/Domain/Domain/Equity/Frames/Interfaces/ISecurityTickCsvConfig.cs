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
        string SecurityTickCusipFieldName { get; set; }
        string SecurityTickExchangeSymbolFieldName { get; set; }

        string SecurityTickCfiFieldName { get; set; }
        string SecurityTickSecurityNameFieldName { get; set; }
        string SecurityTickSpreadAskFieldName { get; set; }
        string SecurityTickSpreadBidFieldName { get; set; }
        string SecurityTickSpreadPriceFieldName { get; set; }
        string SecurityTickVolumeTradedFieldName { get; set; }
        string SecurityTickMarketCapFieldName { get; set; }
        string SecurityTickCurrencyFieldName { get; set; }
        string SecurityTickListedSecuritiesFieldName { get; set; }

        string SecurityTickOpenPriceFieldName { get; set; }
        string SecurityTickClosePriceFieldName { get; set; }
        string SecurityTickHighPriceFieldName { get; set; }
        string SecurityTickLowPriceFieldName { get; set; }

        string SecurityIssuerIdentifierFieldName { get; set; }
        string SecurityLeiFieldName { get; set; }
        string SecurityBloombergTicker { get; set; }
        string SecurityDailyVolumeFieldName { get; set; }
    }
}
