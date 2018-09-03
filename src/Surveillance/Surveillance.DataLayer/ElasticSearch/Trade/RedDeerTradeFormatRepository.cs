using System;
using System.Threading;
using System.Threading.Tasks;
using Surveillance.DataLayer.ElasticSearch.DataAccess.Interfaces;
using Surveillance.DataLayer.ElasticSearch.Trade.Interfaces;
using Surveillance.ElasticSearchDtos.Trades;

namespace Surveillance.DataLayer.ElasticSearch.Trade
{
    public class RedDeerTradeFormatRepository : IRedDeerTradeFormatRepository
    {
        private readonly IElasticSearchDataAccess _dataAccess;

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

            var cts = new CancellationTokenSource(TimeSpan.FromMinutes(15));           
            var index = await
                _dataAccess.GetOrCreateDateBasedIndexAsync<ReddeerTradeDocument>(
                    _dataAccess.ReddeerTradeFormatIndexName,
                    DateTime.UtcNow,
                    cts.Token);

            var saveCts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
            await _dataAccess.IndexDocumentAsync(index, document, DateTime.UtcNow, saveCts.Token);
        }
    }
}
