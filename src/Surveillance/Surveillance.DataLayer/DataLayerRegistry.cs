using StructureMap;
using Surveillance.DataLayer.Api.MarketOpenClose;
using Surveillance.DataLayer.Api.MarketOpenClose.Interfaces;
using Surveillance.DataLayer.Api.RuleParameter;
using Surveillance.DataLayer.Api.RuleParameter.Interfaces;
using Surveillance.DataLayer.ElasticSearch;
using Surveillance.DataLayer.ElasticSearch.DataAccess;
using Surveillance.DataLayer.ElasticSearch.DataAccess.Interfaces;
using Surveillance.DataLayer.ElasticSearch.Interfaces;
using Surveillance.DataLayer.ElasticSearch.Market;
using Surveillance.DataLayer.ElasticSearch.Market.Interfaces;
using Surveillance.DataLayer.ElasticSearch.Trade;
using Surveillance.DataLayer.ElasticSearch.Trade.Interfaces;
using Surveillance.DataLayer.Projectors;
using Surveillance.DataLayer.Projectors.Interfaces;
using Surveillance.DataLayer.Stub;
using Surveillance.DataLayer.Stub.Interfaces;
using Utilities.Aws_IO;
using Utilities.Aws_IO.Interfaces;

namespace Surveillance.DataLayer
{
    public class DataLayerRegistry : Registry
    {
        public DataLayerRegistry()
        {
            For<IElasticSearchDataAccess>().Use<ElasticSearchDataAccess>();
            For<IRedDeerTradeFormatRepository>().Use<RedDeerTradeFormatRepository>();
            For<IRedDeerMarketExchangeFormatRepository>().Use<RedDeerMarketExchangeFormatRepository>();
            For<IReddeerTradeFormatToReddeerTradeFrameProjector>().Use<ReddeerTradeFormatToReddeerTradeFrameProjector>();
            For<IAwsQueueClient>().Use<AwsQueueClient>();
            For<IMarketOpenCloseRepository>().Use<MarketOpenCloseRepository>();
            For<IMarketIndexNameBuilder>().Use<MarketIndexNameBuilder>();

            For<IMarketOpenCloseApiRepository>().Use<MarketOpenCloseApiRepository>();
            For<IMarketOpenCloseApiCachingDecoratorRepository>().Use<MarketOpenCloseApiCachingDecoratorRepository>();

            For<IRuleParameterApiRepository>().Use<RuleParameterApiRepository>();
            
            For<IReddeerMarketExchangeFormatToReddeerExchangeFrameProjector>()
                .Use<ReddeerMarketExchangeFormatToReddeerExchangeFrameProjector>();
        }
    }
}
