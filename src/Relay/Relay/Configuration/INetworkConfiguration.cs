namespace Relay.Configuration
{
    public interface INetworkConfiguration
    {
        string RelayServiceEquityDomain { get; set; }
        string RelayServiceEquityPort { get; set; }
        string SurveillanceServiceEquityDomain { get; set; }
        string SurveillanceServiceEquityPort { get; set; }
    }
}