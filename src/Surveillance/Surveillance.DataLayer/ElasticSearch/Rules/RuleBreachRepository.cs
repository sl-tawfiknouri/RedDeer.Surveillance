using System;
using System.Threading;
using Surveillance.DataLayer.ElasticSearch.DataAccess.Interfaces;
using Surveillance.DataLayer.ElasticSearch.Rules.Interfaces;
using Surveillance.ElasticSearchDtos.Rules;

namespace Surveillance.DataLayer.ElasticSearch.Rules
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

            _elasticSearch.GetOrCreateDateBasedIndexAsync<RuleBreachDocument>(
                _elasticSearch.RuleBreachIndexName,
                DateTime.Now,
                cts.Token);

            var writeCts = new CancellationTokenSource(TimeSpan.FromMilliseconds(30000));

            _elasticSearch.IndexRuleBreachAsync(document, writeCts.Token);
        }
    }
}
