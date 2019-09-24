namespace Surveillance.Engine.Rules.Universe.Subscribers.Equity
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Domain.Surveillance.Scheduling;

    using Infrastructure.Network.Extensions;

    using Microsoft.Extensions.Logging;

    using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Data.Universe.Interfaces;
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
    using Surveillance.Engine.Rules.Universe.OrganisationalFactors.Interfaces;
    using Surveillance.Engine.Rules.Universe.Subscribers.Equity.Interfaces;

    public class CancelledOrderEquitySubscriber : BaseSubscriber, ICancelledOrderEquitySubscriber
    {
        private readonly IOrganisationalFactorBrokerServiceFactory _brokerServiceFactory;

        private readonly IHighVolumeVenueDecoratorFilterFactory _decoratorFilterFactory;

        private readonly IEquityRuleCancelledOrderFactory _equityRuleCancelledOrderFactory;

        private readonly ILogger<CancelledOrderEquitySubscriber> _logger;

        private readonly IRuleParameterToRulesMapperDecorator _ruleParameterMapper;

        private readonly IUniverseFilterFactory _universeFilterFactory;

        public CancelledOrderEquitySubscriber(
            IEquityRuleCancelledOrderFactory equityRuleCancelledOrderFactory,
            IRuleParameterToRulesMapperDecorator ruleParameterMapper,
            IUniverseFilterFactory universeFilterFactory,
            IOrganisationalFactorBrokerServiceFactory brokerServiceFactory,
            IHighVolumeVenueDecoratorFilterFactory decoratorFilterFactory,
            ILogger<CancelledOrderEquitySubscriber> logger)
        {
            this._equityRuleCancelledOrderFactory = equityRuleCancelledOrderFactory;
            this._ruleParameterMapper = ruleParameterMapper;
            this._universeFilterFactory = universeFilterFactory;
            this._brokerServiceFactory = brokerServiceFactory;
            this._decoratorFilterFactory = decoratorFilterFactory;
            this._logger = logger;
        }

        public IReadOnlyCollection<IObserver<IUniverseEvent>> CollateSubscriptions(
            ScheduledExecution execution,
            RuleParameterDto ruleParameters,
            ISystemProcessOperationContext operationContext,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            IJudgementService judgementService,
            IUniverseAlertStream alertStream)
        {
            if (!execution.Rules?.Select(ab => ab.Rule)?.Contains(Rules.CancelledOrders) ?? true)
                return new IObserver<IUniverseEvent>[0];

            var filteredParameters = execution.Rules.SelectMany(ru => ru.Ids).Where(ru => ru != null).ToList();
            var dtos = ruleParameters.CancelledOrders.Where(
                co => filteredParameters.Contains(co.Id, StringComparer.InvariantCultureIgnoreCase)).ToList();

            var cancelledOrderParameters = this._ruleParameterMapper.Map(execution, dtos);

            return this.SubscribeToUniverse(
                execution,
                operationContext,
                alertStream,
                dataRequestSubscriber,
                cancelledOrderParameters);
        }

        private IUniverseRule DecorateWithFilter(
            ISystemProcessOperationContext opCtx,
            ICancelledOrderRuleEquitiesParameters param,
            IUniverseRule cancelledOrderRule,
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
                    "Cancelled Order Equity",
                    universeDataRequestsSubscriber,
                    processOperationRunRuleContext);

                var decoratedFilteredUniverse = filteredUniverse;

                if (param.HasVenueVolumeFilters())
                    decoratedFilteredUniverse = this._decoratorFilterFactory.Build(
                        param.Windows,
                        filteredUniverse,
                        param.VenueVolumeFilter,
                        processOperationRunRuleContext,
                        universeDataRequestsSubscriber,
                        this.DataSourceForWindow(param.Windows),
                        ruleRunMode);

                decoratedFilteredUniverse.Subscribe(cancelledOrderRule);

                return decoratedFilteredUniverse;
            }

            return cancelledOrderRule;
        }

        private IUniverseRule SubscribeParamToUniverse(
            ScheduledExecution execution,
            ISystemProcessOperationContext opCtx,
            IUniverseAlertStream alertStream,
            IUniverseDataRequestsSubscriber universeDataRequestsSubscriber,
            ICancelledOrderRuleEquitiesParameters param)
        {
            var ruleCtx = opCtx.CreateAndStartRuleRunContext(
                Rules.CancelledOrders.GetDescription(),
                EquityRuleCancelledOrderFactory.Version,
                param.Id,
                (int)Rules.CancelledOrders,
                execution.IsBackTest,
                execution.TimeSeriesInitiation.DateTime,
                execution.TimeSeriesTermination.DateTime,
                execution.CorrelationId,
                execution.IsForceRerun);

            var runMode = execution.IsForceRerun ? RuleRunMode.ForceRun : RuleRunMode.ValidationRun;
            var cancelledOrderRule = this._equityRuleCancelledOrderFactory.Build(param, ruleCtx, alertStream, runMode);
            var cancelledOrderOrgFactors = this._brokerServiceFactory.Build(
                cancelledOrderRule,
                param.Factors,
                param.AggregateNonFactorableIntoOwnCategory);
            var cancelledOrderFiltered = this.DecorateWithFilter(
                opCtx,
                param,
                cancelledOrderOrgFactors,
                universeDataRequestsSubscriber,
                ruleCtx,
                runMode);

            return cancelledOrderFiltered;
        }

        private IReadOnlyCollection<IObserver<IUniverseEvent>> SubscribeToUniverse(
            ScheduledExecution execution,
            ISystemProcessOperationContext opCtx,
            IUniverseAlertStream alertStream,
            IUniverseDataRequestsSubscriber universeDataRequestsSubscriber,
            IReadOnlyCollection<ICancelledOrderRuleEquitiesParameters> cancelledOrderParameters)
        {
            var subscriptions = new List<IObserver<IUniverseEvent>>();

            if (cancelledOrderParameters != null && cancelledOrderParameters.Any())
            {
                foreach (var param in cancelledOrderParameters)
                {
                    var baseSubscriber = this.SubscribeParamToUniverse(
                        execution,
                        opCtx,
                        alertStream,
                        universeDataRequestsSubscriber,
                        param);
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