namespace Surveillance.Specflow.Tests.StepDefinitions.Orders
{
    using System;

    using Domain.Core.Trading.Orders;

    public class OrderParameters
    {
        public DateTime? AmendedDate { get; set; }

        public decimal? AverageFillPrice { get; set; }

        public DateTime? BookedDate { get; set; }

        public DateTime? CancelledDate { get; set; }

        public string Currency { get; set; }

        public OrderDirections Direction { get; set; }

        public DateTime? FilledDate { get; set; }

        public int FilledVolume { get; set; }

        public decimal? LimitPrice { get; set; }

        public int OrderedVolume { get; set; }

        public string OrderId { get; set; }

        public DateTime PlacedDate { get; set; }

        public DateTime? RejectedDate { get; set; }

        public string SecurityName { get; set; }

        public OrderTypes Type { get; set; }

        public OrderCleanDirty CleanOrDirty { get; set; } = OrderCleanDirty.NONE;
    }
}