using Domain.Equity.Frames.Interfaces;
using Domain.Trades.Orders.Interfaces;
using Relay.Configuration.Interfaces;

namespace Relay.Configuration
{
    public class Configuration : INetworkConfiguration, IUploadConfiguration, ITradeOrderCsvConfig, ISecurityTickCsvConfig
    {
        public string SurveillanceServiceEquityDomain { get; set; }
        public string SurveillanceServiceEquityPort { get; set; }
        public string RelayServiceEquityDomain { get; set; }
        public string RelayServiceEquityPort { get; set; }

        public string SurveillanceServiceTradeDomain { get; set; }
        public string SurveillanceServiceTradePort { get; set; }
        public string RelayServiceTradeDomain { get; set; }
        public string RelayServiceTradePort { get; set; }
        public string RelayTradeFileUploadDirectoryPath { get; set; }
        public string RelayEquityFileUploadDirectoryPath { get; set; }

        // Security TRADE config
        public string OrderTypeFieldName { get; set; }


        public string MarketIdentifierCodeFieldName { get; set; }
        public string MarketNameFieldName { get; set; }


        public string SecurityNameFieldName { get; set; }
        public string SecurityCfiFieldName { get; set; }

        public string SecurityClientIdentifierFieldName { get; set; }
        public string SecuritySedolFieldName { get; set; }
        public string SecurityIsinFieldName { get; set; }
        public string SecurityFigiFieldName { get; set; }
        public string SecurityCusipFieldName { get; set; }
        public string SecurityExchangeSymbolFieldName { get; set; }

        public string LimitPriceFieldName { get; set; }
        public string TradeSubmittedOnFieldName { get; set; }
        public string StatusChangedOnFieldName { get; set; }
        public string VolumeFieldName { get; set; }
        public string OrderPositionFieldName { get; set; }

        public string TraderIdFieldName { get; set; }
        public string TraderClientAttributionIdFieldName { get; set; }
        public string PartyBrokerIdFieldName { get; set; }
        public string CounterPartyBrokerIdFieldName { get; set; }
        
        public string OrderStatusFieldName { get; set; }
        public string CurrencyFieldName { get; set; }



        // Security TICK config
        public string SecurityTickTimestampFieldName { get; set; }
        public string SecurityTickMarketIdentifierCodeFieldName { get; set; }
        public string SecurityTickMarketNameFieldName { get; set; }

        public string SecurityTickClientIdentifierFieldName { get; set; }
        public string SecurityTickSedolFieldName { get; set; }
        public string SecurityTickIsinFieldName { get; set; }
        public string SecurityTickFigiFieldName { get; set; }
        public string SecurityTickCusipFieldName { get; set; }

        public string SecurityTickCfiFieldName { get; set; }
        public string SecurityTickExchangeSymbolFieldName { get; set; }
        public string SecurityTickSecurityNameFieldName { get; set; }
        public string SecurityTickSpreadAskFieldName { get; set; }
        public string SecurityTickSpreadBidFieldName { get; set; }
        public string SecurityTickSpreadPriceFieldName { get; set; }
        public string SecurityTickVolumeTradedFieldName { get; set; }
        public string SecurityTickMarketCapFieldName { get; set; }
        public string SecurityTickCurrencyFieldName { get; set; }
        public string SecurityTickListedSecuritiesFieldName { get; set; }

        public string SecurityTickOpenPriceFieldName { get; set; }
        public string SecurityTickClosePriceFieldName { get; set; }
        public string SecurityTickHighPriceFieldName { get; set; }
        public string SecurityTickLowPriceFieldName { get; set; }
    }
}
