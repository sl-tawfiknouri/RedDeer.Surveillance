using Surveillance.ElasticSearchDtos.Rules;

namespace Surveillance.DataLayer.ElasticSearch
{
    public interface IRuleBreachRepository
    {
        void Save(RuleBreachDocument document);
    }
}