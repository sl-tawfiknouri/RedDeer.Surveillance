using StructureMap;
using Surveillance.DataLayer.Api.ExchangeRate;
using Surveillance.DataLayer.Api.ExchangeRate.Interfaces;
using Surveillance.DataLayer.Api.MarketOpenClose;
using Surveillance.DataLayer.Api.MarketOpenClose.Interfaces;
using Surveillance.DataLayer.Api.RuleParameter;
using Surveillance.DataLayer.Api.RuleParameter.Interfaces;
using Surveillance.DataLayer.Aurora;
using Surveillance.DataLayer.Aurora.Interfaces;
using Surveillance.DataLayer.Aurora.Trade;
using Surveillance.DataLayer.Aurora.Trade.Interfaces;
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
            For<IMarketIndexNameBuilder>().Use<MarketIndexNameBuilder>();

            For<IMarketOpenCloseApiRepository>().Use<MarketOpenCloseApiRepository>();
            For<IMarketOpenCloseApiCachingDecoratorRepository>().Use<MarketOpenCloseApiCachingDecoratorRepository>();

            For<IRuleParameterApiRepository>().Use<RuleParameterApiRepository>();

            For<IExchangeRateApiRepository>().Use<ExchangeRateApiRepository>();
            For<IExchangeRateApiCachingDecoratorRepository>().Use<ExchangeRateApiCachingDecoratorRepository>();

            For<IReddeerMarketExchangeFormatToReddeerExchangeFrameProjector>()
                .Use<ReddeerMarketExchangeFormatToReddeerExchangeFrameProjector>();

            For<IConnectionStringFactory>().Use<ConnectionStringFactory>();
            For<IReddeerTradeRepository>().Use<ReddeerTradeRepository>();
        }
    }
}
