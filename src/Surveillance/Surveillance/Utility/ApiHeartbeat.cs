using System;
using System.Threading;
using System.Threading.Tasks;
using Surveillance.DataLayer.Api.ExchangeRate.Interfaces;
using Surveillance.DataLayer.Api.MarketOpenClose.Interfaces;
using Surveillance.DataLayer.Api.RuleParameter.Interfaces;
using Surveillance.Utility.Interfaces;

namespace Surveillance.Utility
{
    public class ApiHeartbeat : IApiHeartbeat
    {
        private readonly IExchangeRateApiCachingDecoratorRepository _exchangeRateApi;
        private readonly IMarketOpenCloseApiCachingDecoratorRepository _marketRateApi;
        private readonly IRuleParameterApiRepository _rulesApi;

        public ApiHeartbeat(
            IExchangeRateApiCachingDecoratorRepository exchangeRateApi,
            IMarketOpenCloseApiCachingDecoratorRepository marketRateApi,
            IRuleParameterApiRepository rulesApi)
        {
            _exchangeRateApi = exchangeRateApi ?? throw new ArgumentNullException(nameof(exchangeRateApi));
            _marketRateApi = marketRateApi ?? throw new ArgumentNullException(nameof(marketRateApi));
            _rulesApi = rulesApi ?? throw new ArgumentNullException(nameof(rulesApi));
        }

        public async Task<bool> HeartsBeating()
        {
            var token = new CancellationTokenSource(TimeSpan.FromSeconds(15));

            var exchangeBeating = await _exchangeRateApi.HeartBeating(token.Token);
            var marketBeating = await _marketRateApi.HeartBeating(token.Token);
            var rulesBeating = await _rulesApi.HeartBeating(token.Token);

            return exchangeBeating && marketBeating && rulesBeating;
        }
    }
}
