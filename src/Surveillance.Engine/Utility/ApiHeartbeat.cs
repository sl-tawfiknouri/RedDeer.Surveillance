using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Surveillance.Engine.Rules.Utility.Interfaces;
using Surveillance.Reddeer.ApiClient.Enrichment.Interfaces;
using Surveillance.Reddeer.ApiClient.ExchangeRate.Interfaces;
using Surveillance.Reddeer.ApiClient.MarketOpenClose.Interfaces;
using Surveillance.Reddeer.ApiClient.RuleParameter.Interfaces;

namespace Surveillance.Engine.Rules.Utility
{
    /// <summary>
    /// Checks that our apis on the client service are accessible
    /// </summary>
    public class ApiHeartbeat : IApiHeartbeat
    {
        private readonly IExchangeRateApiCachingDecorator _exchangeRateApi;
        private readonly IMarketOpenCloseApiCachingDecorator _marketRateApi;
        private readonly IRuleParameterApi _rulesApi;
        private readonly IEnrichmentApi _enrichmentApi;
        private readonly ILogger<ApiHeartbeat> _logger;

        public ApiHeartbeat(
            IExchangeRateApiCachingDecorator exchangeRateApi,
            IMarketOpenCloseApiCachingDecorator marketRateApi,
            IRuleParameterApi rulesApi,
            IEnrichmentApi enrichmentApi,
            ILogger<ApiHeartbeat> logger)
        {
            _exchangeRateApi = exchangeRateApi ?? throw new ArgumentNullException(nameof(exchangeRateApi));
            _marketRateApi = marketRateApi ?? throw new ArgumentNullException(nameof(marketRateApi));
            _rulesApi = rulesApi ?? throw new ArgumentNullException(nameof(rulesApi));
            _enrichmentApi = enrichmentApi ?? throw new ArgumentNullException(nameof(enrichmentApi));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> HeartsBeating()
        {
            var token = new CancellationTokenSource(TimeSpan.FromSeconds(15));

            try
            {
                var exchangeBeating = await _exchangeRateApi.HeartBeating(token.Token);
                var marketBeating = await _marketRateApi.HeartBeating(token.Token);
                var rulesBeating = await _rulesApi.HeartBeating(token.Token);
                var enrichmentBeating = await _enrichmentApi.HeartBeating(token.Token);

                return exchangeBeating && marketBeating && rulesBeating && enrichmentBeating;
            }
            catch (Exception e)
            {
                _logger.LogError($"{e.Message} - {e?.InnerException?.Message}");
                return false;
            }
        }
    }
}
