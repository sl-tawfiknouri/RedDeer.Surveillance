namespace Surveillance.Reddeer.ApiClient
{
    using PollyFacade.Policies;
    using PollyFacade.Policies.Interfaces;

    using StructureMap;

    using Surveillance.Reddeer.ApiClient.BmllMarketData;
    using Surveillance.Reddeer.ApiClient.BmllMarketData.Interfaces;
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

    /// <summary>
    /// The red deer client registry.
    /// </summary>
    public class ReddeerApiClientRegistry : Registry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReddeerApiClientRegistry"/> class.
        /// </summary>
        public ReddeerApiClientRegistry()
        {
            this.For<IMarketOpenCloseApi>().Use<MarketOpenCloseApi>();
            this.For<IMarketOpenCloseApiCachingDecorator>().Use<MarketOpenCloseApiCachingDecorator>();
            this.For<IRuleParameterApi>().Use<RuleParameterApi>();
            this.For<IExchangeRateApi>().Use<ExchangeRateApi>();
            this.For<IExchangeRateApiCachingDecorator>().Use<ExchangeRateApiCachingDecorator>();
            this.For<IEnrichmentApi>().Use<EnrichmentApi>();
            this.For<IBmllTimeBarApi>().Use<BmllTimeBarApi>();
            this.For<IFactsetDailyBarApi>().Use<FactsetDailyBarApi>();
            this.For<IBrokerApi>().Use<BrokerApi>();
            this.For<IPolicyFactory>().Use<PolicyFactory>();
        }
    }
}