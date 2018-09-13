using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Nest;
using Surveillance.ElasticSearchDtos.Market;
using Surveillance.ElasticSearchDtos.Rules;
using Surveillance.ElasticSearchDtos.Trades;

namespace Surveillance.DataLayer.ElasticSearch.DataAccess.Interfaces
{
    public interface IElasticSearchDataAccess
    {
        Task<string> GetOrCreateDateBasedIndexAsync<T>(
            string name,
            DateTime dateTime,
            CancellationToken cancellationToken) 
        where T : class;

        Task IndexDocumentAsync<T>(
            string indexName,
            T indexableDocument,
            DateTime timestamp,
            CancellationToken cancellationToken)
            where T : class;

        Task<IReadOnlyCollection<ReddeerTradeDocument>> GetDocuments(
            DateTime start,
            DateTime end,
            CancellationToken cancellationToken);

        Task<IReadOnlyCollection<ReddeerMarketDocument>> GetMarketDocuments(
            DateTime start,
            DateTime end,
            CancellationToken cancellationToken);

        string RuleBreachIndexName { get; }
        string ReddeerTradeFormatIndexName { get; }
        string ReddeerStockExchangeFormatIndexName { get; }
        Task IndexRuleBreachAsync(RuleBreachDocument document, CancellationToken cancellationToken);
        Task DeleteIndexesAsync(string name, CancellationToken cancellationToken);
        void HandleResponseErrors(IResponse response);
    }
}