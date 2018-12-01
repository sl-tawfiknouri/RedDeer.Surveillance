using Surveillance.System.DataLayer.Interfaces;

namespace DataImport.Configuration.Interfaces
{
    public interface INetworkConfiguration : ISystemDataLayerConfig
    {
        string RelayServiceEquityDomain { get; set; }
        string RelayServiceEquityPort { get; set; }
        string RelayServiceTradeDomain { get; set; }
        string RelayServiceTradePort { get; set; }
        string IsDeployedOnClientMachine { get; set; }
    }
}