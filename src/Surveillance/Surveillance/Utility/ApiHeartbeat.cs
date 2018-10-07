using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Api.ExchangeRate.Interfaces;
using Surveillance.DataLayer.Api.MarketOpenClose.Interfaces;
using Surveillance.DataLayer.Api.RuleParameter.Interfaces;
using Surveillance.Utility.Interfaces;

namespace Surveillance.Utility
{
    /// <summary>
    /// Checks that our apis on the client service are accessible
    /// </summary>
    public class ApiHeartbeat : IApiHeartbeat
    {
        private readonly IExchangeRateApiCachingDecoratorRepository _exchangeRateApi;
        private readonly IMarketOpenCloseApiCachingDecoratorRepository _marketRateApi;
        private readonly IRuleParameterApiRepository _rulesApi;
        private readonly ILogger<ApiHeartbeat> _logger;

        public ApiHeartbeat(
            IExchangeRateApiCachingDecoratorRepository exchangeRateApi,
            IMarketOpenCloseApiCachingDecoratorRepository marketRateApi,
            IRuleParameterApiRepository rulesApi,
            ILogger<ApiHeartbeat> logger)
        {
            _exchangeRateApi = exchangeRateApi ?? throw new ArgumentNullException(nameof(exchangeRateApi));
            _marketRateApi = marketRateApi ?? throw new ArgumentNullException(nameof(marketRateApi));
            _rulesApi = rulesApi ?? throw new ArgumentNullException(nameof(rulesApi));
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

                return exchangeBeating && marketBeating && rulesBeating;
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return false;
            }
        }
    }
}
