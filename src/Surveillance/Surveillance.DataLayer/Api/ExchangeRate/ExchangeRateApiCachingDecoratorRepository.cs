using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RedDeer.Contracts.SurveillanceService.Api.ExchangeRate;
using Surveillance.DataLayer.Api.ExchangeRate.Interfaces;

namespace Surveillance.DataLayer.Api.ExchangeRate
{
    public class ExchangeRateApiCachingDecoratorRepository : IExchangeRateApiCachingDecoratorRepository
    {
        private readonly IExchangeRateApiRepository _apiRepository;
        private readonly IDictionary<DateRange, CachedRates> _cache;
        private readonly TimeSpan _expiry;
        private readonly object _lock = new object();

        public ExchangeRateApiCachingDecoratorRepository(IExchangeRateApiRepository apiRepository)
        {
            _apiRepository = apiRepository ?? throw new ArgumentNullException(nameof(apiRepository));
            _cache = new ConcurrentDictionary<DateRange, CachedRates>();
            _expiry = TimeSpan.FromDays(1).Add(TimeSpan.FromHours(1));
        }

        public async Task<IDictionary<DateTime, IReadOnlyCollection<ExchangeRateDto>>> Get(
            DateTime commencement,
            DateTime termination)
        {
            lock (_lock)
            {
                CleanCache();

                var dateRange = new DateRange(commencement, termination);
                if (_cache.ContainsKey(dateRange))
                {
                    _cache.TryGetValue(dateRange, out var cachedRates);

                    if (cachedRates == null)
                    {
                        return new Dictionary<DateTime, IReadOnlyCollection<ExchangeRateDto>>();
                    }

                    return cachedRates.Dtos;
                }
                
                var resultTask = _apiRepository.Get(commencement, termination);
                resultTask.Wait();
                var results = resultTask.Result;

                var cacheRates = new CachedRates { Dtos = results, Expiry = DateTime.UtcNow.Add(_expiry) };
                _cache.Add(dateRange, cacheRates);

                return results;
            }
        }

        public void CleanCache()
        {
            var itemsToRemove = _cache.Where(i => i.Value.Expiry < DateTime.UtcNow).ToList();

            foreach (var item in itemsToRemove)
            {
                _cache.Remove(item);
            }
        }

        private class CachedRates
        {
            public IDictionary<DateTime, IReadOnlyCollection<ExchangeRateDto>> Dtos { get; set; }
            public DateTime Expiry { get; set; }
        }

        private class DateRange
        {
            public DateRange(
                DateTime start,
                DateTime end)
            {
                Start = start;
                End = end;
            }

            public DateTime Start { get; }
            public DateTime End { get; }

            public override int GetHashCode()
            {
                return Start.GetHashCode() * 13
                       + End.GetHashCode() * 19;
            }

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

                return Start.Date == dateRange.Start.Date
                       && End.Date == dateRange.End.Date;
            }
        }
    }
}
