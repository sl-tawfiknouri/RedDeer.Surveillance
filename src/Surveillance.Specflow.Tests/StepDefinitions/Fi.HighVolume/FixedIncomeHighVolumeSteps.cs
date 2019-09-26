namespace Surveillance.Specflow.Tests.StepDefinitions.Fi.HighVolume
{
    using System;

    using Domain.Surveillance.Scheduling;

    using FakeItEasy;

    using Microsoft.Extensions.Logging.Abstractions;

    using RedDeer.Contracts.SurveillanceService.Api.Markets;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.DataLayer.Aurora.BMLL;
    using Surveillance.DataLayer.Aurora.Judgements.Interfaces;
    using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
    using Surveillance.Engine.Rules.Factories;
    using Surveillance.Engine.Rules.Judgements;
    using Surveillance.Engine.Rules.Judgements.Interfaces;
    using Surveillance.Engine.Rules.Markets;
    using Surveillance.Engine.Rules.Markets.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Filter;
    using Surveillance.Engine.Rules.RuleParameters.FixedIncome;
    using Surveillance.Engine.Rules.RuleParameters.OrganisationalFactors;
    using Surveillance.Engine.Rules.Rules;
    using Surveillance.Engine.Rules.Rules.Equity.HighProfits;
    using Surveillance.Engine.Rules.Rules.FixedIncome.HighVolume;
    using Surveillance.Engine.Rules.Rules.Interfaces;
    using Surveillance.Engine.Rules.Trades;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
    using Surveillance.Reddeer.ApiClient.MarketOpenClose.Interfaces;
    using Surveillance.Specflow.Tests.StepDefinitions.Universe;

    using TechTalk.SpecFlow;
    using TechTalk.SpecFlow.Assist;

    /// <summary>
    /// The fixed income high volume issuance steps.
    /// </summary>
    [Binding]
    public class FixedIncomeHighVolumeSteps
    {
        /// <summary>
        /// The scenario context.
        /// </summary>
        private readonly ScenarioContext scenarioContext;

        /// <summary>
        /// The universe selection state.
        /// </summary>
        private readonly UniverseSelectionState universeSelectionState;

        /// <summary>
        /// The interday universe market cache factory.
        /// </summary>
        private UniverseMarketCacheFactory interdayUniverseMarketCacheFactory;

        /// <summary>
        /// The order filter service.
        /// </summary>
        private IUniverseFixedIncomeOrderFilterService orderFilterService;

        /// <summary>
        /// The parameters.
        /// </summary>
        private HighVolumeIssuanceRuleFixedIncomeParameters parameters;

        /// <summary>
        /// The rule context.
        /// </summary>
        private ISystemProcessOperationRunRuleContext ruleContext;

        /// <summary>
        /// The judgement repository.
        /// </summary>
        private IJudgementRepository judgementRepository;

        /// <summary>
        /// The rule violation service.
        /// </summary>
        private IRuleViolationService ruleViolationService;

        /// <summary>
        /// The judgement service.
        /// </summary>
        private IFixedIncomeHighVolumeJudgementService judgementService;

        /// <summary>
        /// The market trading hours service.
        /// </summary>
        private IMarketTradingHoursService marketTradingHoursService;

        /// <summary>
        /// The data request subscriber.
        /// </summary>
        private IUniverseDataRequestsSubscriber dataRequestSubscriber;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedIncomeHighVolumeSteps"/> class.
        /// </summary>
        /// <param name="scenarioContext">
        /// The scenario context.
        /// </param>
        /// <param name="universeSelectionState">
        /// The universe selection state.
        /// </param>
        public FixedIncomeHighVolumeSteps(
            ScenarioContext scenarioContext,
            UniverseSelectionState universeSelectionState)
        {
            this.scenarioContext = scenarioContext;
            this.universeSelectionState = universeSelectionState;
        }

        /// <summary>
        /// The given i have the fixed income high volume issuance rule parameter values.
        /// </summary>
        /// <param name="ruleParameters">
        /// The rule parameters.
        /// </param>
        [Given(@"I have the fixed income high volume rule parameter values")]
        public void GivenIHaveTheFixedIncomeHighVolumeIssuanceRuleParameterValues(Table ruleParameters)
        {
            if (ruleParameters.RowCount != 1)
            {
                this.scenarioContext.Pending();
                return;
            }

            var highVolumeParameters = ruleParameters.CreateInstance<FixedIncomeHighVolumeApiParameters>();

            this.parameters = new HighVolumeIssuanceRuleFixedIncomeParameters(
                "0",
                TimeSpan.FromHours(highVolumeParameters.WindowHours),
                highVolumeParameters.FixedIncomeHighVolumePercentageDaily,
                highVolumeParameters.FixedIncomeHighVolumePercentageWindow,
                DecimalRangeRuleFilter.None(),
                DecimalRangeRuleFilter.None(), 
                RuleFilter.None(),
                RuleFilter.None(),
                RuleFilter.None(),
                RuleFilter.None(),
                RuleFilter.None(),
                RuleFilter.None(),
                RuleFilter.None(),
                RuleFilter.None(),
                RuleFilter.None(),
                new[] { ClientOrganisationalFactors.None },
                true,
                true);
        }

        /// <summary>
        /// The then i will have alerts.
        /// </summary>
        /// <param name="alertCount">
        /// The alert count.
        /// </param>
        [Then(@"I will have (.*) fixed income high volume alerts")]
        public void ThenIWillHaveAlerts(int alertCount)
        {
            A
                .CallTo(() => this.ruleViolationService.AddRuleViolation(A<IRuleBreach>.Ignored))
                .MustHaveHappenedANumberOfTimesMatching(_ => _ == alertCount);
        }

        /// <summary>
        /// The when i run the fixed income high profit rule.
        /// </summary>
        [When(@"I run the fixed income high volume rule")]
        public void WhenIRunTheFixedIncomeHighProfitRule()
        {
            var scheduledExecution = new ScheduledExecution { IsForceRerun = true };

            this.Setup();

            var rule = new FixedIncomeHighVolumeRule(
                this.parameters,
                this.orderFilterService,
                this.ruleContext,
                this.interdayUniverseMarketCacheFactory,
                this.judgementService,
                this.dataRequestSubscriber,
                this.marketTradingHoursService,
                RuleRunMode.ForceRun,
                new NullLogger<FixedIncomeHighVolumeRule>(),
                new NullLogger<TradingHistoryStack>());

            foreach (var universeEvent in this.universeSelectionState.SelectedUniverse.UniverseEvents)
            {
                rule.OnNext(universeEvent);
            }
        }

        /// <summary>
        /// The test setup.
        /// </summary>
        private void Setup()
        {
            this.orderFilterService = A.Fake<IUniverseFixedIncomeOrderFilterService>();
            this.ruleContext = A.Fake<ISystemProcessOperationRunRuleContext>();
            this.dataRequestSubscriber = A.Fake<IUniverseDataRequestsSubscriber>();
            this.marketTradingHoursService = A.Fake<IMarketTradingHoursService>();

            this.judgementRepository = A.Fake<IJudgementRepository>();
            this.ruleViolationService = A.Fake<IRuleViolationService>();

            this.judgementService = new JudgementService(
                this.judgementRepository,
                this.ruleViolationService,
                new HighProfitJudgementMapper(new NullLogger<HighProfitJudgementMapper>()),
                new FixedIncomeHighProfitJudgementMapper(new NullLogger<FixedIncomeHighProfitJudgementMapper>()),
                new FixedIncomeHighVolumeJudgementMapper(new NullLogger<FixedIncomeHighVolumeJudgementMapper>()),
                new NullLogger<JudgementService>());

            var repository = A.Fake<IMarketOpenCloseApiCachingDecorator>();

            A
                .CallTo(() => repository.GetAsync()).
                Returns(new ExchangeDto[]
                            {
                                new ExchangeDto
                                    {
                                        Code = "XLON",
                                        MarketOpenTime = TimeSpan.FromHours(8),
                                        MarketCloseTime = TimeSpan.FromHours(16),
                                        IsOpenOnMonday = true,
                                        IsOpenOnTuesday = true,
                                        IsOpenOnWednesday = true,
                                        IsOpenOnThursday = true,
                                        IsOpenOnFriday = true,
                                        IsOpenOnSaturday = true,
                                        IsOpenOnSunday = true,
                                    },

                                new ExchangeDto
                                    {
                                        Code = "Diversity",
                                        MarketOpenTime = TimeSpan.FromHours(8),
                                        MarketCloseTime = TimeSpan.FromHours(16),
                                        IsOpenOnMonday = true,
                                        IsOpenOnTuesday = true,
                                        IsOpenOnWednesday = true,
                                        IsOpenOnThursday = true,
                                        IsOpenOnFriday = true,
                                        IsOpenOnSaturday = true,
                                        IsOpenOnSunday = true,
                                    },

                                new ExchangeDto
                                    {
                                        Code = "NASDAQ",
                                        MarketOpenTime = TimeSpan.FromHours(15),
                                        MarketCloseTime = TimeSpan.FromHours(23),
                                        IsOpenOnMonday = true,
                                        IsOpenOnTuesday = true,
                                        IsOpenOnWednesday = true,
                                        IsOpenOnThursday = true,
                                        IsOpenOnFriday = true,
                                        IsOpenOnSaturday = true,
                                        IsOpenOnSunday = true,
                                    }
                            });

            this.marketTradingHoursService = new MarketTradingHoursService(repository, new NullLogger<MarketTradingHoursService>());
            
            this.interdayUniverseMarketCacheFactory = new UniverseMarketCacheFactory(
                new StubRuleRunDataRequestRepository(),
                new StubRuleRunDataRequestRepository(),
                new NullLogger<UniverseMarketCacheFactory>());
        }
    }
}