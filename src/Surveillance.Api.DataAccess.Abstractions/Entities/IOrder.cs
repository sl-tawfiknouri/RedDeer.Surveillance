using System;
using Domain.Core.Trading.Orders;

namespace Surveillance.Api.DataAccess.Abstractions.Entities
{
    public interface IOrder : ICloneable
    {
        decimal? AccumulatedInterest { get;  }
        bool Autoscheduled { get;  }
        decimal? AverageFillPrice { get;  }
        string CleanDirty { get;  }
        string ClearingAgent { get;  }
        string ClientOrderId { get;  }
        DateTime CreatedDate { get;  }
        string Currency { get;  }
        string DealingInstructions { get;  }
        int Direction { get;  }
        OrderDirections OrderDirection { get; }
        long? FilledVolume { get;  }
        int Id { get;  }
        int? LifeCycleStatus { get;  }
        decimal? LimitPrice { get;  }
        bool Live { get;  }
        int MarketId { get;  }

        /// <summary>
        /// This is used - don't remove it
        /// </summary>
        IOrderManagementSystem Oms { get;  }
        string OptionEuropeanAmerican { get;  }
        DateTime? OptionExpirationDate { get;  }
        decimal? OptionStrikePrice { get;  }
        IOrderDates OrderDates { get;  }
        long? OrderedVolume { get;  }
        int OrderType { get;  }

        /// <summary>
        /// This is used - don't remove it
        /// </summary>
        OrderTypes OrderTypes { get; }
        int? SecurityId { get;  }
        IFinancialInstrument FinancialInstrument { get; }
        string SettlementCurrency { get;  }
        ITrader Trader { get; }
        string TraderId { get; }
        string TraderName { get; }
        string Fund { get; }
        string Strategy { get; }
        string ClientAccount { get; }
        string OrderVersion { get; }
        string OrderVersionLinkId { get; }
        string OrderGroupId { get; }
        DateTime? PlacedDate { get; }
        DateTime? BookedDate { get; }
        DateTime? AmendedDate { get; }
        DateTime? RejectedDate { get; }
        DateTime? CancelledDate { get; }
        DateTime? FilledDate { get; }

        int? BrokerId { get; }
    }
}