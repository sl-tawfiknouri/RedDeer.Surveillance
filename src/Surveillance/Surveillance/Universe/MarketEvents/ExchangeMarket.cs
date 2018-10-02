﻿using System;
using System.Linq;
using RedDeer.Contracts.SurveillanceService.Api.Markets;

namespace Surveillance.Universe.MarketEvents
{
    public class ExchangeMarket
    {
        private readonly ExchangeDto _dto;
        public ExchangeMarket(ExchangeDto dto)
        {
            _dto = dto;
        }

        public string Code => _dto.Code;
        public TimeSpan MarketOpenTime => _dto.MarketOpenTime;
        public TimeSpan MarketCloseTime => _dto.MarketCloseTime;
        public TimeZoneInfo TimeZone => TryGetTimeZone(_dto.TimeZone);

        public bool IsOpenOnDay(DateTime dateTime)
        {
            if (_dto == null)
            {
                return false;
            }

            if (_dto.Holidays != null
                && _dto.Holidays.Any()
                && _dto.Holidays.Contains(dateTime))
            {
                return false;
            }

            switch (dateTime.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    return _dto.IsOpenOnMonday;
                case DayOfWeek.Tuesday:
                    return _dto.IsOpenOnTuesday;
                case DayOfWeek.Wednesday:
                    return _dto.IsOpenOnWednesday;
                case DayOfWeek.Thursday:
                    return _dto.IsOpenOnThursday;
                case DayOfWeek.Friday:
                    return _dto.IsOpenOnFriday;
                case DayOfWeek.Saturday:
                    return _dto.IsOpenOnSaturday;
                case DayOfWeek.Sunday:
                    return _dto.IsOpenOnSunday;
            }

            return false;
        }

        /// <summary>
        /// Get the offset for the exchange on a given utc date calibrated to 00:00 hours
        /// </summary>
        public DateTimeOffset DateTime(DateTime offsetRelativeTo)
        {
            var timeZoneInfo = TryGetTimeZone(_dto.TimeZone);
            var offset = timeZoneInfo.GetUtcOffset(offsetRelativeTo);
            var baseDate = new DateTime(offsetRelativeTo.Year, offsetRelativeTo.Month, offsetRelativeTo.Day);
            var offsetTime = new DateTimeOffset(baseDate, offset);

            return offsetTime;
        }

        /// <summary>
        /// Added because linux and windows do not share a common language for describing time zones
        /// </summary>
        private TimeZoneInfo TryGetTimeZone(string timeZone)
        {


            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(_dto.TimeZone);

            return timeZoneInfo;
        }
    }
}