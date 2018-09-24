namespace Domain.Trades.Orders.Interfaces
{
    public interface ITradeOrderCsvConfig
    {
        string OrderTypeFieldName { get; set; }


        string MarketIdentifierCodeFieldName { get; set; }
        string MarketNameFieldName { get; set; }


        string SecurityNameFieldName { get; set; }
        string SecurityCfiFieldName { get; set; }

        string SecurityClientIdentifierFieldName { get; set; }
        string SecuritySedolFieldName { get; set; }
        string SecurityIsinFieldName { get; set; }
        string SecurityFigiFieldName { get; set; }
        string SecurityCusipFieldName { get; set; }
        string SecurityExchangeSymbolFieldName { get; set; }

        string LimitPriceFieldName { get; set; }
        string TradeSubmittedOnFieldName { get; set; }
        string StatusChangedOnFieldName { get; set; }
        string FulfilledVolumeFieldName { get; set; }
        string OrderPositionFieldName { get; set; }

        string TraderIdFieldName { get; set; }
        string TraderClientAttributionIdFieldName { get; set; }
        string PartyBrokerIdFieldName { get; set; }
        string CounterPartyBrokerIdFieldName { get; set; }

        string OrderStatusFieldName { get; set; }
        string CurrencyFieldName { get; set; }

        string SecurityLei { get; set; }
        string SecurityBloombergTickerFieldName { get; set; }
        string ExecutedPriceFieldName { get; set; }
        string OrderedVolumeFieldName { get; set; }
        string AccountIdFieldName { get; set; }
        string DealerInstructionsFieldName { get; set; }
        string TradeRationaleFieldName { get; set; }
        string TradeStrategyFieldName { get; set;}
        string SecurityIssuerIdentifier { get; set; }
    }
}
