using Relay.Configuration.Interfaces;

namespace Relay.Configuration
{
    public class Configuration : INetworkConfiguration, IUploadConfiguration
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
    }
}
