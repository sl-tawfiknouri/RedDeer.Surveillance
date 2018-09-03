namespace Surveillance.Configuration.Interfaces
{
    public interface INetworkConfiguration
    {
        string SurveillanceServiceEquityDomain { get; set; }
        string SurveillanceServiceEquityPort { get; set; }
        string SurveillanceServiceTradeDomain { get; set; }
        string SurveillanceServiceTradePort { get; set; }
    }
}