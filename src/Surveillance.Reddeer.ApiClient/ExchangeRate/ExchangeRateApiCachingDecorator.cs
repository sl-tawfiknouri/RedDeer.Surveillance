namespace Surveillance.Reddeer.ApiClient.ExchangeRate
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using RedDeer.Contracts.SurveillanceService.Api.ExchangeRate;

    using Surveillance.Reddeer.ApiClient.ExchangeRate.Interfaces;

    public class ExchangeRateApiCachingDecorator : IExchangeRateApiCachingDecorator
    {
        private readonly IExchangeRateApi _api;

        private readonly IDictionary<DateRange, CachedRates> _cache;

        private readonly object _lock = new object();

        public ExchangeRateApiCachingDecorator(IExchangeRateApi api)
        {
            this._api = api ?? throw new ArgumentNullException(nameof(api));
            this._cache = new ConcurrentDictionary<DateRange, CachedRates>();
            this.Expiry = TimeSpan.FromDays(1).Add(TimeSpan.FromHours(1));
        }

        public TimeSpan Expiry { get; set; }

        public void CleanCache()
        {
            var itemsToRemove = this._cache.Where(i => i.Value.Expiry < DateTime.UtcNow).ToList();

            foreach (var item in itemsToRemove) this._cache.Remove(item);
        }

        public async Task<IDictionary<DateTime, IReadOnlyCollection<ExchangeRateDto>>> Get(
            DateTime commencement,
            DateTime termination)
        {
            commencement = commencement.Date;
            termination = termination.Date;

            this.CleanCache();

            var dateRange = new DateRange(commencement, termination);
            if (this._cache.ContainsKey(dateRange))
            {
                this._cache.TryGetValue(dateRange, out var cachedRates);

                if (cachedRates == null) return new Dictionary<DateTime, IReadOnlyCollection<ExchangeRateDto>>();

                return cachedRates.Dtos;
            }

            var results = await this._api.Get(commencement, termination);

            var cacheRates = new CachedRates { Dtos = results, Expiry = DateTime.UtcNow.Add(this.Expiry) };
            this._cache.Add(dateRange, cacheRates);

            return results;
        }

        public async Task<bool> HeartBeating(CancellationToken token)
        {
            if (this._api == null) return false;

            var result = await this._api.HeartBeating(token);

            return result;
        }

        private class CachedRates
        {
            public IDictionary<DateTime, IReadOnlyCollection<ExchangeRateDto>> Dtos { get; set; }

            public DateTime Expiry { get; set; }
        }

        private class DateRange
        {
            public DateRange(DateTime start, DateTime end)
            {
                this.Start = start;
                this.End = end;
            }

            public DateTime End { get; }

            public DateTime Start { get; }

            public override bool Equals(object obj)
            {
                if (obj == null) return false;

                if (!(obj is DateRange dateRange)) return false;

                return this.Start.Date == dateRange.Start.Date && this.End.Date == dateRange.End.Date;
            }

            public override int GetHashCode()
            {
                return this.Start.Date.GetHashCode() * 13 + this.End.Date.GetHashCode() * 19;
            }
        }
    }
}