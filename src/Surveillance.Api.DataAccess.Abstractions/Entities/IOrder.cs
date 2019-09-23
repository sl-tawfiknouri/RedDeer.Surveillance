namespace Surveillance.Api.DataAccess.Abstractions.Entities
{
    using System;

    using Domain.Core.Trading.Orders;

    public interface IOrder : ICloneable
    {
        decimal? AccumulatedInterest { get; }

        DateTime? AmendedDate { get; }

        bool Autoscheduled { get; }

        decimal? AverageFillPrice { get; }

        DateTime? BookedDate { get; }

        int? BrokerId { get; }

        DateTime? CancelledDate { get; }

        string CleanDirty { get; }

        string ClearingAgent { get; }

        string ClientAccount { get; }

        string ClientOrderId { get; }

        DateTime CreatedDate { get; }

        string Currency { get; }

        string DealingInstructions { get; }

        int Direction { get; }

        DateTime? FilledDate { get; }

        long? FilledVolume { get; }

        IFinancialInstrument FinancialInstrument { get; }

        string Fund { get; }

        int Id { get; }

        int? LifeCycleStatus { get; }

        decimal? LimitPrice { get; }

        bool Live { get; }

        int MarketId { get; }

        /// <summary>
        ///     This is used - don't remove it
        /// </summary>
        IOrderManagementSystem Oms { get; }

        string OptionEuropeanAmerican { get; }

        DateTime? OptionExpirationDate { get; }

        decimal? OptionStrikePrice { get; }

        IOrderDates OrderDates { get; }

        OrderDirections OrderDirection { get; }

        long? OrderedVolume { get; }

        string OrderGroupId { get; }

        int OrderType { get; }

        /// <summary>
        ///     This is used - don't remove it
        /// </summary>
        OrderTypes OrderTypes { get; }

        string OrderVersion { get; }

        string OrderVersionLinkId { get; }

        DateTime? PlacedDate { get; }

        DateTime? RejectedDate { get; }

        int? SecurityId { get; }

        string SettlementCurrency { get; }

        string Strategy { get; }

        ITrader Trader { get; }

        string TraderId { get; }

        string TraderName { get; }
    }
}