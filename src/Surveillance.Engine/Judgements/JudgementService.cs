using System;
using Domain.Surveillance.Judgements.Equity;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Aurora.Judgements.Interfaces;
using Surveillance.Engine.Rules.Judgements.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Interfaces;

namespace Surveillance.Engine.Rules.Judgements
{
    public class JudgementService : IJudgementService
    {
        private readonly IJudgementRepository _judgementRepository;
        private readonly IHighProfitRuleCachedMessageSender _highProfitCachedMessageSender;
        private readonly IRuleViolationService _ruleViolationService;

        private readonly ILogger<JudgementService> _logger;

        public JudgementService(
            IJudgementRepository judgementRepository,
            IHighProfitRuleCachedMessageSender highProfitCachedRuleCachedMessageSender,
            IRuleViolationService ruleViolationService,
            ILogger<JudgementService> logger)
        {
            _judgementRepository = judgementRepository ?? throw new ArgumentNullException(nameof(judgementRepository));
            _highProfitCachedMessageSender = highProfitCachedRuleCachedMessageSender ?? throw new ArgumentNullException(nameof(highProfitCachedRuleCachedMessageSender));
            _ruleViolationService = ruleViolationService ?? throw new ArgumentNullException(nameof(ruleViolationService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Judgement(HighProfitJudgement highProfit)
        {
            if (highProfit == null)
            {
                _logger?.LogError($"High Profit Judgement was null");
                return;
            }

            // save judgement
            _judgementRepository.Save(highProfit);

            if (!highProfit.ProjectToAlert)
                return;

            // judgement is also a rule breach
            var lol = (IHighProfitRuleBreach)new object();

            _highProfitCachedMessageSender.Send(lol);
            _highProfitCachedMessageSender.Flush();



            _ruleViolationService.AddRuleViolation(lol);
        }

        public void Judgement(CancelledOrderJudgement cancelledOrder)
        {
            if (cancelledOrder == null)
            {
                _logger?.LogError($"Cancelled Order Judgement was null");
                return;
            }

            _judgementRepository.Save(cancelledOrder);
        }

        public void Judgement(HighVolumeJudgement highVolume)
        {
            if (highVolume == null)
            {
                _logger?.LogError($"High Volume Judgement was null");
                return;
            }

            _judgementRepository.Save(highVolume);
        }

        public void Judgement(LayeringJudgement layering)
        {
            if (layering == null)
            {
                _logger?.LogError($"Layering Judgement was null");
                return;
            }

            _judgementRepository.Save(layering);
        }

        public void Judgement(MarkingTheCloseJudgement markingTheClose)
        {
            if (markingTheClose == null)
            {
                _logger?.LogError($"Marking The Close Judgement was null");
                return;
            }

            _judgementRepository.Save(markingTheClose);
        }

        public void Judgement(PlacingOrdersWithNoIntentToExecuteJudgement placingOrders)
        {
            if (placingOrders == null)
            {
                _logger?.LogError($"Placing Orders Judgement was null");
                return;
            }

            _judgementRepository.Save(placingOrders);
        }

        public void Judgement(RampingJudgement ramping)
        {
            if (ramping == null)
            {
                _logger?.LogError($"Ramping Judgement was null");
                return;
            }

            _judgementRepository.Save(ramping);
        }

        public void Judgement(SpoofingJudgement spoofing)
        {
            if (spoofing == null)
            {
                _logger?.LogError($"Spoofing Judgement was null");
                return;
            }

            _judgementRepository.Save(spoofing);
        }
    }
}
