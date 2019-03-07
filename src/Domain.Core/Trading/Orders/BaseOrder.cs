using System;
using Domain.Core.Trading.Orders.Interfaces;

namespace Domain.Core.Trading.Orders
{
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
            PlacedDate = placedDate;
            BookedDate = bookedDate;
            AmendedDate = amendedDate;
            RejectedDate = rejectedDate;
            CancelledDate = cancelledDate;
            FilledDate = filledDate;
        }
        
        // Dates
        public DateTime? PlacedDate { get; set; }
        public DateTime? BookedDate { get; set; }
        public DateTime? AmendedDate { get; set; }
        public DateTime? RejectedDate { get; set; }
        public DateTime? CancelledDate { get; set; }
        public DateTime? FilledDate { get; set; }

        /// <summary>
        /// Captures trade life cycle status
        /// </summary>
        public OrderStatus OrderStatus()
        {
            if (CancelledDate != null)
            {
                return Orders.OrderStatus.Cancelled;
            }

            if (RejectedDate != null)
            {
                return Orders.OrderStatus.Rejected;
            }

            if (FilledDate != null)
            {
                return Orders.OrderStatus.Filled;
            }

            if (AmendedDate != null)
            {
                return Orders.OrderStatus.Amended;
            }

            if (BookedDate != null)
            {
                return Orders.OrderStatus.Booked;
            }

            if (PlacedDate != null)
            {
                return Orders.OrderStatus.Placed;
            }

            return Orders.OrderStatus.Unknown;
        }
    }
}
