using StructureMap;
using Surveillance.DataLayer.ElasticSearch;
using Surveillance.DataLayer.ElasticSearch.Interfaces;
using Surveillance.DataLayer.Trade;
using Surveillance.DataLayer.Trade.Interfaces;

namespace Surveillance
{
    public class DataLayerRegistry : Registry
    {
        public DataLayerRegistry()
        {
            For<IElasticSearchDataAccess>().Use<ElasticSearchDataAccess>();
            For<IRuleBreachRepository>().Use<RuleBreachRepository>();
            For<IRedDeerTradeFormatRepository>().Use<RedDeerTradeFormatRepository>();
        }
    }
}
