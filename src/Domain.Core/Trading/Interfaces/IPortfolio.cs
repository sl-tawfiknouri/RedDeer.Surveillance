using System;
using System.Collections.Generic;
using Domain.Core.Accounts;
using Domain.Core.Trading.Orders;

namespace Domain.Core.Trading.Interfaces
{
    public interface IPortfolio
    {
        IPortfolioExposure PortfolioExposure { get; }
        ITradingExposure TradingExposure { get; }

        IOrderLedger Ledger { get; }

        IReadOnlyCollection<ProfitAndLossStatement> ProfitAndLossTotal();
        IReadOnlyCollection<ProfitAndLossStatement> ProfitAndLoss(DateTime from, TimeSpan span);

        BalanceSheetStatement BalanceSheets();

        void Add(IReadOnlyCollection<Order> orders);
        void Add(Order order);
    }
}