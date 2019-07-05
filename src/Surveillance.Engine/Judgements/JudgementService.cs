using System;
using Domain.Surveillance.Judgements.Equity;
using Microsoft.Extensions.Logging;

namespace Surveillance.Engine.Rules.Judgements
{
    public class JudgementService
    {
        private readonly ILogger<JudgementService> _logger;

        public JudgementService(ILogger<JudgementService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Judgement(HighProfitJudgement highProfit)
        {
            if (highProfit == null)
            {
                _logger?.LogError($"High Profit Judgement was null");
                return;
            }
        }

        public void Judgement(CancelledOrderJudgement cancelledOrder)
        {
            if (cancelledOrder == null)
            {
                _logger?.LogError($"Cancelled Order Judgement was null");
                return;
            }
        }

        public void Judgement(HighVolumeJudgement highVolume)
        {
            if (highVolume == null)
            {
                _logger?.LogError($"High Volume Judgement was null");
                return;
            }
        }

        public void Judgement(LayeringJudgement layering)
        {
            if (layering == null)
            {
                _logger?.LogError($"Layering Judgement was null");
                return;
            }
        }

        public void Judgement(MarkingTheCloseJudgement markingTheClose)
        {
            if (markingTheClose == null)
            {
                _logger?.LogError($"Marking The Close Judgement was null");
                return;
            }
        }

        public void Judgement(PlacingOrdersWithNoIntentToExecuteJudgement placingOrders)
        {
            if (placingOrders == null)
            {
                _logger?.LogError($"Placing Orders Judgement was null");
                return;
            }
        }

        public void Judgement(RampingJudgement ramping)
        {
            if (ramping == null)
            {
                _logger?.LogError($"Ramping Judgement was null");
                return;
            }
        }

        public void Judgement(SpoofingJudgement spoofing)
        {
            if (spoofing == null)
            {
                _logger?.LogError($"Spoofing Judgement was null");
                return;
            }
        }



    }
}
