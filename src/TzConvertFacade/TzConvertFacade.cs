using System;
using System.Runtime.InteropServices;
using TimeZoneConverter;

namespace TzConvertFacade
{
    public static class TzConvertFacade
    {
        /// <summary>
        /// Added because linux and windows do not share a common language for describing time zones
        /// </summary>
        /// <returns>
        /// The <see cref="TimeZoneInfo"/>.
        /// </returns>
        public static TimeZoneInfo TryGetTimeZone(string timezone, string countryCode)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var windowsTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timezone);

                return windowsTimeZoneInfo;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                if (string.IsNullOrWhiteSpace(countryCode))
                {
                    countryCode = null;
                }

                var ianaTimeZone = ConvertWindowsToIana(timezone, countryCode);
                var linuxTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(ianaTimeZone);

                return linuxTimeZoneInfo;
            }

            throw new InvalidOperationException("ExchangeMarket.cs did not recognise the host operating system");
        }

        /// <summary>
        /// Ideally we would use library that has the same breadth as the factset data source does for different timezones
        /// We know that their format is windows-like but has additional entries
        /// This is a plaster - replace it with a good timezone library
        /// </summary>
        private static string ConvertWindowsToIana(string timezone, string countryCode)
        {
            if (string.Equals(timezone, "AUS Eastern Daylight Time", StringComparison.OrdinalIgnoreCase))
            {
                timezone = "AUS Eastern Standard Time";
            }

            var ianaTimeZone = TZConvert.WindowsToIana(timezone, countryCode);

            return ianaTimeZone;
        }
    }
}
