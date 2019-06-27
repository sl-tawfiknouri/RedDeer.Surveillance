using System;
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
            PlacedDate = placedDate;
            BookedDate = bookedDate;
            AmendedDate = amendedDate;
            RejectedDate = rejectedDate;
            CancelledDate = cancelledDate;
            FilledDate = filledDate;
            StatusChangedDate = statusChangedDate;
        }

        public DateTime? PlacedDate { get; set; }
        public DateTime? BookedDate { get; set; }
        public DateTime? AmendedDate { get; set; }
        public DateTime? RejectedDate { get; set; }
        public DateTime? CancelledDate { get; set; }
        public DateTime? FilledDate { get; set; }
        public DateTime? StatusChangedDate { get; set; }
    }
}
