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

    public class SpoofingEquitySubscriber : BaseSubscriber, ISpoofingEquitySubscriber
    {
        private readonly IOrganisationalFactorBrokerServiceFactory _brokerServiceFactory;

        private readonly IHighVolumeVenueDecoratorFilterFactory _decoratorFilterFactory;

        private readonly IEquityRuleSpoofingFactory _equityRuleSpoofingFactory;

        private readonly ILogger _logger;

        private readonly IRuleParameterToRulesMapperDecorator _ruleParameterMapper;

        private readonly IUniverseFilterFactory _universeFilterFactory;

        public SpoofingEquitySubscriber(
            IEquityRuleSpoofingFactory equityRuleSpoofingFactory,
            IRuleParameterToRulesMapperDecorator ruleParameterMapper,
            IUniverseFilterFactory universeFilterFactory,
            IOrganisationalFactorBrokerServiceFactory brokerServiceFactory,
            IHighVolumeVenueDecoratorFilterFactory decoratorFilterFactory,
            ILogger<UniverseRuleSubscriber> logger)
        {
            this._equityRuleSpoofingFactory = equityRuleSpoofingFactory
                                              ?? throw new ArgumentNullException(nameof(equityRuleSpoofingFactory));
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
            ISystemProcessOperationContext operationContext,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            IJudgementService judgementService,
            IUniverseAlertStream alertStream)
        {
            if (!execution.Rules?.Select(ab => ab.Rule)?.ToList().Contains(Rules.Spoofing) ?? true)
                return new IObserver<IUniverseEvent>[0];

            var filteredParameters = execution.Rules.SelectMany(ru => ru.Ids).Where(ru => ru != null).ToList();

            var dtos = ruleParameters.Spoofings
                .Where(sp => filteredParameters.Contains(sp.Id, StringComparer.InvariantCultureIgnoreCase)).ToList();

            var spoofingParameters = this._ruleParameterMapper.Map(execution, dtos);
            var subscriptionRequests = this.SubscribeToUniverse(
                execution,
                operationContext,
                alertStream,
                dataRequestSubscriber,
                spoofingParameters);

            return subscriptionRequests;
        }

        private IUniverseRule DecorateWithFilters(
            ISystemProcessOperationContext opCtx,
            ISpoofingRuleEquitiesParameters param,
            IUniverseRule spoofingRule,
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
                    "Spoofing Equity",
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

                decoratedFilter.Subscribe(spoofingRule);

                return decoratedFilter;
            }

            return spoofingRule;
        }

        private IUniverseRule SubscribeForParams(
            ScheduledExecution execution,
            ISystemProcessOperationContext opCtx,
            IUniverseAlertStream alertStream,
            IUniverseDataRequestsSubscriber universeDataRequestsSubscriber,
            ISpoofingRuleEquitiesParameters param)
        {
            var ruleCtx = opCtx.CreateAndStartRuleRunContext(
                Rules.Spoofing.GetDescription(),
                EquityRuleSpoofingFactory.Version,
                param.Id,
                (int)Rules.Spoofing,
                execution.IsBackTest,
                execution.TimeSeriesInitiation.DateTime,
                execution.TimeSeriesTermination.DateTime,
                execution.CorrelationId,
                execution.IsForceRerun);

            var runMode = execution.IsForceRerun ? RuleRunMode.ForceRun : RuleRunMode.ValidationRun;
            var spoofingRule = this._equityRuleSpoofingFactory.Build(param, ruleCtx, alertStream, runMode);
            var spoofingRuleOrgFactors = this._brokerServiceFactory.Build(
                spoofingRule,
                param.Factors,
                param.AggregateNonFactorableIntoOwnCategory);

            var filteredSpoofingRule = this.DecorateWithFilters(
                opCtx,
                param,
                spoofingRuleOrgFactors,
                universeDataRequestsSubscriber,
                ruleCtx,
                runMode);

            return filteredSpoofingRule;
        }

        private IReadOnlyCollection<IObserver<IUniverseEvent>> SubscribeToUniverse(
            ScheduledExecution execution,
            ISystemProcessOperationContext opCtx,
            IUniverseAlertStream alertStream,
            IUniverseDataRequestsSubscriber universeDataRequestsSubscriber,
            IReadOnlyCollection<ISpoofingRuleEquitiesParameters> spoofingParameters)
        {
            var subscriptions = new List<IObserver<IUniverseEvent>>();

            if (spoofingParameters != null && spoofingParameters.Any())
            {
                foreach (var param in spoofingParameters)
                {
                    var paramSubscriptions = this.SubscribeForParams(
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
                const string errorMessage = "tried to schedule a spoofing rule execution with no parameters set";
                this._logger.LogError(errorMessage);
                opCtx.EventError(errorMessage);
            }

            return subscriptions;
        }
    }
}