using System;
using System.Globalization;
using Surveillance.Api.DataAccess.Abstractions.Entities;

namespace Surveillance.Api.DataAccess.Entities
{
    public class OrderDates : IOrderDates
    {
        public OrderDates(
            DateTime? placedDate,
            DateTime? bookedDate,
            DateTime? amendedDate,
            DateTime? rejectedDate,
            DateTime? cancelledDate,
            DateTime? filledDate,
            DateTime? statusChangedDate)
        {
            PlacedDate = placedDate?.ToString(CultureInfo.GetCultureInfo("en-GB")) ?? string.Empty;
            BookedDate = bookedDate?.ToString(CultureInfo.GetCultureInfo("en-GB")) ?? string.Empty;
            AmendedDate = amendedDate?.ToString(CultureInfo.GetCultureInfo("en-GB")) ?? string.Empty;
            RejectedDate = rejectedDate?.ToString(CultureInfo.GetCultureInfo("en-GB")) ?? string.Empty;
            CancelledDate = cancelledDate?.ToString(CultureInfo.GetCultureInfo("en-GB")) ?? string.Empty;
            FilledDate = filledDate?.ToString(CultureInfo.GetCultureInfo("en-GB")) ?? string.Empty;
            StatusChangedDate = statusChangedDate?.ToString(CultureInfo.GetCultureInfo("en-GB")) ?? string.Empty;
        }

        public OrderDates(
            string placedDate,
            string bookedDate,
            string amendedDate,
            string rejectedDate,
            string cancelledDate,
            string filledDate,
            string statusChangedDate)
        {
            PlacedDate = placedDate ?? string.Empty;
            BookedDate = bookedDate ?? string.Empty;
            AmendedDate = amendedDate ?? string.Empty;
            RejectedDate = rejectedDate ?? string.Empty;
            CancelledDate = cancelledDate ?? string.Empty;
            FilledDate = filledDate ?? string.Empty;
            StatusChangedDate = statusChangedDate ?? string.Empty;
        }

        public string PlacedDate { get; set; }
        public string BookedDate { get; set; }
        public string AmendedDate { get; set; }
        public string RejectedDate { get; set; }
        public string CancelledDate { get; set; }
        public string FilledDate { get; set; }
        public string StatusChangedDate { get; set; }
    }
}
