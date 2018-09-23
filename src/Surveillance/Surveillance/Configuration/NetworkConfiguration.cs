using Surveillance.Configuration.Interfaces;

namespace Surveillance.Configuration
{
    public class NetworkConfiguration : INetworkConfiguration
    {
        public string SurveillanceServiceEquityDomain { get; set; }
        public string SurveillanceServiceEquityPort { get; set; }
        public string SurveillanceServiceTradeDomain { get; set; }
        public string SurveillanceServiceTradePort { get; set; }
    }
}
