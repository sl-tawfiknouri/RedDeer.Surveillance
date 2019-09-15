namespace Surveillance.Engine.Rules.Judgements
{
    using System;

    using Microsoft.Extensions.Logging;

    using Surveillance.DataLayer.Aurora.Judgements.Interfaces;
    using Surveillance.Engine.Rules.Judgements.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Interfaces;

    /// <summary>
    /// The judgement service factory.
    /// </summary>
    public class JudgementServiceFactory : IJudgementServiceFactory
    {
        /// <summary>
        /// The high profit judgement mapper.
        /// </summary>
        private readonly IHighProfitJudgementMapper highProfitJudgementMapper;

        /// <summary>
        /// The fixed income high profit judgement mapper.
        /// </summary>
        private readonly IFixedIncomeHighProfitJudgementMapper fixedIncomeHighProfitJudgementMapper;

        /// <summary>
        /// The judgement repository.
        /// </summary>
        private readonly IJudgementRepository judgementRepository;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<JudgementService> logger;

        /// <summary>
        /// The rule violation service factory.
        /// </summary>
        private readonly IRuleViolationServiceFactory ruleViolationServiceFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="JudgementServiceFactory"/> class.
        /// </summary>
        /// <param name="ruleViolationServiceFactory">
        /// The rule violation service factory.
        /// </param>
        /// <param name="judgementRepository">
        /// The judgement repository.
        /// </param>
        /// <param name="highProfitJudgementMapper">
        /// The high profit judgement mapper.
        /// </param>
        /// <param name="fixedIncomeHighProfitJudgementMapper">
        /// The fixed income high profit judgement mapper.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public JudgementServiceFactory(
            IRuleViolationServiceFactory ruleViolationServiceFactory,
            IJudgementRepository judgementRepository,
            IHighProfitJudgementMapper highProfitJudgementMapper,
            IFixedIncomeHighProfitJudgementMapper fixedIncomeHighProfitJudgementMapper,
            ILogger<JudgementService> logger)
        {
            this.ruleViolationServiceFactory =
                ruleViolationServiceFactory ?? throw new ArgumentNullException(nameof(ruleViolationServiceFactory));

            this.judgementRepository =
                judgementRepository ?? throw new ArgumentNullException(nameof(judgementRepository));

            this.highProfitJudgementMapper =
                highProfitJudgementMapper ?? throw new ArgumentNullException(nameof(highProfitJudgementMapper));

            this.fixedIncomeHighProfitJudgementMapper =
                fixedIncomeHighProfitJudgementMapper ?? throw new ArgumentNullException(nameof(fixedIncomeHighProfitJudgementMapper));

            this.logger =
                logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// The build judgement service - injects in dependencies.
        /// </summary>
        /// <returns>
        /// The <see cref="IJudgementService"/>.
        /// </returns>
        public IJudgementService Build()
        {
            return new JudgementService(
                this.judgementRepository,
                this.ruleViolationServiceFactory.Build(),
                this.highProfitJudgementMapper,
                this.fixedIncomeHighProfitJudgementMapper,
                this.logger);
        }
    }
}