using Surveillance.DataLayer.ElasticSearch.Market.Interfaces;
using Surveillance.ElasticSearchDtos.Market;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Surveillance.DataLayer.ElasticSearch.DataAccess.Interfaces;

namespace Surveillance.DataLayer.ElasticSearch.Market
{
    public class RedDeerMarketExchangeFormatRepository : IRedDeerMarketExchangeFormatRepository
    {
        private readonly IElasticSearchDataAccess _dataAccess;

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

            var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
            var index = await
                _dataAccess.GetOrCreateDateBasedIndexAsync<ReddeerMarketDocument>(
                    _dataAccess.ReddeerStockExchangeFormatIndexName,
                    DateTime.UtcNow,
                    cts.Token);

            var saveCts = new CancellationTokenSource(TimeSpan.FromMinutes(5));

            await _dataAccess.IndexDocumentAsync(index, document, DateTime.UtcNow, saveCts.Token);
        }

        public async Task<IReadOnlyCollection<ReddeerMarketDocument>> Get(DateTime start, DateTime end)
        {
            var cancellationToken = new CancellationTokenSource(TimeSpan.FromMinutes(5)).Token;

            return
                await _dataAccess.GetMarketDocuments(start, end, cancellationToken)
                ?? new List<ReddeerMarketDocument>();
        }
    }
}
