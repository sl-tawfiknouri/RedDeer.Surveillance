using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Surveillance.Api.Client.Dtos
{
    public class OrderDatesDto
    {
        public string AmendedDate { get; set; }
        public string BookedDate { get; set; }
        public string CancelledDate { get; set; }
        public string FilledDate { get; set; }
        public string PlacedDate { get; set; }
        public string RejectedDate { get; set; }
        public string StatusChangedDate { get; set; }

        public DateTime? Amended => string.IsNullOrEmpty(AmendedDate) ? (DateTime?)null : DateTime.Parse(AmendedDate, CultureInfo.GetCultureInfo("en-GB"));
        public DateTime? Booked => string.IsNullOrEmpty(BookedDate) ? (DateTime?)null : DateTime.Parse(BookedDate, CultureInfo.GetCultureInfo("en-GB"));
        public DateTime? Cancelled => string.IsNullOrEmpty(CancelledDate) ? (DateTime?)null : DateTime.Parse(CancelledDate, CultureInfo.GetCultureInfo("en-GB"));
        public DateTime? Filled => string.IsNullOrEmpty(FilledDate) ? (DateTime?)null : DateTime.Parse(FilledDate, CultureInfo.GetCultureInfo("en-GB"));
        public DateTime? Placed => string.IsNullOrEmpty(PlacedDate) ? (DateTime?)null : DateTime.Parse(PlacedDate, CultureInfo.GetCultureInfo("en-GB"));
        public DateTime? Rejected => string.IsNullOrEmpty(RejectedDate) ? (DateTime?)null : DateTime.Parse(RejectedDate, CultureInfo.GetCultureInfo("en-GB"));
        public DateTime? StatusChanged => string.IsNullOrEmpty(StatusChangedDate) ? (DateTime?)null : DateTime.Parse(StatusChangedDate, CultureInfo.GetCultureInfo("en-GB"));
    }
}
