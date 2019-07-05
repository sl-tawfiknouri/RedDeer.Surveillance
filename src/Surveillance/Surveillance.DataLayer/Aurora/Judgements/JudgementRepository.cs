using System;
using Domain.Surveillance.Judgements.Equity;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Aurora.Interfaces;
using Surveillance.DataLayer.Aurora.Judgements.Interfaces;

namespace Surveillance.DataLayer.Aurora.Judgements
{
    public class JudgementRepository : IJudgementRepository
    {
        private readonly IConnectionStringFactory _dbConnectionFactory;
        private readonly ILogger<JudgementRepository> _logger;

        public JudgementRepository(
            IConnectionStringFactory dbConnectionFactory,
            ILogger<JudgementRepository> logger)
        {
            _dbConnectionFactory = dbConnectionFactory ?? throw new ArgumentNullException(nameof(dbConnectionFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Save(HighProfitJudgement highProfit)
        {
            if (highProfit == null)
            {
                _logger?.LogError($"High Profit Judgement was null");
                return;
            }
        }

        public void Save(CancelledOrderJudgement cancelledOrder)
        {
            if (cancelledOrder == null)
            {
                _logger?.LogError($"Cancelled Order Judgement was null");
                return;
            }
        }

        public void Save(HighVolumeJudgement highVolume)
        {
            if (highVolume == null)
            {
                _logger?.LogError($"High Volume Judgement was null");
                return;
            }
        }

        public void Save(LayeringJudgement layering)
        {
            if (layering == null)
            {
                _logger?.LogError($"Layering Judgement was null");
                return;
            }
        }

        public void Save(MarkingTheCloseJudgement markingTheClose)
        {
            if (markingTheClose == null)
            {
                _logger?.LogError($"Marking The Close Judgement was null");
                return;
            }
        }

        public void Save(PlacingOrdersWithNoIntentToExecuteJudgement placingOrders)
        {
            if (placingOrders == null)
            {
                _logger?.LogError($"Placing Orders Judgement was null");
                return;
            }
        }

        public void Save(RampingJudgement ramping)
        {
            if (ramping == null)
            {
                _logger?.LogError($"Ramping Judgement was null");
                return;
            }
        }

        public void Save(SpoofingJudgement spoofing)
        {
            if (spoofing == null)
            {
                _logger?.LogError($"Spoofing Judgement was null");
                return;
            }
        }



    }
}
