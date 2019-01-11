using DomainV2.Markets;
using ThirdPartySurveillanceDataSynchroniser.DataSources;

namespace ThirdPartySurveillanceDataSynchroniser.Manager
{
    public class MarketDataRequestDataSource
    {
        public MarketDataRequestDataSource(
            DataSource dataSource,
            MarketDataRequest dataRequest)
        {
            DataSource = dataSource;
            DataRequest = dataRequest;
        }

        public DataSource DataSource { get; }
        public MarketDataRequest DataRequest { get; }
    }
}
