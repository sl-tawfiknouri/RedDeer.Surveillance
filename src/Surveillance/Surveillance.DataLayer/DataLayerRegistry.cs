using Domain.Core.Financial.Cfis;
using Domain.Core.Financial.Cfis.Interfaces;
using Infrastructure.Network.Aws_IO;
using Infrastructure.Network.Aws_IO.Interfaces;
using Infrastructure.Network.HttpClient.Interfaces;
using PollyFacade.Policies;
using PollyFacade.Policies.Interfaces;
using StructureMap;
using Surveillance.DataLayer.Api.BmllMarketData;
using Surveillance.DataLayer.Api.BmllMarketData.Interfaces;
using Surveillance.DataLayer.Api.Enrichment;
using Surveillance.DataLayer.Api.Enrichment.Interfaces;
using Surveillance.DataLayer.Api.ExchangeRate;
using Surveillance.DataLayer.Api.ExchangeRate.Interfaces;
using Surveillance.DataLayer.Api.FactsetMarketData;
using Surveillance.DataLayer.Api.FactsetMarketData.Interfaces;
using Surveillance.DataLayer.Api.MarketOpenClose;
using Surveillance.DataLayer.Api.MarketOpenClose.Interfaces;
using Surveillance.DataLayer.Api.RuleParameter;
using Surveillance.DataLayer.Api.RuleParameter.Interfaces;
using Surveillance.DataLayer.Aurora;
using Surveillance.DataLayer.Aurora.Analytics;
using Surveillance.DataLayer.Aurora.Analytics.Interfaces;
using Surveillance.DataLayer.Aurora.BMLL;
using Surveillance.DataLayer.Aurora.BMLL.Interfaces;
using Surveillance.DataLayer.Aurora.Files;
using Surveillance.DataLayer.Aurora.Files.Interfaces;
using Surveillance.DataLayer.Aurora.Interfaces;
using Surveillance.DataLayer.Aurora.Market;
using Surveillance.DataLayer.Aurora.Market.Interfaces;
using Surveillance.DataLayer.Aurora.Orders;
using Surveillance.DataLayer.Aurora.Orders.Interfaces;
using Surveillance.DataLayer.Aurora.Rules;
using Surveillance.DataLayer.Aurora.Rules.Interfaces;
using Infrastructure.Network.HttpClient;

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
            For<IOrdersRepository>().Use<OrdersRepository>().Singleton();
            For<IReddeerMarketRepository>().Use<ReddeerMarketRepository>().Singleton();
            For<IRuleAnalyticsUniverseRepository>().Use<RuleAnalyticsUniverseRepository>();
            For<IRuleAnalyticsAlertsRepository>().Use<RuleAnalyticsAlertsRepository>();
            For<ICfiInstrumentTypeMapper>().Use<CfiInstrumentTypeMapper>();
            For<IRuleRunDataRequestRepository>().Use<RuleRunDataRequestRepository>();
            For<IStubRuleRunDataRequestRepository>().Use<StubRuleRunDataRequestRepository>();
            For<IBmllTimeBarApiRepository>().Use<BmllTimeBarApiRepository>();
            For<IReddeerMarketDailySummaryRepository>().Use<ReddeerMarketDailySummaryRepository>();
            For<IReddeerMarketTimeBarRepository>().Use<ReddeerMarketTimeBarRepository>();
            For<IFactsetDailyBarApiRepository>().Use<FactsetDailyBarApiRepository>();
            For<IOrderAllocationRepository>().Use<OrderAllocationRepository>();
            For<IRuleBreachRepository>().Use<RuleBreachRepository>();
            For<IRuleBreachOrdersRepository>().Use<RuleBreachOrdersRepository>();
            For<IFileUploadOrdersRepository>().Use<FileUploadOrdersRepository>();
            For<IFileUploadOrderAllocationRepository>().Use<FileUploadOrderAllocationRepository>();
            For<IPolicyFactory>().Use<PolicyFactory>();
            For<IHttpClientFactory>().Use<HttpClientFactory>();
        }
    }
}
