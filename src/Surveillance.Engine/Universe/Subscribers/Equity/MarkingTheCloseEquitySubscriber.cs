namespace Surveillance.Engine.Rules.Universe.Subscribers.Equity
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Domain.Surveillance.Scheduling;

    using Infrastructure.Network.Extensions;

    using Microsoft.Extensions.Logging;

    using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;

    using SharedKernel.Contracts.Markets;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
    using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
    using Surveillance.Engine.Rules.Factories.Equities;
    using Surveillance.Engine.Rules.Factories.Equities.Interfaces;
    using Surveillance.Engine.Rules.Judgements.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Interfaces;
    using Surveillance.Engine.Rules.Rules;
    using Surveillance.Engine.Rules.Rules.Equity.MarkingTheClose.Interfaces;
    using Surveillance.Engine.Rules.Rules.Interfaces;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
    using Surveillance.Engine.Rules.Universe.Interfaces;
    using Surveillance.Engine.Rules.Universe.OrganisationalFactors.Interfaces;
    using Surveillance.Engine.Rules.Universe.Subscribers.Equity.Interfaces;

    public class MarkingTheCloseEquitySubscriber : BaseSubscriber, IMarkingTheCloseEquitySubscriber
    {
        private readonly IOrganisationalFactorBrokerServiceFactory _brokerServiceFactory;

        private readonly IHighVolumeVenueDecoratorFilterFactory _decoratorFilterFactory;

        private readonly IEquityRuleMarkingTheCloseFactory _equityRuleMarkingTheCloseFactory;

        private readonly ILogger<MarkingTheCloseEquitySubscriber> _logger;

        private readonly IRuleParameterToRulesMapperDecorator _ruleParameterMapper;

        private readonly IUniverseFilterFactory _universeFilterFactory;

        public MarkingTheCloseEquitySubscriber(
            IEquityRuleMarkingTheCloseFactory equityRuleMarkingTheCloseFactory,
            IRuleParameterToRulesMapperDecorator ruleParameterMapper,
            IUniverseFilterFactory universeFilterFactory,
            IHighVolumeVenueDecoratorFilterFactory decoratorFilterFactory,
            IOrganisationalFactorBrokerServiceFactory brokerServiceFactory,
            ILogger<MarkingTheCloseEquitySubscriber> logger)
        {
            this._equityRuleMarkingTheCloseFactory = equityRuleMarkingTheCloseFactory
                                                     ?? throw new ArgumentNullException(
                                                         nameof(equityRuleMarkingTheCloseFactory));
            this._ruleParameterMapper =
                ruleParameterMapper ?? throw new ArgumentNullException(nameof(ruleParameterMapper));
            this._universeFilterFactory =
                universeFilterFactory ?? throw new ArgumentNullException(nameof(universeFilterFactory));
            this._brokerServiceFactory =
                brokerServiceFactory ?? throw new ArgumentNullException(nameof(brokerServiceFactory));
            this._decoratorFilterFactory =
                decoratorFilterFactory ?? throw new ArgumentNullException(nameof(decoratorFilterFactory));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IReadOnlyCollection<IObserver<IUniverseEvent>> CollateSubscriptions(
            ScheduledExecution execution,
            RuleParameterDto ruleParameters,
            ISystemProcessOperationContext opCtx,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            IJudgementService judgementService,
            IUniverseAlertStream alertStream)
        {
            if (!execution.Rules?.Select(ru => ru.Rule)?.Contains(Rules.MarkingTheClose) ?? true)
                return new IObserver<IUniverseEvent>[0];

            var filteredParameters = execution.Rules.SelectMany(ru => ru.Ids).Where(ru => ru != null).ToList();
            var dtos = ruleParameters.MarkingTheCloses.Where(
                mtc => filteredParameters.Contains(mtc.Id, StringComparer.InvariantCultureIgnoreCase)).ToList();

            var markingTheCloseParameters = this._ruleParameterMapper.Map(execution, dtos);
            var subscriptions = this.SubscribeToUniverse(
                execution,
                opCtx,
                alertStream,
                dataRequestSubscriber,
                markingTheCloseParameters);

            return subscriptions;
        }

        private IUniverseRule DecorateWithFilters(
            ISystemProcessOperationContext opCtx,
            IMarkingTheCloseEquitiesParameters param,
            IUniverseRule markingTheClose,
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
                    "Marking The Close Equity",
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
                        this.MarkingTheCloseDataSource(param),
                        ruleRunMode);

                decoratedFilter.Subscribe(markingTheClose);

                return decoratedFilter;
            }

            return markingTheClose;
        }

        private DataSource MarkingTheCloseDataSource(IMarkingTheCloseEquitiesParameters parameters)
        {
            if (parameters == null) return DataSource.AllInterday;

            if (parameters.PercentageThresholdWindowVolume != null) return DataSource.AllIntraday;

            if (parameters.PercentageThresholdDailyVolume != null) return DataSource.AllInterday;

            return this.DataSourceForWindow(parameters.Windows);
        }

        private IUniverseRule SubscribeToParams(
            ScheduledExecution execution,
            ISystemProcessOperationContext opCtx,
            IUniverseAlertStream alertStream,
            IMarkingTheCloseEquitiesParameters param,
            IUniverseDataRequestsSubscriber dataRequestSubscriber)
        {
            var ruleCtx = opCtx.CreateAndStartRuleRunContext(
                Rules.MarkingTheClose.GetDescription(),
                EquityRuleMarkingTheCloseFactory.Version,
                param.Id,
                (int)Rules.MarkingTheClose,
                execution.IsBackTest,
                execution.TimeSeriesInitiation.DateTime,
                execution.TimeSeriesTermination.DateTime,
                execution.CorrelationId,
                execution.IsForceRerun);

            var runMode = execution.IsForceRerun ? RuleRunMode.ForceRun : RuleRunMode.ValidationRun;
            var markingTheClose = this._equityRuleMarkingTheCloseFactory.Build(
                param,
                ruleCtx,
                alertStream,
                runMode,
                dataRequestSubscriber);
            var markingTheCloseOrgFactors = this._brokerServiceFactory.Build(
                markingTheClose,
                param.Factors,
                param.AggregateNonFactorableIntoOwnCategory);

            var filteredMarkingTheClose = this.DecorateWithFilters(
                opCtx,
                param,
                markingTheCloseOrgFactors,
                dataRequestSubscriber,
                ruleCtx,
                runMode);

            return filteredMarkingTheClose;
        }

        private IReadOnlyCollection<IObserver<IUniverseEvent>> SubscribeToUniverse(
            ScheduledExecution execution,
            ISystemProcessOperationContext opCtx,
            IUniverseAlertStream alertStream,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            IReadOnlyCollection<IMarkingTheCloseEquitiesParameters> markingTheCloseParameters)
        {
            var subscriptions = new List<IObserver<IUniverseEvent>>();

            if (markingTheCloseParameters != null && markingTheCloseParameters.Any())
            {
                foreach (var param in markingTheCloseParameters)
                {
                    var paramSubscriptions = this.SubscribeToParams(
                        execution,
                        opCtx,
                        alertStream,
                        param,
                        dataRequestSubscriber);
                    subscriptions.Add(paramSubscriptions);
                }
            }
            else
            {
                const string errorMessage =
                    "tried to schedule a marking the close rule execution with no parameters set";
                this._logger.LogError(errorMessage);
                opCtx.EventError(errorMessage);
            }

            return subscriptions;
        }
    }
}