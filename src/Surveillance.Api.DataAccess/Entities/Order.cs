using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Core.Trading.Orders;
using Surveillance.Api.DataAccess.Abstractions.Entities;

namespace Surveillance.Api.DataAccess.Entities
{
    public class Order : IOrder
    {
        public int MarketId { get; set; }


        [Key]
        public int Id { get; set; }


        public OrderTypes OrderTypes => (OrderTypes)OrderType;
        public OrderDirections OrderDirection => (OrderDirections)Direction;

        public int? SecurityId { get; set; }

        [ForeignKey("SecurityId")]
        public FinancialInstrument FinancialInstrument { get; set; }

        public string ClientOrderId { get; set; }

        public string OrderVersion { get; set; }
        public string OrderVersionLinkId { get; set; }
        public string OrderGroupId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? PlacedDate { get; set; }
        public DateTime? BookedDate { get; set; }
        public DateTime? AmendedDate { get; set; }
        public DateTime? RejectedDate { get; set; }
        public DateTime? CancelledDate { get; set; }
        public DateTime? FilledDate { get; set; }
        public DateTime? StatusChangedDate { get; set; }
        public int OrderType { get; set; }
        public int Direction { get; set; }
        public string Currency { get; set; }
        public string SettlementCurrency { get; set; }
        public string CleanDirty { get; set; }
        public decimal? AccumulatedInterest { get; set; }
        public decimal? LimitPrice { get; set; }
        public decimal? AverageFillPrice { get; set; }
        public long? OrderedVolume { get; set; }
        public long? FilledVolume { get; set; }
        public string TraderId { get; set; }
        public string TraderName { get; set; }
        public string ClearingAgent { get; set; }
        public string DealingInstructions { get; set; }
        public decimal? OptionStrikePrice { get; set; }
        public DateTime? OptionExpirationDate { get; set; }
        public string OptionEuropeanAmerican { get; set; }
        public int? LifeCycleStatus { get; set; }
        public bool Live { get; set; }
        public bool Autoscheduled { get; set; }

        [NotMapped]
        public IOrderManagementSystem Oms
        {
            get => new OrderManagementSystem(OrderVersion, OrderVersionLinkId, OrderGroupId);
        }

        [NotMapped]
        public IOrderDates OrderDates
        {
            get => new OrderDates(PlacedDate, BookedDate, AmendedDate, RejectedDate, CancelledDate, FilledDate, StatusChangedDate);
        }
        
        [NotMapped]
        public ITrader Trader
        {
            get => new Trader { Id = TraderId, Name = TraderName };
        }

        [NotMapped]
        public string Fund { get; set; }
        [NotMapped]
        public string Strategy { get; set; }
        [NotMapped]
        public string ClientAccount { get; set; }

        IOrderDates IOrder.OrderDates
        {
            get => OrderDates;
        }

        IFinancialInstrument IOrder.FinancialInstrument
        {
            get => FinancialInstrument;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        object ICloneable.Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
