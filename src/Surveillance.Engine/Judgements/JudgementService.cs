using System;
using Domain.Surveillance.Judgement.Equity;
using Domain.Surveillance.Judgement.Equity.Interfaces;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Aurora.Judgements.Interfaces;
using Surveillance.Engine.Rules.Judgements.Equities.Interfaces;
using Surveillance.Engine.Rules.Judgements.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.CancelledOrders.Interfaces;
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
        private readonly IHighProfitJudgementMapper _highProfitJudgementMapper;
        private readonly IRuleViolationService _ruleViolationService;
        private readonly ILogger<JudgementService> _logger;

        public JudgementService(
            IJudgementRepository judgementRepository,
            IRuleViolationService ruleViolationService,
            IHighProfitJudgementMapper highProfitJudgementMapper,
            ILogger<JudgementService> logger)
        {
            _judgementRepository = judgementRepository ?? throw new ArgumentNullException(nameof(judgementRepository));
            _highProfitJudgementMapper = highProfitJudgementMapper ?? throw new ArgumentNullException(nameof(highProfitJudgementMapper));
            _ruleViolationService = ruleViolationService ?? throw new ArgumentNullException(nameof(ruleViolationService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Judgement(IHighProfitJudgementContext judgementContext)
        {
            if (judgementContext == null)
            {
                _logger?.LogError($"High Profit Judgement was null");
                return;
            }

            if (string.IsNullOrWhiteSpace(judgementContext?.Judgement?.OrderId))
            {
                _logger?.LogInformation($"High Profit Judgement had no order id - this is normal for market close analysis");
                return;
            }

            _judgementRepository.Save(judgementContext.Judgement);

            if (!judgementContext.ProjectToAlert)
                return;

            var projectedBreach = _highProfitJudgementMapper.Map(judgementContext);
            _ruleViolationService.AddRuleViolation(projectedBreach);
        }

        public void Judgement(ICancelledOrderJudgement cancelledOrder)
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

        public void Judgement(IHighVolumeJudgement highVolume)
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

        public void Judgement(ILayeringJudgement layering)
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

        public void Judgement(IMarkingTheCloseJudgement markingTheClose)
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

        public void Judgement(IPlacingOrdersWithNoIntentToExecuteJudgement placingOrders)
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

        public void Judgement(IRampingJudgement ramping)
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

        public void Judgement(ISpoofingJudgement spoofing)
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

        public void PassJudgement()
        {
            _ruleViolationService.ProcessRuleViolationCache();
        }
    }
}
