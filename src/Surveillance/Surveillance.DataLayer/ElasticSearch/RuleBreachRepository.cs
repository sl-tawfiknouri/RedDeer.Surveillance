using Surveillance.DataLayer.ElasticSearch.Interfaces;
using Surveillance.ElasticSearchDtos.Rules;
using System;
using System.Threading;

namespace Surveillance.DataLayer.ElasticSearch
{
    public class RuleBreachRepository : IRuleBreachRepository
    {
        private readonly IElasticSearchDataAccess _elasticSearch;

        public RuleBreachRepository(IElasticSearchDataAccess elasticSearch)
        {
            _elasticSearch = elasticSearch ?? throw new ArgumentNullException(nameof(elasticSearch));
        }

        public void Save(RuleBreachDocument document)
        {
            if (document == null)
            {
                return;
            }

            var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(10000));

            var targetIndex = _elasticSearch.GetOrCreateDateBasedIndexAsync<RuleBreachDocument>(
                _elasticSearch.RuleBreachIndexName,
                DateTime.Now,
                cts.Token);

            var writeCts = new CancellationTokenSource(TimeSpan.FromMilliseconds(30000));

            _elasticSearch.IndexRuleBreachAsync(document, writeCts.Token);
        }
    }
}
