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

    /// <summary>
    /// The market trading hours service.
    /// </summary>
    public class MarketTradingHoursService : IMarketTradingHoursService
    {
        /// <summary>
        /// The market open close caching decorator.
        /// </summary>
        private readonly IMarketOpenCloseApiCachingDecorator marketOpenCloseCachingDecorator;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<MarketTradingHoursService> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="MarketTradingHoursService"/> class.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public MarketTradingHoursService(
            IMarketOpenCloseApiCachingDecorator repository,
            ILogger<MarketTradingHoursService> logger)
        {
            this.marketOpenCloseCachingDecorator = repository ?? throw new ArgumentNullException(nameof(repository));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// The get trading hours for mic.
        /// </summary>
        /// <param name="marketIdentifierCode">
        /// The market identifier code.
        /// </param>
        /// <returns>
        /// The <see cref="ITradingHours"/>.
        /// </returns>
        public ITradingHours GetTradingHoursForMic(string marketIdentifierCode)
        {
            if (string.Equals(marketIdentifierCode, "NA", StringComparison.OrdinalIgnoreCase)
                || string.Equals(marketIdentifierCode, "N/A", StringComparison.OrdinalIgnoreCase))
            {
                return new TradingHours { Mic = marketIdentifierCode, IsValid = false };
            }

            if (string.Equals(marketIdentifierCode, "OTC", StringComparison.OrdinalIgnoreCase))
            {
                return new TradingHours
                           {
                               OpenOffsetInUtc = TimeSpan.Zero,
                               CloseOffsetInUtc = TimeSpan.FromDays(1) - TimeSpan.FromSeconds(1),
                               IsValid = true,
                               Mic = "OTC"
                           };
            }

            var exchange = this.GetExchange(marketIdentifierCode);

            if (exchange == null)
            {
                return new TradingHours { Mic = marketIdentifierCode, IsValid = false };
            }

            return new TradingHours
            {
                Mic = marketIdentifierCode,
                IsValid = true,
                OpenOffsetInUtc = exchange.MarketOpenTime,
                CloseOffsetInUtc = exchange.MarketCloseTime
            };
        }

        /// <summary>
        /// Will create a series of requests based off of the from universal time zone (and time) to the to universal time zone (and time)
        /// </summary>
        /// <param name="fromUtc">
        /// The from universal time zone.
        /// </param>
        /// <param name="toUtc">
        /// The to universal time zone.
        /// </param>
        /// <param name="marketIdentifierCode">
        /// The market Identifier Code.
        /// </param>
        /// <returns>
        /// The <see cref="DateRange"/>.
        /// </returns>
        public IReadOnlyCollection<DateRange> GetTradingDaysWithinRangeAdjustedToTime(DateTime fromUtc, DateTime toUtc, string marketIdentifierCode)
        {
            if (string.IsNullOrWhiteSpace(marketIdentifierCode))
            {
                return new DateRange[0];
            }

            if (fromUtc > toUtc)
            {
                return new DateRange[0];
            }

            var offsetTimeSpan = TimeSpan.Zero;
            if (fromUtc.TimeOfDay <= toUtc.TimeOfDay)
            {
                offsetTimeSpan = toUtc.TimeOfDay - fromUtc.TimeOfDay;
            }
            else
            {
                offsetTimeSpan = fromUtc.TimeOfDay - toUtc.TimeOfDay;
            }

            if (offsetTimeSpan == TimeSpan.Zero)
            {
                offsetTimeSpan = TimeSpan.FromDays(1);
            }

            var exchange = GetExchange(marketIdentifierCode);

            if (exchange == null)
            {
                return new DateRange[0];
            }

            var currentDate = fromUtc.Date;

            var dateRanges = new List<DateRange>();
            while (currentDate <= toUtc)
            {
                var from = currentDate.Date.Add(fromUtc.TimeOfDay);
                var to = from.Add(offsetTimeSpan);

                dateRanges.Add(new DateRange(from, to));
                currentDate = currentDate.AddDays(1);
            }

            var filteredAdjustedDateRange = FilterOutForNonTradingDays(dateRanges, exchange);
            var holidayAdjustedDateRange = FilterOutForHolidays(filteredAdjustedDateRange, exchange);

            return holidayAdjustedDateRange;
        }

        /// <summary>
        /// The filter out for non trading days.
        /// </summary>
        /// <param name="dateRanges">
        /// The date ranges.
        /// </param>
        /// <param name="exchange">
        /// The exchange.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        private List<DateRange> FilterOutForNonTradingDays(List<DateRange> dateRanges, ExchangeDto exchange)
        {
            if (dateRanges == null
                || !dateRanges.Any()
                || exchange == null)
            {
                return new List<DateRange>();
            }

            return dateRanges.Where(i => ExchangeIsOpenOnDayStart(i, exchange)).ToList();
        }

        /// <summary>
        /// The exchange is open on day start.
        /// </summary>
        /// <param name="range">
        /// The range.
        /// </param>
        /// <param name="exchange">
        /// The exchange.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool ExchangeIsOpenOnDayStart(DateRange range, ExchangeDto exchange)
        {
            if (range == null
                || exchange == null)
            {
                return false;
            }

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

        /// <summary>
        /// The filter out for holidays.
        /// </summary>
        /// <param name="dateRanges">
        /// The date ranges.
        /// </param>
        /// <param name="exchange">
        /// The exchange.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        private List<DateRange> FilterOutForHolidays(List<DateRange> dateRanges, ExchangeDto exchange)
        {
            if (dateRanges == null
                || !dateRanges.Any()
                || exchange == null)
            {
                return new List<DateRange>();
            }

            var exchangeHolidays = exchange.Holidays ?? new DateTime[0];
            var holidays = exchangeHolidays.Select(y => y.Date).ToList();
            var adjustedHolidays = holidays
                .Select(y => new DateRange(y.Add(exchange.MarketOpenTime), y.Add(exchange.MarketCloseTime)))
                .ToList();

            foreach (var hol in adjustedHolidays)
            {
                dateRanges.RemoveAll(x => x.Intersection(hol));
            }

            return dateRanges;
        }

        /// <summary>
        /// The get exchange.
        /// </summary>
        /// <param name="marketIdentifierCode">
        /// The market identifier code.
        /// </param>
        /// <returns>
        /// The <see cref="ExchangeDto"/>.
        /// </returns>
        private ExchangeDto GetExchange(string marketIdentifierCode)
        {
            if (string.IsNullOrWhiteSpace(marketIdentifierCode))
            {
                this.logger.LogInformation($"received a null or empty MIC {marketIdentifierCode}");
                return null;
            }

            if (string.Equals(marketIdentifierCode, "na", StringComparison.OrdinalIgnoreCase)
                || string.Equals(marketIdentifierCode, "n/a", StringComparison.OrdinalIgnoreCase))
            {
                this.logger.LogDebug($"received an na or n/a MIC {marketIdentifierCode}");
                return null;
            }

            var resultTask = this.marketOpenCloseCachingDecorator.GetAsync();
            var result = resultTask.Result;

            var exchange = result.FirstOrDefault(res => string.Equals(res.Code, marketIdentifierCode, StringComparison.InvariantCultureIgnoreCase));

            if (exchange == null)
            {
                this.logger.LogError($"could not find a match for {marketIdentifierCode}");
                return null;               
            }

            this.logger.LogInformation($"found a match for {marketIdentifierCode}");

            return exchange;
        }
    }
}
