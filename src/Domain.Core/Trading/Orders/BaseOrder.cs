namespace Domain.Core.Trading.Orders
{
    using System;

    using Domain.Core.Trading.Orders.Interfaces;

    public abstract class BaseOrder : IBaseOrder
    {
        protected BaseOrder(
            DateTime? placedDate,
            DateTime? bookedDate,
            DateTime? amendedDate,
            DateTime? rejectedDate,
            DateTime? cancelledDate,
            DateTime? filledDate)
        {
            this.PlacedDate = placedDate;
            this.BookedDate = bookedDate;
            this.AmendedDate = amendedDate;
            this.RejectedDate = rejectedDate;
            this.CancelledDate = cancelledDate;
            this.FilledDate = filledDate;
        }

        public DateTime? AmendedDate { get; set; }

        public DateTime? BookedDate { get; set; }

        public DateTime? CancelledDate { get; set; }

        public DateTime? FilledDate { get; set; }

        // Dates
        public DateTime? PlacedDate { get; set; }

        public DateTime? RejectedDate { get; set; }

        /// <summary>
        ///     Captures trade life cycle status
        /// </summary>
        public OrderStatus OrderStatus()
        {
            if (this.CancelledDate != null) return Orders.OrderStatus.Cancelled;

            if (this.RejectedDate != null) return Orders.OrderStatus.Rejected;

            if (this.FilledDate != null) return Orders.OrderStatus.Filled;

            if (this.AmendedDate != null) return Orders.OrderStatus.Amended;

            if (this.BookedDate != null) return Orders.OrderStatus.Booked;

            if (this.PlacedDate != null) return Orders.OrderStatus.Placed;

            return Orders.OrderStatus.Unknown;
        }
    }
}