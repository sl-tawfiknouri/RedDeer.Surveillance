using System;
using System.Threading;
using System.Threading.Tasks;
using Nest;
using Surveillance.ElasticSearchDtos.Rules;

namespace Surveillance.DataLayer.ElasticSearch.Interfaces
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

        string RuleBreachIndexName { get; }
        string ReddeerTradeFormatIndexName { get; }
        Task IndexRuleBreachAsync(RuleBreachDocument document, CancellationToken cancellationToken);
        Task DeleteIndexesAsync(string name, CancellationToken cancellationToken);
        void HandleResponseErrors(IResponse response);
    }
}