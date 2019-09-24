namespace Surveillance.Data.Universe.MarketEvents
{
    using System;
    using System.Linq;
    using System.Runtime.InteropServices;

    using RedDeer.Contracts.SurveillanceService.Api.Markets;

    using TimeZoneConverter;

    public class ExchangeMarket
    {
        private readonly ExchangeDto _dto;

        public ExchangeMarket(ExchangeDto dto)
        {
            this._dto = dto;
        }

        public string Code => this._dto.Code;

        public TimeSpan MarketCloseTime => this._dto.MarketCloseTime;

        public TimeSpan MarketOpenTime => this._dto.MarketOpenTime;

        public TimeZoneInfo TimeZone => this.TryGetTimeZone();

        /// <summary>
        ///     Get the offset for the exchange on a given utc date calibrated to 00:00 hours
        /// </summary>
        public DateTimeOffset DateTime(DateTime offsetRelativeTo)
        {
            var timeZoneInfo = this.TryGetTimeZone();
            var offset = timeZoneInfo.GetUtcOffset(offsetRelativeTo);
            var baseDate = new DateTime(offsetRelativeTo.Year, offsetRelativeTo.Month, offsetRelativeTo.Day);
            var offsetTime = new DateTimeOffset(baseDate, offset);

            return offsetTime;
        }

        public bool IsOpenOnDay(DateTime dateTime)
        {
            if (this._dto == null) return false;

            if (this._dto.Holidays != null && this._dto.Holidays.Any() && this._dto.Holidays.Contains(dateTime))
                return false;

            switch (dateTime.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    return this._dto.IsOpenOnMonday;
                case DayOfWeek.Tuesday:
                    return this._dto.IsOpenOnTuesday;
                case DayOfWeek.Wednesday:
                    return this._dto.IsOpenOnWednesday;
                case DayOfWeek.Thursday:
                    return this._dto.IsOpenOnThursday;
                case DayOfWeek.Friday:
                    return this._dto.IsOpenOnFriday;
                case DayOfWeek.Saturday:
                    return this._dto.IsOpenOnSaturday;
                case DayOfWeek.Sunday:
                    return this._dto.IsOpenOnSunday;
            }

            return false;
        }

        /// <summary>
        ///     Added because linux and windows do not share a common language for describing time zones
        /// </summary>
        private TimeZoneInfo TryGetTimeZone()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var windowsTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(this._dto.TimeZone);

                return windowsTimeZoneInfo;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                var countryCode = string.IsNullOrWhiteSpace(this._dto.CountryCode) ? null : this._dto.CountryCode;

                var ianaTimeZone = TZConvert.WindowsToIana(this._dto.TimeZone, countryCode);
                var linuxTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(ianaTimeZone);

                return linuxTimeZoneInfo;
            }

            throw new InvalidOperationException("ExchangeMarket.cs did not recognise the host operating system");
        }
    }
}