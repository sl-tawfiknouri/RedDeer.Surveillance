using PollyFacade.Policies;
using PollyFacade.Policies.Interfaces;
using StructureMap;
using Surveillance.Reddeer.ApiClient.BmllMarketData;
using Surveillance.Reddeer.ApiClient.BmllMarketData.Interfaces;
using Surveillance.Reddeer.ApiClient.Configuration;
using Surveillance.Reddeer.ApiClient.Configuration.Interfaces;
using Surveillance.Reddeer.ApiClient.Enrichment;
using Surveillance.Reddeer.ApiClient.Enrichment.Interfaces;
using Surveillance.Reddeer.ApiClient.ExchangeRate;
using Surveillance.Reddeer.ApiClient.ExchangeRate.Interfaces;
using Surveillance.Reddeer.ApiClient.FactsetMarketData;
using Surveillance.Reddeer.ApiClient.FactsetMarketData.Interfaces;
using Surveillance.Reddeer.ApiClient.MarketOpenClose;
using Surveillance.Reddeer.ApiClient.MarketOpenClose.Interfaces;
using Surveillance.Reddeer.ApiClient.RuleParameter;
using Surveillance.Reddeer.ApiClient.RuleParameter.Interfaces;

namespace Surveillance.Reddeer.ApiClient
{
    public class ReddeerApiClientRegistry : Registry
    {
        public ReddeerApiClientRegistry()
        {
            For<IMarketOpenCloseApi>().Use<MarketOpenCloseApi>();
            For<IMarketOpenCloseApiCachingDecorator>().Use<MarketOpenCloseApiCachingDecorator>();
            For<IRuleParameterApi>().Use<RuleParameterApi>();
            For<IExchangeRateApi>().Use<ExchangeRateApi>();
            For<IExchangeRateApiCachingDecorator>().Use<ExchangeRateApiCachingDecorator>();
            For<IEnrichmentApi>().Use<EnrichmentApi>();
            For<IBmllTimeBarApi>().Use<BmllTimeBarApi>();
            For<IFactsetDailyBarApi>().Use<FactsetDailyBarApi>();
            For<IBrokerApi>().Use<BrokerApi>();
            For<IPolicyFactory>().Use<PolicyFactory>();
            For<IApiClientConfiguration>().Use<ApiClientConfiguration>();
        }
    }
}
