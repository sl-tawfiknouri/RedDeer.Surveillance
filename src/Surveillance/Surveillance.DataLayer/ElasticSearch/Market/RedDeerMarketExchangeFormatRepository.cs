using Surveillance.DataLayer.ElasticSearch.Interfaces;
using Surveillance.DataLayer.ElasticSearch.Market.Interfaces;
using Surveillance.ElasticSearchDtos.Market;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Surveillance.DataLayer.ElasticSearch.Market
{
    public class RedDeerMarketExchangeFormatRepository : IRedDeerMarketExchangeFormatRepository
    {
        private IElasticSearchDataAccess _dataAccess;

        public RedDeerMarketExchangeFormatRepository(IElasticSearchDataAccess dataAccess)
        {
            _dataAccess = dataAccess ?? throw new ArgumentNullException(nameof(dataAccess));
        }

        public async Task Save(ReddeerMarketDocument document)
        {
            if (document == null)
            {
                return;
            }

            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));
            var index = await
                _dataAccess.GetOrCreateDateBasedIndexAsync<ReddeerMarketDocument>(
                    _dataAccess.ReddeerStockExchangeFormatIndexName,
                    DateTime.UtcNow,
                    cts.Token);

            var saveCts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            await _dataAccess.IndexDocumentAsync(index, document, DateTime.UtcNow, saveCts.Token);
        }
    }
}
