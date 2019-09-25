namespace Surveillance.Engine.Rules.Utility
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using Surveillance.Engine.Rules.Utility.Interfaces;
    using Surveillance.Reddeer.ApiClient.Enrichment.Interfaces;
    using Surveillance.Reddeer.ApiClient.ExchangeRate.Interfaces;
    using Surveillance.Reddeer.ApiClient.MarketOpenClose.Interfaces;
    using Surveillance.Reddeer.ApiClient.RuleParameter.Interfaces;

    /// <summary>
    /// The application programming interface heartbeat.
    /// </summary>
    public class ApiHeartbeat : IApiHeartbeat
    {
        /// <summary>
        /// The enrichment.
        /// </summary>
        private readonly IEnrichmentApi enrichmentApi;

        /// <summary>
        /// The exchange rate.
        /// </summary>
        private readonly IExchangeRateApiCachingDecorator exchangeRateApi;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<ApiHeartbeat> logger;

        /// <summary>
        /// The market rate.
        /// </summary>
        private readonly IMarketOpenCloseApiCachingDecorator marketRateApi;

        /// <summary>
        /// The rules.
        /// </summary>
        private readonly IRuleParameterApi rulesApi;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiHeartbeat"/> class.
        /// </summary>
        /// <param name="exchangeRateApi">
        /// The exchange rate.
        /// </param>
        /// <param name="marketRateApi">
        /// The market rate.
        /// </param>
        /// <param name="rulesApi">
        /// The rules.
        /// </param>
        /// <param name="enrichmentApi">
        /// The enrichment.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public ApiHeartbeat(
            IExchangeRateApiCachingDecorator exchangeRateApi,
            IMarketOpenCloseApiCachingDecorator marketRateApi,
            IRuleParameterApi rulesApi,
            IEnrichmentApi enrichmentApi,
            ILogger<ApiHeartbeat> logger)
        {
            this.exchangeRateApi = exchangeRateApi ?? throw new ArgumentNullException(nameof(exchangeRateApi));
            this.marketRateApi = marketRateApi ?? throw new ArgumentNullException(nameof(marketRateApi));
            this.rulesApi = rulesApi ?? throw new ArgumentNullException(nameof(rulesApi));
            this.enrichmentApi = enrichmentApi ?? throw new ArgumentNullException(nameof(enrichmentApi));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// The hearts beating.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<bool> HeartsBeating()
        {
            var token = new CancellationTokenSource(TimeSpan.FromSeconds(15));

            try
            {
                var exchangeBeating = await this.exchangeRateApi.HeartBeating(token.Token);
                var marketBeating = await this.marketRateApi.HeartBeating(token.Token);
                var rulesBeating = await this.rulesApi.HeartBeating(token.Token);
                var enrichmentBeating = await this.enrichmentApi.HeartBeating(token.Token);

                return exchangeBeating && marketBeating && rulesBeating && enrichmentBeating;
            }
            catch (Exception e)
            {
                this.logger.LogError($"{e.Message} - {e?.InnerException?.Message}");

                return false;
            }
        }
    }
}