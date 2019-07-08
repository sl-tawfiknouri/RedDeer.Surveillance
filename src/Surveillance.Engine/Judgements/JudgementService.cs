using System;
using Domain.Surveillance.Judgement.Equity;
using Domain.Surveillance.Rules;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Aurora.Judgements.Interfaces;
using Surveillance.Engine.Rules.Judgements.Interfaces;
using Surveillance.Engine.Rules.Rules;
using Surveillance.Engine.Rules.Rules.Equity.CancelledOrders.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.HighProfits;
using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.HighVolume.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.Layering.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.MarkingTheClose.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.PlacingOrderNoIntentToExecute.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.Ramping.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.Spoofing.Interfaces;

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

            var projectedBreach = (IHighProfitRuleBreach) new object();
            _ruleViolationService.AddRuleViolation(projectedBreach);
        }

        public void Judgement(CancelledOrderJudgement cancelledOrder)
        {
            if (cancelledOrder == null)
            {
                _logger?.LogError($"Cancelled Order Judgement was null");
                return;
            }

            _judgementRepository.Save(cancelledOrder);

            // judgement is also a rule breach
            var projectedBreach = (ICancelledOrderRuleBreach)new object();
            _ruleViolationService.AddRuleViolation(projectedBreach);
        }

        public void Judgement(HighVolumeJudgement highVolume)
        {
            if (highVolume == null)
            {
                _logger?.LogError($"High Volume Judgement was null");
                return;
            }

            _judgementRepository.Save(highVolume);

            // judgement is also a rule breach
            var projectedBreach = (IHighVolumeRuleBreach)new object();
            _ruleViolationService.AddRuleViolation(projectedBreach);
        }

        public void Judgement(LayeringJudgement layering)
        {
            if (layering == null)
            {
                _logger?.LogError($"Layering Judgement was null");
                return;
            }

            _judgementRepository.Save(layering);

            // judgement is also a rule breach
            var projectedBreach = (ILayeringRuleBreach)new object();
            _ruleViolationService.AddRuleViolation(projectedBreach);
        }

        public void Judgement(MarkingTheCloseJudgement markingTheClose)
        {
            if (markingTheClose == null)
            {
                _logger?.LogError($"Marking The Close Judgement was null");
                return;
            }

            _judgementRepository.Save(markingTheClose);

            // judgement is also a rule breach
            var projectedBreach = (IMarkingTheCloseBreach)new object();
            _ruleViolationService.AddRuleViolation(projectedBreach);
        }

        public void Judgement(PlacingOrdersWithNoIntentToExecuteJudgement placingOrders)
        {
            if (placingOrders == null)
            {
                _logger?.LogError($"Placing Orders Judgement was null");
                return;
            }

            _judgementRepository.Save(placingOrders);

            // judgement is also a rule breach
            var projectedBreach = (IPlacingOrdersWithNoIntentToExecuteRuleBreach)new object();
            _ruleViolationService.AddRuleViolation(projectedBreach);
        }

        public void Judgement(RampingJudgement ramping)
        {
            if (ramping == null)
            {
                _logger?.LogError($"Ramping Judgement was null");
                return;
            }

            _judgementRepository.Save(ramping);

            // judgement is also a rule breach
            var projectedBreach = (IRampingRuleBreach)new object();
            _ruleViolationService.AddRuleViolation(projectedBreach);
        }

        public void Judgement(SpoofingJudgement spoofing)
        {
            if (spoofing == null)
            {
                _logger?.LogError($"Spoofing Judgement was null");
                return;
            }

            _judgementRepository.Save(spoofing);

            // judgement is also a rule breach
            var projectedBreach = (ISpoofingRuleBreach)new object();
            _ruleViolationService.AddRuleViolation(projectedBreach);
        }
    }
}
