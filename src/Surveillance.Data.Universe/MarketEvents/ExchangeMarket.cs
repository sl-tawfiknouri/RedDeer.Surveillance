namespace Surveillance.Data.Universe.MarketEvents
{
    using System;
    using System.Linq;
    using System.Runtime.InteropServices;

    using RedDeer.Contracts.SurveillanceService.Api.Markets;

    using TimeZoneConverter;

    /// <summary>
    /// The exchange market.
    /// </summary>
    public class ExchangeMarket
    {
        /// <summary>
        /// The dto.
        /// </summary>
        private readonly ExchangeDto dto;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExchangeMarket"/> class.
        /// </summary>
        /// <param name="dto">
        /// The dto.
        /// </param>
        public ExchangeMarket(ExchangeDto dto)
        {
            this.dto = dto;
        }

        /// <summary>
        /// The code.
        /// </summary>
        public string Code => this.dto.Code;

        /// <summary>
        /// The market close time.
        /// </summary>
        public TimeSpan MarketCloseTime => this.dto.MarketCloseTime;

        /// <summary>
        /// The market open time.
        /// </summary>
        public TimeSpan MarketOpenTime => this.dto.MarketOpenTime;

        /// <summary>
        /// The time zone.
        /// </summary>
        public TimeZoneInfo TimeZone => this.TryGetTimeZone();

        /// <summary>
        /// Get the offset for the exchange on a given universal central time date calibrated to 00:00 hours
        /// </summary>
        /// <param name="offsetRelativeTo">
        /// The offset Relative To.
        /// </param>
        /// <returns>
        /// The <see cref="DateTimeOffset"/>.
        /// </returns>
        public DateTimeOffset DateTime(DateTime offsetRelativeTo)
        {
            var timeZoneInfo = this.TryGetTimeZone();
            var offset = timeZoneInfo.GetUtcOffset(offsetRelativeTo);
            var baseDate = new DateTime(offsetRelativeTo.Year, offsetRelativeTo.Month, offsetRelativeTo.Day);
            var offsetTime = new DateTimeOffset(baseDate, offset);

            return offsetTime;
        }

        /// <summary>
        /// The is open on day.
        /// </summary>
        /// <param name="dateTime">
        /// The date time.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsOpenOnDay(DateTime dateTime)
        {
            if (this.dto == null)
            {
                return false;
            }

            if (this.dto.Holidays != null 
                && this.dto.Holidays.Any()
                && this.dto.Holidays.Contains(dateTime))
            {
                return false;
            }

            switch (dateTime.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    return this.dto.IsOpenOnMonday;
                case DayOfWeek.Tuesday:
                    return this.dto.IsOpenOnTuesday;
                case DayOfWeek.Wednesday:
                    return this.dto.IsOpenOnWednesday;
                case DayOfWeek.Thursday:
                    return this.dto.IsOpenOnThursday;
                case DayOfWeek.Friday:
                    return this.dto.IsOpenOnFriday;
                case DayOfWeek.Saturday:
                    return this.dto.IsOpenOnSaturday;
                case DayOfWeek.Sunday:
                    return this.dto.IsOpenOnSunday;
            }

            return false;
        }

        /// <summary>
        /// Added because linux and windows do not share a common language for describing time zones
        /// </summary>
        /// <returns>
        /// The <see cref="TimeZoneInfo"/>.
        /// </returns>
        private TimeZoneInfo TryGetTimeZone()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var windowsTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(this.dto.TimeZone);

                return windowsTimeZoneInfo;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                var countryCode = string.IsNullOrWhiteSpace(this.dto.CountryCode) ? null : this.dto.CountryCode;

                var ianaTimeZone = TZConvert.WindowsToIana(this.dto.TimeZone, countryCode);
                var linuxTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(ianaTimeZone);

                return linuxTimeZoneInfo;
            }

            throw new InvalidOperationException("ExchangeMarket.cs did not recognise the host operating system");
        }
    }
}