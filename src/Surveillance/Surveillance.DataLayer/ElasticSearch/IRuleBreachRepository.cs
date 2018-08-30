using Surveillance.ElasticSearchDtos;

namespace Surveillance.DataLayer.ElasticSearch
{
    public interface IRuleBreachRepository
    {
        void Save(RuleBreachDocument document);
    }
}