using Domain.Trading;
using System;

namespace Domain.Core.Trading
{
    /// <summary>
    /// Aggregate root (DDD)
    /// </summary>
    public class Portfolio
    {
        private ProfitAndLossAccountService _profitAndLossService;
        private BalanceSheetAccountService _balanceSheetService;

        public Portfolio(
            Holdings holding,
            OrderLedger ledger,
            ProfitAndLossAccountService profitAndLossService,
            BalanceSheetAccountService balanceSheetService)
        {
            Holdings = holding ?? throw new ArgumentNullException(nameof(holding));
            Ledger = ledger ?? throw new ArgumentNullException(nameof(ledger));
            _profitAndLossService = profitAndLossService ?? throw new ArgumentNullException(nameof(profitAndLossService));
            _balanceSheetService = balanceSheetService ?? throw new ArgumentNullException(nameof(balanceSheetService));
        }

        public Holdings Holdings { get; }

        public OrderLedger Ledger { get; }

        public ProfitAndLossStatement Accounts()
        {
            return null;
        }

        public BalanceSheetStatement BalanceSheets()
        {
            return null;
        }

        public void Add(Order order)
        {
            // add to ledger
            // add to holdings
            // update accounts
        }
    }
}
