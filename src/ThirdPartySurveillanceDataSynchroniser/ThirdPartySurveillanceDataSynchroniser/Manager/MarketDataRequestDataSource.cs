using DataSynchroniser.DataSources;
using Domain.Markets;

namespace DataSynchroniser.Manager
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
