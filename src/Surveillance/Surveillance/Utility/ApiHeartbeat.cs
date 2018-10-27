using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Api.Enrichment.Interfaces;
using Surveillance.DataLayer.Api.ExchangeRate.Interfaces;
using Surveillance.DataLayer.Api.MarketOpenClose.Interfaces;
using Surveillance.DataLayer.Api.RuleParameter.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.Utility.Interfaces;

namespace Surveillance.Utility
{
    /// <summary>
    /// Checks that our apis on the client service are accessible
    /// </summary>
    public class ApiHeartbeat : IApiHeartbeat
    {
        private readonly ISystemProcessContext _systemProcessContext;
        private readonly IExchangeRateApiCachingDecoratorRepository _exchangeRateApi;
        private readonly IMarketOpenCloseApiCachingDecoratorRepository _marketRateApi;
        private readonly IRuleParameterApiRepository _rulesApi;
        private readonly IEnrichmentApiRepository _enrichmentApi;
        private readonly ILogger<ApiHeartbeat> _logger;

        public ApiHeartbeat(
            IExchangeRateApiCachingDecoratorRepository exchangeRateApi,
            IMarketOpenCloseApiCachingDecoratorRepository marketRateApi,
            IRuleParameterApiRepository rulesApi,
            IEnrichmentApiRepository enrichmentApi,
            ISystemProcessContext processCtx,
            ILogger<ApiHeartbeat> logger)
        {
            _exchangeRateApi = exchangeRateApi ?? throw new ArgumentNullException(nameof(exchangeRateApi));
            _marketRateApi = marketRateApi ?? throw new ArgumentNullException(nameof(marketRateApi));
            _rulesApi = rulesApi ?? throw new ArgumentNullException(nameof(rulesApi));
            _systemProcessContext = processCtx ?? throw new ArgumentNullException(nameof(processCtx));
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
                _logger.LogError(e.ToString());
                return false;
            }
        }
    }
}
