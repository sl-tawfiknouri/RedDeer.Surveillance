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
        public string StatusChangedOnFieldName { get; set; }
        public string MarketIdFieldName { get; set; }
        public string MarketAbbreviationFieldName { get; set; }
        public string MarketNameFieldName { get; set; }
        public string SecurityNameFieldName { get; set; }
        public string OrderTypeFieldName { get; set; }
        public string OrderDirectionFieldName { get; set; }
        public string OrderStatusFieldName { get; set; }
        public string VolumeFieldName { get; set; }
        public string LimitPriceFieldName { get; set; }

        public string SecurityClientIdentifierFieldName { get; set; }
        public string SecuritySedolFieldName { get; set; }
        public string SecurityIsinFieldName { get; set; }
        public string SecurityFigiFieldName { get; set; }
        
        public string SecurityTickTimestampFieldName { get; set; }
        public string SecurityTickMarketIdentifierCodeFieldName { get; set; }
        public string SecurityTickMarketNameFieldName { get; set; }
        public string SecurityTickClientIdentifierFieldName { get; set; }
        public string SecurityTickSedolFieldName { get; set; }
        public string SecurityTickIsinFieldName { get; set; }
        public string SecurityTickFigiFieldName { get; set; }
        public string SecurityTickCfiFieldName { get; set; }
        public string SecurityTickTickerSymbolFieldName { get; set; }
        public string SecurityTickSecurityNameFieldName { get; set; }
        public string SecurityTickSpreadAskFieldName { get; set; }
        public string SecurityTickSpreadBidFieldName { get; set; }
        public string SecurityTickSpreadPriceFieldName { get; set; }
        public string SecurityTickVolumeTradedFieldName { get; set; }
        public string SecurityTickMarketCapFieldName { get; set; }
        public string SecurityTickSpreadCurrencyFieldName { get; set; }
    }
}
