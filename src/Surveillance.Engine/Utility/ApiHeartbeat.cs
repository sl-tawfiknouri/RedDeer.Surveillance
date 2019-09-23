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
    ///     Checks that our apis on the client service are accessible
    /// </summary>
    public class ApiHeartbeat : IApiHeartbeat
    {
        private readonly IEnrichmentApi _enrichmentApi;

        private readonly IExchangeRateApiCachingDecorator _exchangeRateApi;

        private readonly ILogger<ApiHeartbeat> _logger;

        private readonly IMarketOpenCloseApiCachingDecorator _marketRateApi;

        private readonly IRuleParameterApi _rulesApi;

        public ApiHeartbeat(
            IExchangeRateApiCachingDecorator exchangeRateApi,
            IMarketOpenCloseApiCachingDecorator marketRateApi,
            IRuleParameterApi rulesApi,
            IEnrichmentApi enrichmentApi,
            ILogger<ApiHeartbeat> logger)
        {
            this._exchangeRateApi = exchangeRateApi ?? throw new ArgumentNullException(nameof(exchangeRateApi));
            this._marketRateApi = marketRateApi ?? throw new ArgumentNullException(nameof(marketRateApi));
            this._rulesApi = rulesApi ?? throw new ArgumentNullException(nameof(rulesApi));
            this._enrichmentApi = enrichmentApi ?? throw new ArgumentNullException(nameof(enrichmentApi));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> HeartsBeating()
        {
            var token = new CancellationTokenSource(TimeSpan.FromSeconds(15));

            try
            {
                var exchangeBeating = await this._exchangeRateApi.HeartBeating(token.Token);
                var marketBeating = await this._marketRateApi.HeartBeating(token.Token);
                var rulesBeating = await this._rulesApi.HeartBeating(token.Token);
                var enrichmentBeating = await this._enrichmentApi.HeartBeating(token.Token);

                return exchangeBeating && marketBeating && rulesBeating && enrichmentBeating;
            }
            catch (Exception e)
            {
                this._logger.LogError($"{e.Message} - {e?.InnerException?.Message}");
                return false;
            }
        }
    }
}