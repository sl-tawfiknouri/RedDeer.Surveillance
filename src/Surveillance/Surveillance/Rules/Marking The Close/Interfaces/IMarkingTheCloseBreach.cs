using Surveillance.DataLayer.Stub;
using Surveillance.Rules.Interfaces;

namespace Surveillance.Rules.Marking_The_Close.Interfaces
{
    public interface IMarkingTheCloseBreach : IRuleBreach
    {
        bool HasBuyDailyVolumeBreach { get; }
        /// <summary>
        /// A fractional representation of the % of daily liquidity in buy orders within the time window
        /// </summary>
        decimal? BuyDailyVolumeBreach { get; }

        bool HasSellDailyVolumeBreach { get; }
        /// <summary>
        /// A fractional representation of the % of daily liquidity in sell orders within the time window
        /// </summary>
        decimal? SellDailyVolumeBreach { get; }

        MarketOpenClose MarketClose { get; }
        IMarkingTheCloseParameters Parameters { get; }
    }
}