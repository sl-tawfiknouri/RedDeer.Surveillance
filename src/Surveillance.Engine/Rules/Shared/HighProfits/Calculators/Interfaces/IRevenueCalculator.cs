namespace Surveillance.Engine.Rules.Rules.Shared.HighProfits.Calculators.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain.Core.Trading.Orders;
    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.Markets.Interfaces;

    /// <summary>
    /// The RevenueCalculator interface.
    /// </summary>
    public interface IRevenueCalculator
    {
        /// <summary>
        /// The calculate revenue of position.
        /// </summary>
        /// <param name="activeFulfilledTradeOrders">
        /// The active fulfilled trade orders.
        /// </param>
        /// <param name="universeDateTime">
        /// The universe date time.
        /// </param>
        /// <param name="ruleRunContext">
        /// The rule run context.
        /// </param>
        /// <param name="cacheStrategy">
        /// The cache strategy.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<RevenueMoney> CalculateRevenueOfPosition(
            IList<Order> activeFulfilledTradeOrders,
            DateTime universeDateTime,
            ISystemProcessOperationRunRuleContext ruleRunContext,
            IMarketDataCacheStrategy cacheStrategy);
    }
}