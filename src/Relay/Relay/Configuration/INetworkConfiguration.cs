namespace Relay.Configuration
{
    public interface INetworkConfiguration
    {
        string RelayServiceEquityDomain { get; set; }
        string RelayServiceEquityPort { get; set; }
        string SurveillanceServiceEquityDomain { get; set; }
        string SurveillanceServiceEquityPort { get; set; }
        string SurveillanceServiceTradeDomain { get; set; }
        string SurveillanceServiceTradePort { get; set; }
        string RelayServiceTradeDomain { get; set; }
        string RelayServiceTradePort { get; set; }
    }
}