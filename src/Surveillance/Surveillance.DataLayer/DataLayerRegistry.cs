using StructureMap;
using Surveillance.DataLayer.Configuration;
using Surveillance.DataLayer.Configuration.Interfaces;
using Surveillance.DataLayer.ElasticSearch.DataAccess;
using Surveillance.DataLayer.ElasticSearch.DataAccess.Interfaces;
using Surveillance.DataLayer.ElasticSearch.Market;
using Surveillance.DataLayer.ElasticSearch.Market.Interfaces;
using Surveillance.DataLayer.ElasticSearch.Rules;
using Surveillance.DataLayer.ElasticSearch.Rules.Interfaces;
using Surveillance.DataLayer.ElasticSearch.Trade;
using Surveillance.DataLayer.ElasticSearch.Trade.Interfaces;
using Surveillance.DataLayer.Projectors;
using Surveillance.DataLayer.Projectors.Interfaces;
using Utilities.Aws_IO;
using Utilities.Aws_IO.Interfaces;

namespace Surveillance.DataLayer
{
    public class DataLayerRegistry : Registry
    {
        public DataLayerRegistry()
        {
            For<IElasticSearchDataAccess>().Use<ElasticSearchDataAccess>();
            For<IRuleBreachRepository>().Use<RuleBreachRepository>();
            For<IRedDeerTradeFormatRepository>().Use<RedDeerTradeFormatRepository>();
            For<IRedDeerMarketExchangeFormatRepository>().Use<RedDeerMarketExchangeFormatRepository>();
            For<IReddeerTradeFormatToReddeerTradeFrameProjector>().Use<ReddeerTradeFormatToReddeerTradeFrameProjector>();
            For<IAwsQueueClient>().Use<AwsQueueClient>();
        }
    }
}
