using System;
using Domain.Core.Financial;
using Domain.Trading.Interfaces;

namespace Domain.Trading
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
                return Core.Financial.OrderStatus.Cancelled;
            }

            if (RejectedDate != null)
            {
                return Core.Financial.OrderStatus.Rejected;
            }

            if (FilledDate != null)
            {
                return Core.Financial.OrderStatus.Filled;
            }

            if (AmendedDate != null)
            {
                return Core.Financial.OrderStatus.Amended;
            }

            if (BookedDate != null)
            {
                return Core.Financial.OrderStatus.Booked;
            }

            if (PlacedDate != null)
            {
                return Core.Financial.OrderStatus.Placed;
            }

            return Core.Financial.OrderStatus.Unknown;
        }
    }
}
