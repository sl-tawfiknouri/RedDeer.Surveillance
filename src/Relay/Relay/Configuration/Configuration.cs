using Domain.Trades.Orders.Interfaces;
using Relay.Configuration.Interfaces;

namespace Relay.Configuration
{
    public class Configuration : INetworkConfiguration, IUploadConfiguration, ITradeOrderCsvConfig
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
        public string StatusChangedOnFieldName { get; set; }
        public string MarketIdFieldName { get; set; }
        public string MarketAbbreviationFieldName { get; set; }
        public string MarketNameFieldName { get; set; }
        public string SecurityIdFieldName { get; set; }
        public string SecurityNameFieldName { get; set; }
        public string OrderTypeFieldName { get; set; }
        public string OrderDirectionFieldName { get; set; }
        public string OrderStatusFieldName { get; set; }
        public string VolumeFieldName { get; set; }
        public string LimitPriceFieldName { get; set; }
    }
}
