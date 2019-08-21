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
    using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Interfaces;
    using Surveillance.Engine.Rules.Rules;
    using Surveillance.Engine.Rules.Rules.Interfaces;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
    using Surveillance.Engine.Rules.Universe.Interfaces;
    using Surveillance.Engine.Rules.Universe.OrganisationalFactors.Interfaces;
    using Surveillance.Engine.Rules.Universe.Subscribers.Equity.Interfaces;

    public class LayeringEquitySubscriber : BaseSubscriber, ILayeringEquitySubscriber
    {
        private readonly IOrganisationalFactorBrokerServiceFactory _brokerServiceFactory;

        private readonly IHighVolumeVenueDecoratorFilterFactory _decoratorFilterFactory;

        private readonly IEquityRuleLayeringFactory _equityRuleLayeringFactory;

        private readonly ILogger<LayeringEquitySubscriber> _logger;

        private readonly IRuleParameterToRulesMapperDecorator _ruleParameterMapper;

        private readonly IUniverseFilterFactory _universeFilterFactory;

        public LayeringEquitySubscriber(
            IEquityRuleLayeringFactory equityRuleLayeringFactory,
            IRuleParameterToRulesMapperDecorator ruleParameterMapper,
            IUniverseFilterFactory universeFilterFactory,
            IOrganisationalFactorBrokerServiceFactory brokerServiceFactory,
            IHighVolumeVenueDecoratorFilterFactory decoratorFilterFactory,
            ILogger<LayeringEquitySubscriber> logger)
        {
            this._equityRuleLayeringFactory = equityRuleLayeringFactory
                                              ?? throw new ArgumentNullException(nameof(equityRuleLayeringFactory));
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
            if (!execution.Rules?.Select(ru => ru.Rule)?.Contains(Rules.Layering) ?? true)
                return new IObserver<IUniverseEvent>[0];

            var filteredParameters = execution.Rules.SelectMany(ru => ru.Ids).Where(ru => ru != null).ToList();
            var dtos = ruleParameters.Layerings
                .Where(la => filteredParameters.Contains(la.Id, StringComparer.InvariantCultureIgnoreCase)).ToList();

            var layeringParameters = this._ruleParameterMapper.Map(execution, dtos);
            var subscriptions = this.SubscribeToUniverse(
                execution,
                opCtx,
                alertStream,
                dataRequestSubscriber,
                layeringParameters);

            return subscriptions;
        }

        private IUniverseRule DecorateWithFilter(
            ISystemProcessOperationContext opCtx,
            ILayeringRuleEquitiesParameters param,
            IUniverseRule layering,
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
                    "Layering Equity",
                    universeDataRequestsSubscriber,
                    processOperationRunRuleContext);

                var decoratedFilters = filteredUniverse;

                if (param.HasVenueVolumeFilters())
                    decoratedFilters = this._decoratorFilterFactory.Build(
                        param.Windows,
                        filteredUniverse,
                        param.VenueVolumeFilter,
                        processOperationRunRuleContext,
                        universeDataRequestsSubscriber,
                        this.LayeringDataSource(param),
                        ruleRunMode);

                decoratedFilters.Subscribe(layering);

                return decoratedFilters;
            }

            return layering;
        }

        private DataSource LayeringDataSource(ILayeringRuleEquitiesParameters parameters)
        {
            if (parameters == null) return DataSource.AllInterday;

            if (parameters.PercentageOfMarketWindowVolume != null) return DataSource.AllIntraday;

            if (parameters.PercentageOfMarketDailyVolume != null) return DataSource.AllInterday;

            return this.DataSourceForWindow(parameters.Windows);
        }

        private IUniverseRule SubscribeToParameters(
            ScheduledExecution execution,
            ISystemProcessOperationContext opCtx,
            IUniverseAlertStream alertStream,
            IUniverseDataRequestsSubscriber universeDataRequestsSubscriber,
            ILayeringRuleEquitiesParameters param)
        {
            var ruleCtx = opCtx.CreateAndStartRuleRunContext(
                Rules.Layering.GetDescription(),
                EquityRuleLayeringFactory.Version,
                param.Id,
                (int)Rules.Layering,
                execution.IsBackTest,
                execution.TimeSeriesInitiation.DateTime,
                execution.TimeSeriesTermination.DateTime,
                execution.CorrelationId,
                execution.IsForceRerun);

            var runMode = execution.IsForceRerun ? RuleRunMode.ForceRun : RuleRunMode.ValidationRun;
            var layeringRule = this._equityRuleLayeringFactory.Build(param, ruleCtx, alertStream, runMode);

            var layeringRuleOrgFactors = this._brokerServiceFactory.Build(
                layeringRule,
                param.Factors,
                param.AggregateNonFactorableIntoOwnCategory);

            var layeringRuleFiltered = this.DecorateWithFilter(
                opCtx,
                param,
                layeringRuleOrgFactors,
                universeDataRequestsSubscriber,
                ruleCtx,
                runMode);

            return layeringRuleFiltered;
        }

        private IReadOnlyCollection<IObserver<IUniverseEvent>> SubscribeToUniverse(
            ScheduledExecution execution,
            ISystemProcessOperationContext opCtx,
            IUniverseAlertStream alertStream,
            IUniverseDataRequestsSubscriber universeDataRequestsSubscriber,
            IReadOnlyCollection<ILayeringRuleEquitiesParameters> layeringParameters)
        {
            var subscriptions = new List<IObserver<IUniverseEvent>>();

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (layeringParameters != null && layeringParameters.Any())
            {
                foreach (var param in layeringParameters)
                {
                    var paramSubscriptions = this.SubscribeToParameters(
                        execution,
                        opCtx,
                        alertStream,
                        universeDataRequestsSubscriber,
                        param);
                    subscriptions.Add(paramSubscriptions);
                }
            }
            else
            {
                const string errorMessage = "tried to schedule a layering rule execution with no parameters set";
                this._logger.LogError(errorMessage);
                opCtx.EventError(errorMessage);
            }

            return subscriptions;
        }
    }
}