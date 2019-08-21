namespace Surveillance.Engine.Rules.Judgements
{
    using System;

    using Microsoft.Extensions.Logging;

    using Surveillance.DataLayer.Aurora.Judgements.Interfaces;
    using Surveillance.Engine.Rules.Judgements.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Interfaces;

    public class JudgementServiceFactory : IJudgementServiceFactory
    {
        private readonly IHighProfitJudgementMapper _highProfitJudgementMapper;

        private readonly IJudgementRepository _judgementRepository;

        private readonly ILogger<JudgementService> _logger;

        private readonly IRuleViolationServiceFactory _ruleViolationServiceFactory;

        public JudgementServiceFactory(
            IRuleViolationServiceFactory ruleViolationServiceFactory,
            IJudgementRepository judgementRepository,
            IHighProfitJudgementMapper highProfitJudgementMapper,
            ILogger<JudgementService> logger)
        {
            this._ruleViolationServiceFactory = ruleViolationServiceFactory
                                                ?? throw new ArgumentNullException(nameof(ruleViolationServiceFactory));

            this._judgementRepository =
                judgementRepository ?? throw new ArgumentNullException(nameof(judgementRepository));

            this._highProfitJudgementMapper = highProfitJudgementMapper
                                              ?? throw new ArgumentNullException(nameof(highProfitJudgementMapper));

            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IJudgementService Build()
        {
            return new JudgementService(
                this._judgementRepository,
                this._ruleViolationServiceFactory.Build(),
                this._highProfitJudgementMapper,
                this._logger);
        }
    }
}