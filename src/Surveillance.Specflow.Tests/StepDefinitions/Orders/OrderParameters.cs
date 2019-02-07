using DomainV2.Financial;
using System;

namespace Surveillance.Specflow.Tests.StepDefinitions.Orders
{
    public class OrderParameters
    {
        public string SecurityName { get; set; }
        public string OrderId { get; set; }
        public DateTime PlacedDate { get; set; }
        public DateTime? BookedDate { get; set; }
        public DateTime? AmendedDate { get; set; }
        public DateTime? RejectedDate { get; set; }
        public DateTime? CancelledDate { get; set; }
        public DateTime? FilledDate { get; set; }
        public OrderTypes Type { get; set; }
        public OrderDirections Direction { get; set; }
        public string Currency { get; set; }
        public decimal? LimitPrice { get; set; }
        public decimal? AverageFillPrice { get; set; }
        public int OrderedVolume { get; set; }
        public int FilledVolume { get; set; }
    }
}
