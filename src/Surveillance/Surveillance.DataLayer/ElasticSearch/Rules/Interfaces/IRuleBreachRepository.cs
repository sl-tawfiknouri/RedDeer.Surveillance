using Surveillance.ElasticSearchDtos.Rules;

namespace Surveillance.DataLayer.ElasticSearch.Rules.Interfaces
{
    public interface IRuleBreachRepository
    {
        void Save(RuleBreachDocument document);
    }
}