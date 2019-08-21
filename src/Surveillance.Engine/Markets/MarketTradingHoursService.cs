namespace Surveillance.Engine.Rules.Markets
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Domain.Core.Dates;

    using Microsoft.Extensions.Logging;

    using RedDeer.Contracts.SurveillanceService.Api.Markets;

    using Surveillance.Engine.Rules.Markets.Interfaces;
    using Surveillance.Reddeer.ApiClient.MarketOpenClose.Interfaces;

    public class MarketTradingHoursService : IMarketTradingHoursService
    {
        private readonly ILogger<MarketTradingHoursService> _logger;

        private readonly IMarketOpenCloseApiCachingDecorator _repository;

        public MarketTradingHoursService(
            IMarketOpenCloseApiCachingDecorator repository,
            ILogger<MarketTradingHoursService> logger)
        {
            this._repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        ///     Will create a series of requests based off of the from utc (and time) to the to utc (and time)
        /// </summary>
        public IReadOnlyCollection<DateRange> GetTradingDaysWithinRangeAdjustedToTime(
            DateTime fromUtc,
            DateTime toUtc,
            string marketIdentifierCode)
        {
            if (string.IsNullOrWhiteSpace(marketIdentifierCode)) return new DateRange[0];

            if (fromUtc > toUtc) return new DateRange[0];

            var offsetTimeSpan = TimeSpan.Zero;
            if (fromUtc.TimeOfDay <= toUtc.TimeOfDay) offsetTimeSpan = toUtc.TimeOfDay - fromUtc.TimeOfDay;
            else offsetTimeSpan = fromUtc.TimeOfDay - toUtc.TimeOfDay;

            if (offsetTimeSpan == TimeSpan.Zero) offsetTimeSpan = TimeSpan.FromDays(1);

            var exchange = this.GetExchange(marketIdentifierCode);

            if (exchange == null) return new DateRange[0];

            var currentDate = fromUtc.Date;

            var dateRanges = new List<DateRange>();
            while (currentDate <= toUtc)
            {
                var from = currentDate.Date.Add(fromUtc.TimeOfDay);
                var to = from.Add(offsetTimeSpan);

                dateRanges.Add(new DateRange(from, to));
                currentDate = currentDate.AddDays(1);
            }

            var filteredAdjustedDateRange = this.FilterOutForNonTradingDays(dateRanges, exchange);
            var holidayAdjustedDateRange = this.FilterOutForHolidays(filteredAdjustedDateRange, exchange);

            return holidayAdjustedDateRange;
        }

        public ITradingHours GetTradingHoursForMic(string marketIdentifierCode)
        {
            var exchange = this.GetExchange(marketIdentifierCode);

            if (exchange == null) return new TradingHours { Mic = marketIdentifierCode, IsValid = false };

            return new TradingHours
                       {
                           Mic = marketIdentifierCode,
                           IsValid = true,
                           OpenOffsetInUtc = exchange.MarketOpenTime,
                           CloseOffsetInUtc = exchange.MarketCloseTime
                       };
        }

        private bool ExchangeIsOpenOnDayStart(DateRange range, ExchangeDto exchange)
        {
            if (range == null || exchange == null) return false;

            switch (range.Start.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    return exchange.IsOpenOnMonday;
                case DayOfWeek.Tuesday:
                    return exchange.IsOpenOnTuesday;
                case DayOfWeek.Wednesday:
                    return exchange.IsOpenOnWednesday;
                case DayOfWeek.Thursday:
                    return exchange.IsOpenOnThursday;
                case DayOfWeek.Friday:
                    return exchange.IsOpenOnFriday;
                case DayOfWeek.Saturday:
                    return exchange.IsOpenOnSaturday;
                case DayOfWeek.Sunday:
                    return exchange.IsOpenOnSunday;
                default:
                    return false;
            }
        }

        private List<DateRange> FilterOutForHolidays(List<DateRange> dateRanges, ExchangeDto exchange)
        {
            if (dateRanges == null || !dateRanges.Any() || exchange == null) return new List<DateRange>();

            var exchangeHolidays = exchange.Holidays ?? new DateTime[0];
            var holidays = exchangeHolidays.Select(y => y.Date).ToList();
            var adjustedHolidays = holidays
                .Select(y => new DateRange(y.Add(exchange.MarketOpenTime), y.Add(exchange.MarketCloseTime))).ToList();

            foreach (var hol in adjustedHolidays) dateRanges.RemoveAll(x => x.Intersection(hol));

            return dateRanges;
        }

        private List<DateRange> FilterOutForNonTradingDays(List<DateRange> dateRanges, ExchangeDto exchange)
        {
            if (dateRanges == null || !dateRanges.Any() || exchange == null) return new List<DateRange>();

            return dateRanges.Where(i => this.ExchangeIsOpenOnDayStart(i, exchange)).ToList();
        }

        private ExchangeDto GetExchange(string marketIdentifierCode)
        {
            if (string.IsNullOrWhiteSpace(marketIdentifierCode))
            {
                this._logger.LogInformation($"received a null or empty MIC {marketIdentifierCode}");
                return null;
            }

            if (string.Equals(marketIdentifierCode, "na", StringComparison.OrdinalIgnoreCase) || string.Equals(
                    marketIdentifierCode,
                    "n/a",
                    StringComparison.OrdinalIgnoreCase))
            {
                this._logger.LogDebug($"received an na or n/a MIC {marketIdentifierCode}");
                return null;
            }

            var resultTask = this._repository.Get();
            var result = resultTask.Result;

            var exchange = result.FirstOrDefault(
                res => string.Equals(res.Code, marketIdentifierCode, StringComparison.InvariantCultureIgnoreCase));

            if (exchange == null)
            {
                this._logger.LogError($"could not find a match for {marketIdentifierCode}");
                return null;
            }

            this._logger.LogInformation($"found a match for {marketIdentifierCode}");

            return exchange;
        }
    }
}