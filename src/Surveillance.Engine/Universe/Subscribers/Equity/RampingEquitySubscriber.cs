namespace Surveillance.Engine.Rules.Universe.Subscribers.Equity
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Domain.Core.Extensions;
    using Domain.Surveillance.Scheduling;

    using Microsoft.Extensions.Logging;

    using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
    using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
    using Surveillance.Engine.Rules.Factories.Equities;
    using Surveillance.Engine.Rules.Factories.Equities.Interfaces;
    using Surveillance.Engine.Rules.Judgements.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Interfaces;
    using Surveillance.Engine.Rules.Rules;
    using Surveillance.Engine.Rules.Rules.Interfaces;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
    using Surveillance.Engine.Rules.Universe.Interfaces;
    using Surveillance.Engine.Rules.Universe.OrganisationalFactors.Interfaces;
    using Surveillance.Engine.Rules.Universe.Subscribers.Equity.Interfaces;

    public class RampingEquitySubscriber : BaseSubscriber, IRampingEquitySubscriber
    {
        private readonly IOrganisationalFactorBrokerServiceFactory _brokerServiceFactory;

        private readonly IHighVolumeVenueDecoratorFilterFactory _decoratorFilterFactory;

        private readonly IEquityRuleRampingFactory _equityRuleRampingFactory;

        private readonly ILogger<RampingEquitySubscriber> _logger;

        private readonly IRuleParameterToRulesMapperDecorator _ruleParameterMapper;

        private readonly IUniverseFilterFactory _universeFilterFactory;

        public RampingEquitySubscriber(
            IRuleParameterToRulesMapperDecorator ruleParameterMapper,
            IUniverseFilterFactory universeFilterFactory,
            IOrganisationalFactorBrokerServiceFactory brokerServiceFactory,
            IEquityRuleRampingFactory equityRuleRampingFactory,
            IHighVolumeVenueDecoratorFilterFactory decoratorFilterFactory,
            ILogger<RampingEquitySubscriber> logger)
        {
            this._ruleParameterMapper =
                ruleParameterMapper ?? throw new ArgumentNullException(nameof(ruleParameterMapper));
            this._universeFilterFactory =
                universeFilterFactory ?? throw new ArgumentNullException(nameof(universeFilterFactory));
            this._brokerServiceFactory =
                brokerServiceFactory ?? throw new ArgumentNullException(nameof(brokerServiceFactory));
            this._equityRuleRampingFactory = equityRuleRampingFactory
                                             ?? throw new ArgumentNullException(nameof(equityRuleRampingFactory));
            this._decoratorFilterFactory =
                decoratorFilterFactory ?? throw new ArgumentNullException(nameof(decoratorFilterFactory));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IReadOnlyCollection<IObserver<IUniverseEvent>> CollateSubscriptions(
            ScheduledExecution execution,
            RuleParameterDto ruleParameters,
            ISystemProcessOperationContext operationContext,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            IJudgementService judgementService,
            IUniverseAlertStream alertStream)
        {
            if (!execution.Rules?.Select(_ => _.Rule)?.Contains(Rules.Ramping) ?? true)
                return new IObserver<IUniverseEvent>[0];

            var filteredParameters = execution.Rules.SelectMany(_ => _.Ids).Where(_ => _ != null).ToList();
            var dtos = ruleParameters.Rampings
                .Where(_ => filteredParameters.Contains(_.Id, StringComparer.OrdinalIgnoreCase)).ToList();

            var rampingParameters = this._ruleParameterMapper.Map(execution, dtos);

            return this.SubscribeToUniverse(execution, operationContext, alertStream, rampingParameters, dataRequestSubscriber);
        }

        private IUniverseRule DecorateWithFilters(
            ISystemProcessOperationContext opCtx,
            IRampingRuleEquitiesParameters param,
            IUniverseRule rampingRule,
            IUniverseDataRequestsSubscriber universeDataRequestsSubscriber,
            ISystemProcessOperationRunRuleContext processOperationRunRuleContext,
            RuleRunMode ruleRunMode)
        {
            if (param.HasInternalFilters() || param.HasReferenceDataFilters() || param.HasMarketCapFilters()
                || param.HasVenueVolumeFilters())
            {
                this._logger.LogInformation($"parameters had filters. Inserting filtered universe in {opCtx.Id} OpCtx");
                var filteredUniverse = this._universeFilterFactory.Build(
                    param.Accounts,
                    param.Traders,
                    param.Markets,
                    param.Funds,
                    param.Strategies,
                    param.Sectors,
                    param.Industries,
                    param.Regions,
                    param.Countries,
                    param.MarketCapFilter,
                    ruleRunMode,
                    "Ramping Equity",
                    universeDataRequestsSubscriber,
                    processOperationRunRuleContext);

                var decoratedFilter = filteredUniverse;

                if (param.HasVenueVolumeFilters())
                    decoratedFilter = this._decoratorFilterFactory.Build(
                        param.Windows,
                        filteredUniverse,
                        param.VenueVolumeFilter,
                        processOperationRunRuleContext,
                        universeDataRequestsSubscriber,
                        this.DataSourceForWindow(param.Windows),
                        ruleRunMode);

                decoratedFilter.Subscribe(rampingRule);

                return decoratedFilter;
            }

            return rampingRule;
        }

        private IUniverseRule SubscribeParamToUniverse(
            ScheduledExecution execution,
            ISystemProcessOperationContext opCtx,
            IUniverseAlertStream alertStream,
            IRampingRuleEquitiesParameters param,
            IUniverseDataRequestsSubscriber dataRequestSubscriber)
        {
            var ruleCtx = opCtx.CreateAndStartRuleRunContext(
                Rules.Ramping.GetDescription(),
                EquityRuleRampingFactory.Version,
                param.Id,
                (int)Rules.Ramping,
                execution.IsBackTest,
                execution.TimeSeriesInitiation.DateTime,
                execution.TimeSeriesTermination.DateTime,
                execution.CorrelationId,
                execution.IsForceRerun);

            var runMode = execution.IsForceRerun ? RuleRunMode.ForceRun : RuleRunMode.ValidationRun;
            var rampingRule = this._equityRuleRampingFactory.Build(
                param,
                ruleCtx,
                alertStream,
                runMode,
                dataRequestSubscriber);
            var rampingRuleOrgFactors = this._brokerServiceFactory.Build(
                rampingRule,
                param.Factors,
                param.AggregateNonFactorableIntoOwnCategory);
            var rampingRuleFiltered = this.DecorateWithFilters(
                opCtx,
                param,
                rampingRuleOrgFactors,
                dataRequestSubscriber,
                ruleCtx,
                runMode);

            return rampingRuleFiltered;
        }

        private IReadOnlyCollection<IObserver<IUniverseEvent>> SubscribeToUniverse(
            ScheduledExecution execution,
            ISystemProcessOperationContext opCtx,
            IUniverseAlertStream alertStream,
            IReadOnlyCollection<IRampingRuleEquitiesParameters> rampingParameters,
            IUniverseDataRequestsSubscriber dataRequestSubscriber)
        {
            var subscriptions = new List<IObserver<IUniverseEvent>>();

            if (rampingParameters != null && rampingParameters.Any())
            {
                foreach (var param in rampingParameters)
                {
                    var baseSubscriber = this.SubscribeParamToUniverse(
                        execution,
                        opCtx,
                        alertStream,
                        param,
                        dataRequestSubscriber);
                    subscriptions.Add(baseSubscriber);
                }
            }
            else
            {
                const string errorMessage = "tried to schedule a cancelled order rule execution with no parameters set";
                this._logger.LogError(errorMessage);
                opCtx.EventError(errorMessage);
            }

            return subscriptions;
        }
    }
}