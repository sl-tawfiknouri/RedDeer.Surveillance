using Surveillance.DataLayer.ElasticSearch.Interfaces;
using Surveillance.DataLayer.Trade.Interfaces;
using Surveillance.ElasticSearchDtos.Trades;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Surveillance.DataLayer.Trade
{
    public class RedDeerTradeFormatRepository : IRedDeerTradeFormatRepository
    {
        private IElasticSearchDataAccess _dataAccess;

        public RedDeerTradeFormatRepository(IElasticSearchDataAccess dataAccess)
        {
            _dataAccess = dataAccess ?? throw new ArgumentNullException(nameof(dataAccess));
        }

        public async Task Save(ReddeerTradeDocument document)
        {
            if (document == null)
            {
                return;
            }

            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));           
            var index = await
                _dataAccess.GetOrCreateDateBasedIndexAsync<ReddeerTradeDocument>(
                    _dataAccess.ReddeerTradeFormatIndexName,
                    DateTime.UtcNow,
                    cts.Token);

            var saveCts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            await _dataAccess.IndexDocumentAsync(index, document, DateTime.UtcNow, saveCts.Token);
        }
    }
}
