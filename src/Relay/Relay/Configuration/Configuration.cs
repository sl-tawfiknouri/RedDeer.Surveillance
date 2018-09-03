namespace Relay.Configuration
{
    public class Configuration : INetworkConfiguration
    {
        public string SurveillanceServiceEquityDomain { get; set; }
        public string SurveillanceServiceEquityPort { get; set; }
        public string RelayServiceEquityDomain { get; set; }
        public string RelayServiceEquityPort { get; set; }
    }
}
