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

    /// <summary>
    /// The exchange rate caching decorator.
    /// </summary>
    public class ExchangeRateApiCachingDecorator : IExchangeRateApiCachingDecorator
    {
        /// <summary>
        /// The exchange rate.
        /// </summary>
        private readonly IExchangeRateApi api;

        /// <summary>
        /// The cache.
        /// </summary>
        private readonly IDictionary<DateRange, CachedRates> cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExchangeRateApiCachingDecorator"/> class.
        /// </summary>
        /// <param name="api">
        /// The exchange rate.
        /// </param>
        public ExchangeRateApiCachingDecorator(IExchangeRateApi api)
        {
            this.api = api ?? throw new ArgumentNullException(nameof(api));
            this.cache = new ConcurrentDictionary<DateRange, CachedRates>();
            this.Expiry = TimeSpan.FromDays(1).Add(TimeSpan.FromHours(1));
        }

        /// <summary>
        /// Gets or sets the expiry.
        /// </summary>
        public TimeSpan Expiry { get; set; }

        /// <summary>
        /// The clean cache.
        /// </summary>
        public void CleanCache()
        {
            var itemsToRemove = this.cache.Where(i => i.Value.Expiry < DateTime.UtcNow).ToList();

            foreach (var item in itemsToRemove)
            {
                this.cache.Remove(item);
            }
        }

        /// <summary>
        /// The get async.
        /// </summary>
        /// <param name="commencement">
        /// The commencement.
        /// </param>
        /// <param name="termination">
        /// The termination.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<IDictionary<DateTime, IReadOnlyCollection<ExchangeRateDto>>> GetAsync(
            DateTime commencement,
            DateTime termination)
        {
            commencement = commencement.Date;
            termination = termination.Date;

            this.CleanCache();

            var dateRange = new DateRange(commencement, termination);

            if (this.cache.TryGetValue(dateRange, out var cachedRates))
            {
                if (cachedRates == null)
                {
                    return new Dictionary<DateTime, IReadOnlyCollection<ExchangeRateDto>>();
                }

                return cachedRates.Dtos;
            }

            var results = await this.api.GetAsync(commencement, termination);

            var cacheRates = new CachedRates { Dtos = results, Expiry = DateTime.UtcNow.Add(this.Expiry) };
            this.cache.Add(dateRange, cacheRates);

            return results;
        }

        /// <summary>
        /// The heart beating async.
        /// </summary>
        /// <param name="token">
        /// The token.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<bool> HeartBeatingAsync(CancellationToken token)
        {
            if (this.api == null)
            {
                return false;
            }

            var result = await this.api.HeartBeatingAsync(token);

            return result;
        }

        /// <summary>
        /// The cached rates.
        /// </summary>
        private class CachedRates
        {
            /// <summary>
            /// Gets or sets the exchange rate collection.
            /// </summary>
            public IDictionary<DateTime, IReadOnlyCollection<ExchangeRateDto>> Dtos { get; set; }

            /// <summary>
            /// Gets or sets the expiry.
            /// </summary>
            public DateTime Expiry { get; set; }
        }

        /// <summary>
        /// The date range.
        /// </summary>
        private class DateRange
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="DateRange"/> class.
            /// </summary>
            /// <param name="start">
            /// The start.
            /// </param>
            /// <param name="end">
            /// The end.
            /// </param>
            public DateRange(DateTime start, DateTime end)
            {
                this.Start = start;
                this.End = end;
            }

            /// <summary>
            /// Gets the end.
            /// </summary>
            public DateTime End { get; }

            /// <summary>
            /// Gets the start.
            /// </summary>
            public DateTime Start { get; }

            /// <summary>
            /// The equals.
            /// </summary>
            /// <param name="obj">
            /// The object.
            /// </param>
            /// <returns>
            /// The <see cref="bool"/>.
            /// </returns>
            public override bool Equals(object obj)
            {
                if (obj == null)
                {
                    return false;
                }

                if (!(obj is DateRange dateRange))
                {
                    return false;
                }

                return this.Start.Date == dateRange.Start.Date && this.End.Date == dateRange.End.Date;
            }

            /// <summary>
            /// The get hash code.
            /// </summary>
            /// <returns>
            /// The <see cref="int"/>.
            /// </returns>
            public override int GetHashCode()
            {
                return (this.Start.Date.GetHashCode() * 13) + (this.End.Date.GetHashCode() * 19);
            }
        }
    }
}