namespace Surveillance.Api.DataAccess.Entities
{
    using System;

    using Surveillance.Api.DataAccess.Abstractions.Entities;

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
            this.PlacedDate = placedDate;
            this.BookedDate = bookedDate;
            this.AmendedDate = amendedDate;
            this.RejectedDate = rejectedDate;
            this.CancelledDate = cancelledDate;
            this.FilledDate = filledDate;
            this.StatusChangedDate = statusChangedDate;
        }

        public DateTime? AmendedDate { get; set; }

        public DateTime? BookedDate { get; set; }

        public DateTime? CancelledDate { get; set; }

        public DateTime? FilledDate { get; set; }

        public DateTime? PlacedDate { get; set; }

        public DateTime? RejectedDate { get; set; }

        public DateTime? StatusChangedDate { get; set; }
    }
}