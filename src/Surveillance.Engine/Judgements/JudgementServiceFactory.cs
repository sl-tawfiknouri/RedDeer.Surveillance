using System;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Aurora.Judgements.Interfaces;
using Surveillance.Engine.Rules.Judgements.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Interfaces;

namespace Surveillance.Engine.Rules.Judgements
{
    public class JudgementServiceFactory : IJudgementServiceFactory
    {
        private readonly IRuleViolationServiceFactory _ruleViolationServiceFactory;
        private readonly IJudgementRepository _judgementRepository;
        private readonly IHighProfitJudgementMapper _highProfitJudgementMapper;

        private readonly ILogger<JudgementService> _logger;

        public JudgementServiceFactory(
            IRuleViolationServiceFactory ruleViolationServiceFactory,
            IJudgementRepository judgementRepository,
            IHighProfitJudgementMapper highProfitJudgementMapper,
            ILogger<JudgementService> logger)
        {
            _ruleViolationServiceFactory =
                ruleViolationServiceFactory
                ?? throw new ArgumentNullException(nameof(ruleViolationServiceFactory));

            _judgementRepository =
                judgementRepository
                ?? throw new ArgumentNullException(nameof(judgementRepository));

            _highProfitJudgementMapper =
                highProfitJudgementMapper
                ?? throw new ArgumentNullException(nameof(highProfitJudgementMapper));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IJudgementService Build()
        {
            return new JudgementService(
                _judgementRepository,
                _ruleViolationServiceFactory.Build(),
                _highProfitJudgementMapper,
                _logger);
        }
    }
}
