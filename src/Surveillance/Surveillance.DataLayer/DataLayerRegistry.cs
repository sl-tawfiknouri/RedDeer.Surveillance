using StructureMap;
using Surveillance.DataLayer.ElasticSearch;
using Surveillance.DataLayer.ElasticSearch.Interfaces;

namespace Surveillance
{
    public class DataLayerRegistry : Registry
    {
        public DataLayerRegistry()
        {
            For<IElasticSearchDataAccess>().Use<ElasticSearchDataAccess>();
            For<IRuleBreachRepository>().Use<RuleBreachRepository>();
        }
    }
}
