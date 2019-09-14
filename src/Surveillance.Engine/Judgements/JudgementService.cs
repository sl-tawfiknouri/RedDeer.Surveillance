namespace Surveillance.Engine.Rules.Judgements
{
    using System;

    using Domain.Surveillance.Judgement.Equity.Interfaces;

    using Microsoft.Extensions.Logging;

    using Surveillance.DataLayer.Aurora.Judgements.Interfaces;
    using Surveillance.Engine.Rules.Judgements.Equities.Interfaces;
    using Surveillance.Engine.Rules.Judgements.FixedIncome.Interfaces;
    using Surveillance.Engine.Rules.Judgements.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.CancelledOrders.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.HighVolume.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.Layering.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.MarkingTheClose.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.PlacingOrderNoIntentToExecute.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.Ramping.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.Spoofing.Interfaces;

    public class JudgementService : IJudgementService
    {
        private readonly IHighProfitJudgementMapper _highProfitJudgementMapper;

        private readonly IJudgementRepository _judgementRepository;

        private readonly ILogger<JudgementService> _logger;

        private readonly IRuleViolationService _ruleViolationService;

        public JudgementService(
            IJudgementRepository judgementRepository,
            IRuleViolationService ruleViolationService,
            IHighProfitJudgementMapper highProfitJudgementMapper,
            ILogger<JudgementService> logger)
        {
            this._judgementRepository =
                judgementRepository ?? throw new ArgumentNullException(nameof(judgementRepository));
            this._highProfitJudgementMapper = highProfitJudgementMapper
                                              ?? throw new ArgumentNullException(nameof(highProfitJudgementMapper));
            this._ruleViolationService =
                ruleViolationService ?? throw new ArgumentNullException(nameof(ruleViolationService));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Judgement(IHighProfitJudgementContext judgementContext)
        {
            if (judgementContext == null)
            {
                this._logger?.LogError("High Profit Judgement was null");
                return;
            }

            if (string.IsNullOrWhiteSpace(judgementContext?.Judgement?.OrderId))
            {
                this._logger?.LogInformation(
                    "High Profit Judgement had no order id - this is normal for market close analysis");
                return;
            }

            this._judgementRepository.Save(judgementContext.Judgement);

            if (!judgementContext.ProjectToAlert)
                return;

            var projectedBreach = this._highProfitJudgementMapper.Map(judgementContext);
            this._ruleViolationService.AddRuleViolation(projectedBreach);
        }

        public void Judgement(IFixedIncomeHighProfitJudgementContext judgementContext)
        {
            throw new NotImplementedException();
        }

        public void Judgement(ICancelledOrderJudgement cancelledOrder)
        {
            if (cancelledOrder == null)
            {
                this._logger?.LogError("Cancelled Order Judgement was null");
                return;
            }

            this._judgementRepository.Save(cancelledOrder);

            // judgement is also a rule breach
            var projectedBreach = (ICancelledOrderRuleBreach)null;
            this._ruleViolationService.AddRuleViolation(projectedBreach);
        }

        public void Judgement(IHighVolumeJudgement highVolume)
        {
            if (highVolume == null)
            {
                this._logger?.LogError("High Volume Judgement was null");
                return;
            }

            this._judgementRepository.Save(highVolume);

            // judgement is also a rule breach
            var projectedBreach = (IHighVolumeRuleBreach)null;
            this._ruleViolationService.AddRuleViolation(projectedBreach);
        }

        public void Judgement(ILayeringJudgement layering)
        {
            if (layering == null)
            {
                this._logger?.LogError("Layering Judgement was null");
                return;
            }

            this._judgementRepository.Save(layering);

            // judgement is also a rule breach
            var projectedBreach = (ILayeringRuleBreach)null;
            this._ruleViolationService.AddRuleViolation(projectedBreach);
        }

        public void Judgement(IMarkingTheCloseJudgement markingTheClose)
        {
            if (markingTheClose == null)
            {
                this._logger?.LogError("Marking The Close Judgement was null");
                return;
            }

            this._judgementRepository.Save(markingTheClose);

            // judgement is also a rule breach
            var projectedBreach = (IMarkingTheCloseBreach)null;
            this._ruleViolationService.AddRuleViolation(projectedBreach);
        }

        public void Judgement(IPlacingOrdersWithNoIntentToExecuteJudgement placingOrders)
        {
            if (placingOrders == null)
            {
                this._logger?.LogError("Placing Orders Judgement was null");
                return;
            }

            this._judgementRepository.Save(placingOrders);

            // judgement is also a rule breach
            var projectedBreach = (IPlacingOrdersWithNoIntentToExecuteRuleBreach)null;
            this._ruleViolationService.AddRuleViolation(projectedBreach);
        }

        public void Judgement(IRampingJudgement ramping)
        {
            if (ramping == null)
            {
                this._logger?.LogError("Ramping Judgement was null");
                return;
            }

            this._judgementRepository.Save(ramping);

            // judgement is also a rule breach
            var projectedBreach = (IRampingRuleBreach)null;
            this._ruleViolationService.AddRuleViolation(projectedBreach);
        }

        public void Judgement(ISpoofingJudgement spoofing)
        {
            if (spoofing == null)
            {
                this._logger?.LogError("Spoofing Judgement was null");
                return;
            }

            this._judgementRepository.Save(spoofing);

            // judgement is also a rule breach
            var projectedBreach = (ISpoofingRuleBreach)null;
            this._ruleViolationService.AddRuleViolation(projectedBreach);
        }

        public void PassJudgement()
        {
            this._ruleViolationService.ProcessRuleViolationCache();
        }
    }
}