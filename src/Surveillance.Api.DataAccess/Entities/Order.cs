namespace Surveillance.Api.DataAccess.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Domain.Core.Trading.Orders;

    using Surveillance.Api.DataAccess.Abstractions.Entities;

    public class Order : IOrder
    {
        public decimal? AccumulatedInterest { get; set; }

        public DateTime? AmendedDate { get; set; }

        public bool Autoscheduled { get; set; }

        public decimal? AverageFillPrice { get; set; }

        public DateTime? BookedDate { get; set; }

        public int? BrokerId { get; set; }

        public DateTime? CancelledDate { get; set; }

        public string CleanDirty { get; set; }

        public string ClearingAgent { get; set; }

        [NotMapped]
        public string ClientAccount { get; set; }

        public string ClientOrderId { get; set; }

        public DateTime CreatedDate { get; set; }

        public string Currency { get; set; }

        public string DealingInstructions { get; set; }

        public int Direction { get; set; }

        public DateTime? FilledDate { get; set; }

        public long? FilledVolume { get; set; }

        [ForeignKey("SecurityId")]
        public FinancialInstrument FinancialInstrument { get; set; }

        [NotMapped]
        public string Fund { get; set; }

        [Key]
        public int Id { get; set; }

        public int? LifeCycleStatus { get; set; }

        public decimal? LimitPrice { get; set; }

        public bool Live { get; set; }

        public int MarketId { get; set; }

        [NotMapped]
        public IOrderManagementSystem Oms =>
            new OrderManagementSystem(this.OrderVersion, this.OrderVersionLinkId, this.OrderGroupId);

        public string OptionEuropeanAmerican { get; set; }

        public DateTime? OptionExpirationDate { get; set; }

        public decimal? OptionStrikePrice { get; set; }

        [NotMapped]
        public IOrderDates OrderDates =>
            new OrderDates(
                this.PlacedDate,
                this.BookedDate,
                this.AmendedDate,
                this.RejectedDate,
                this.CancelledDate,
                this.FilledDate,
                this.StatusChangedDate);

        public OrderDirections OrderDirection => (OrderDirections)this.Direction;

        public long? OrderedVolume { get; set; }

        public string OrderGroupId { get; set; }

        public int OrderType { get; set; }

        public OrderTypes OrderTypes => (OrderTypes)this.OrderType;

        public string OrderVersion { get; set; }

        public string OrderVersionLinkId { get; set; }

        public DateTime? PlacedDate { get; set; }

        public DateTime? RejectedDate { get; set; }

        public int? SecurityId { get; set; }

        public string SettlementCurrency { get; set; }

        public DateTime? StatusChangedDate { get; set; }

        [NotMapped]
        public string Strategy { get; set; }

        [NotMapped]
        public ITrader Trader => new Trader { Id = this.TraderId, Name = this.TraderName };

        public string TraderId { get; set; }

        public string TraderName { get; set; }

        IFinancialInstrument IOrder.FinancialInstrument => this.FinancialInstrument;

        IOrderDates IOrder.OrderDates => this.OrderDates;

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