using DomainV2.Financial;
using DomainV2.Financial.Interfaces;
using StructureMap;
using Surveillance.DataLayer.Api.BmllMarketData;
using Surveillance.DataLayer.Api.BmllMarketData.Interfaces;
using Surveillance.DataLayer.Api.Enrichment;
using Surveillance.DataLayer.Api.Enrichment.Interfaces;
using Surveillance.DataLayer.Api.ExchangeRate;
using Surveillance.DataLayer.Api.ExchangeRate.Interfaces;
using Surveillance.DataLayer.Api.MarketOpenClose;
using Surveillance.DataLayer.Api.MarketOpenClose.Interfaces;
using Surveillance.DataLayer.Api.RuleParameter;
using Surveillance.DataLayer.Api.RuleParameter.Interfaces;
using Surveillance.DataLayer.Aurora;
using Surveillance.DataLayer.Aurora.Analytics;
using Surveillance.DataLayer.Aurora.Analytics.Interfaces;
using Surveillance.DataLayer.Aurora.BMLL;
using Surveillance.DataLayer.Aurora.BMLL.Interfaces;
using Surveillance.DataLayer.Aurora.Interfaces;
using Surveillance.DataLayer.Aurora.Market;
using Surveillance.DataLayer.Aurora.Market.Interfaces;
using Surveillance.DataLayer.Aurora.Trade;
using Surveillance.DataLayer.Aurora.Trade.Interfaces;
using Utilities.Aws_IO;
using Utilities.Aws_IO.Interfaces;

namespace Surveillance.DataLayer
{
    public class DataLayerRegistry : Registry
    {
        public DataLayerRegistry()
        {
            For<IAwsQueueClient>().Use<AwsQueueClient>();
            For<IMarketOpenCloseApiRepository>().Use<MarketOpenCloseApiRepository>();
            For<IMarketOpenCloseApiCachingDecoratorRepository>().Use<MarketOpenCloseApiCachingDecoratorRepository>();
            For<IRuleParameterApiRepository>().Use<RuleParameterApiRepository>();
            For<IExchangeRateApiRepository>().Use<ExchangeRateApiRepository>();
            For<IExchangeRateApiCachingDecoratorRepository>().Use<ExchangeRateApiCachingDecoratorRepository>();
            For<IEnrichmentApiRepository>().Use<EnrichmentApiRepository>();
            For<IConnectionStringFactory>().Use<ConnectionStringFactory>();
            For<IReddeerTradeRepository>().Use<ReddeerTradeRepository>().Singleton();
            For<IReddeerMarketRepository>().Use<ReddeerMarketRepository>().Singleton();
            For<IRuleAnalyticsUniverseRepository>().Use<RuleAnalyticsUniverseRepository>();
            For<IRuleAnalyticsAlertsRepository>().Use<RuleAnalyticsAlertsRepository>();
            For<ICfiInstrumentTypeMapper>().Use<CfiInstrumentTypeMapper>();
            For<IRuleRunDataRequestRepository>().Use<RuleRunDataRequestRepository>();
            For<IStubRuleRunDataRequestRepository>().Use<StubRuleRunDataRequestRepository>();
            For<IBmllTimeBarApiRepository>().Use<BmllTimeBarApiRepository>();
            For<IReddeerMarketDailySummaryRepository>().Use<ReddeerMarketDailySummaryRepository>();
            For<IReddeerMarketTimeBarRepository>().Use<ReddeerMarketTimeBarRepository>();
        }
    }
}
