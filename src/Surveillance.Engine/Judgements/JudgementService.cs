namespace Surveillance.Engine.Rules.Judgements
{
    using System;
    using System.Threading.Tasks;

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
    using Surveillance.Engine.Rules.Rules.FixedIncome.HighVolume.Interfaces;

    /// <summary>
    /// The judgement service.
    /// Contains processing for rule analysis outcomes
    /// </summary>
    public class JudgementService : IJudgementService
    {
        /// <summary>
        /// The equity high profit judgement mapper.
        /// </summary>
        private readonly IHighProfitJudgementMapper equityHighProfitJudgementMapper;

        /// <summary>
        /// The fixed income high profit judgement mapper.
        /// </summary>
        private readonly IFixedIncomeHighProfitJudgementMapper fixedIncomeHighProfitJudgementMapper;

        /// <summary>
        /// The fixed income high volume judgement mapper.
        /// </summary>
        private readonly IFixedIncomeHighVolumeJudgementMapper fixedIncomeHighVolumeJudgementMapper;

        /// <summary>
        /// The judgement repository.
        /// </summary>
        private readonly IJudgementRepository judgementRepository;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<JudgementService> logger;

        /// <summary>
        /// The rule violation service.
        /// </summary>
        private readonly IRuleViolationService ruleViolationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="JudgementService"/> class.
        /// </summary>
        /// <param name="judgementRepository">
        /// The judgement repository.
        /// </param>
        /// <param name="ruleViolationService">
        /// The rule violation service.
        /// </param>
        /// <param name="equityHighProfitJudgementMapper">
        /// The high profit judgement mapper.
        /// </param>
        /// <param name="fixedIncomeHighProfitJudgementMapper">
        /// The fixed income high profit judgement mapper
        /// </param>
        /// <param name="fixedIncomeHighVolumeJudgementMapper">
        /// The fixed income high volume judgement mapper for secondary market and percentage issuance
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public JudgementService(
            IJudgementRepository judgementRepository,
            IRuleViolationService ruleViolationService,
            IHighProfitJudgementMapper equityHighProfitJudgementMapper,
            IFixedIncomeHighProfitJudgementMapper fixedIncomeHighProfitJudgementMapper,
            IFixedIncomeHighVolumeJudgementMapper fixedIncomeHighVolumeJudgementMapper,
            ILogger<JudgementService> logger)
        {
            this.judgementRepository =
                judgementRepository ?? throw new ArgumentNullException(nameof(judgementRepository));
            this.equityHighProfitJudgementMapper =
                equityHighProfitJudgementMapper ?? throw new ArgumentNullException(nameof(equityHighProfitJudgementMapper));
            this.fixedIncomeHighProfitJudgementMapper =
                fixedIncomeHighProfitJudgementMapper ?? throw new ArgumentNullException(nameof(fixedIncomeHighProfitJudgementMapper));
            this.fixedIncomeHighVolumeJudgementMapper = 
                fixedIncomeHighVolumeJudgementMapper ?? throw new ArgumentNullException(nameof(fixedIncomeHighVolumeJudgementMapper));
            this.ruleViolationService =
                ruleViolationService ?? throw new ArgumentNullException(nameof(ruleViolationService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// The judgement.
        /// </summary>
        /// <param name="judgementContext">
        /// The judgement context.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task Judgement(IHighProfitJudgementContext judgementContext)
        {
            if (judgementContext == null)
            {
                this.logger?.LogError("Equity High Profit Judgement was null");

                return;
            }

            if (string.IsNullOrWhiteSpace(judgementContext?.Judgement?.OrderId))
            {
                this.logger?.LogInformation(
                    "Equity High Profit Judgement had no order id - this is normal for market close analysis");
                return;
            }

            await this.judgementRepository.SaveAsync(judgementContext.Judgement);

            if (!judgementContext.RaiseRuleViolation)
            {
                return;
            }

            var projectedBreach = this.equityHighProfitJudgementMapper.Map(judgementContext);
            this.ruleViolationService.AddRuleViolation(projectedBreach);
        }

        /// <summary>
        /// The judgement.
        /// </summary>
        /// <param name="judgementContext">
        /// The judgement context.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task Judgement(IFixedIncomeHighProfitJudgementContext judgementContext)
        {
            if (judgementContext == null)
            {
                this.logger?.LogError("Fixed Income High Profit Judgement was null");
                return;
            }

            if (string.IsNullOrWhiteSpace(judgementContext?.Judgement?.OrderId))
            {
                this.logger?.LogInformation(
                    "Fixed Income High Profit Judgement had no order id - this is normal for market close analysis");
                return;
            }

            await this.judgementRepository.SaveAsync(judgementContext.Judgement);

            if (!judgementContext.RaiseRuleViolation)
            {
                return;
            }

            var projectedBreach = this.fixedIncomeHighProfitJudgementMapper.Map(judgementContext);
            this.ruleViolationService.AddRuleViolation(projectedBreach);
        }

        /// <summary>
        /// The judgement for high volume - auto save as alert.
        /// </summary>
        /// <param name="highVolume">
        /// The high volume.
        /// </param>
        public async Task Judgement(IHighVolumeJudgement highVolume)
        {
            if (highVolume == null)
            {
                this.logger?.LogError("High Volume Judgement was null");
                return;
            }

            await this.judgementRepository.SaveAsync(highVolume);

            // judgement is also a rule breach
            var projectedBreach = (IHighVolumeRuleBreach)null;
            this.ruleViolationService.AddRuleViolation(projectedBreach);
        }

        /// <summary>
        /// The judgement.
        /// </summary>
        /// <param name="highVolumeJudgementContext">
        /// The high volume judgement context.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task Judgement(IFixedIncomeHighVolumeJudgementContext highVolumeJudgementContext)
        {
            if (highVolumeJudgementContext == null)
            {
                this.logger?.LogError("Fixed Income High Volume Judgement was null");

                return;
            }

            if (string.IsNullOrWhiteSpace(highVolumeJudgementContext?.Judgement?.OrderId))
            {
                this.logger?.LogError("Fixed Income High Volume Judgement had no order id");

                return;
            }

            await this.judgementRepository.SaveAsync(highVolumeJudgementContext.Judgement);

            if (!highVolumeJudgementContext.RaiseRuleViolation)
            {
                return;
            }

            var projectedBreach = this.fixedIncomeHighVolumeJudgementMapper.MapContextToBreach(highVolumeJudgementContext);
            this.ruleViolationService.AddRuleViolation(projectedBreach);
        }

        /// <summary>
        /// The judgement for a cancelled order - auto save as alert.
        /// </summary>
        /// <param name="cancelledOrder">
        /// The cancelled order.
        /// </param>
        public async Task Judgement(ICancelledOrderJudgement cancelledOrder)
        {
            if (cancelledOrder == null)
            {
                this.logger?.LogError("Cancelled Order Judgement was null");
                return;
            }

            await this.judgementRepository.SaveAsync(cancelledOrder);

            // judgement is also a rule breach
            var projectedBreach = (ICancelledOrderRuleBreach)null;
            this.ruleViolationService.AddRuleViolation(projectedBreach);
        }

        /// <summary>
        /// The judgement for layering - auto save as alert.
        /// </summary>
        /// <param name="layering">
        /// The layering.
        /// </param>
        public async Task Judgement(ILayeringJudgement layering)
        {
            if (layering == null)
            {
                this.logger?.LogError("Layering Judgement was null");
                return;
            }

            await this.judgementRepository.SaveAsync(layering);

            // judgement is also a rule breach
            var projectedBreach = (ILayeringRuleBreach)null;
            this.ruleViolationService.AddRuleViolation(projectedBreach);
        }

        /// <summary>
        /// The judgement for marking the close - auto save as alert.
        /// </summary>
        /// <param name="markingTheClose">
        /// The marking the close.
        /// </param>
        public async Task Judgement(IMarkingTheCloseJudgement markingTheClose)
        {
            if (markingTheClose == null)
            {
                this.logger?.LogError("Marking The Close Judgement was null");
                return;
            }

            await this.judgementRepository.SaveAsync(markingTheClose);

            // judgement is also a rule breach
            var projectedBreach = (IMarkingTheCloseBreach)null;
            this.ruleViolationService.AddRuleViolation(projectedBreach);
        }

        /// <summary>
        /// The judgement for placing orders with no intent to execute - auto save as alert.
        /// </summary>
        /// <param name="placingOrders">
        /// The placing orders.
        /// </param>
        public async Task Judgement(IPlacingOrdersWithNoIntentToExecuteJudgement placingOrders)
        {
            if (placingOrders == null)
            {
                this.logger?.LogError("Placing Orders Judgement was null");
                return;
            }

            await this.judgementRepository.SaveAsync(placingOrders);

            // judgement is also a rule breach
            var projectedBreach = (IPlacingOrdersWithNoIntentToExecuteRuleBreach)null;
            this.ruleViolationService.AddRuleViolation(projectedBreach);
        }

        /// <summary>
        /// The judgement for ramping - auto save as alert.
        /// </summary>
        /// <param name="ramping">
        /// The ramping.
        /// </param>
        public async Task Judgement(IRampingJudgement ramping)
        {
            if (ramping == null)
            {
                this.logger?.LogError("Ramping Judgement was null");
                return;
            }

            await this.judgementRepository.SaveAsync(ramping);

            // judgement is also a rule breach
            var projectedBreach = (IRampingRuleBreach)null;
            this.ruleViolationService.AddRuleViolation(projectedBreach);
        }

        /// <summary>
        /// The judgement.
        /// </summary>
        /// <param name="spoofing">
        /// The spoofing.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task Judgement(ISpoofingJudgement spoofing)
        {
            if (spoofing == null)
            {
                this.logger?.LogError("Spoofing Judgement was null");
                return;
            }

            await this.judgementRepository.SaveAsync(spoofing);

            // judgement is also a rule breach
            var projectedBreach = (ISpoofingRuleBreach)null;
            this.ruleViolationService.AddRuleViolation(projectedBreach);
        }

        /// <summary>
        /// The pass judgement call to ensure the rule violations service runs against its cache of judgements.
        /// </summary>
        public void PassJudgement()
        {
            this.ruleViolationService.ProcessRuleViolationCache();
        }
    }
}