﻿using System;
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

namespace Surveillance.Engine.Rules.Universe.Subscribers.Equity
{
    public class SpoofingEquitySubscriber : BaseSubscriber, ISpoofingEquitySubscriber
    {
        private readonly IEquityRuleSpoofingFactory _equityRuleSpoofingFactory;
        private readonly IRuleParameterToRulesMapperDecorator _ruleParameterMapper;
        private readonly IUniverseFilterFactory _universeFilterFactory;
        private readonly IOrganisationalFactorBrokerServiceFactory _brokerServiceFactory;
        private readonly IHighVolumeVenueDecoratorFilterFactory _decoratorFilterFactory;
        private readonly ILogger _logger;

        public SpoofingEquitySubscriber(
            IEquityRuleSpoofingFactory equityRuleSpoofingFactory,
            IRuleParameterToRulesMapperDecorator ruleParameterMapper,
            IUniverseFilterFactory universeFilterFactory,
            IOrganisationalFactorBrokerServiceFactory brokerServiceFactory,
            IHighVolumeVenueDecoratorFilterFactory decoratorFilterFactory,
            ILogger<UniverseRuleSubscriber> logger)
        {
            _equityRuleSpoofingFactory = equityRuleSpoofingFactory ?? throw new ArgumentNullException(nameof(equityRuleSpoofingFactory));
            _ruleParameterMapper = ruleParameterMapper ?? throw new ArgumentNullException(nameof(ruleParameterMapper));
            _universeFilterFactory = universeFilterFactory ?? throw new ArgumentNullException(nameof(universeFilterFactory));
            _brokerServiceFactory = brokerServiceFactory ?? throw new ArgumentNullException(nameof(brokerServiceFactory));
            _decoratorFilterFactory = decoratorFilterFactory ?? throw new ArgumentNullException(nameof(decoratorFilterFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public IReadOnlyCollection<IObserver<IUniverseEvent>> CollateSubscriptions(
            ScheduledExecution execution,
            RuleParameterDto ruleParameters,
            ISystemProcessOperationContext opCtx,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            IJudgementService judgementService,
            IUniverseAlertStream alertStream)
        {
            if (!execution.Rules?.Select(ab => ab.Rule)?.ToList().Contains(Domain.Surveillance.Scheduling.Rules.Spoofing)
                ?? true)
            {
                return new IObserver<IUniverseEvent>[0];
            }

            var filteredParameters =
                execution
                    .Rules
                    .SelectMany(ru => ru.Ids)
                    .Where(ru => ru != null)
                    .ToList();

            var dtos =
                ruleParameters
                    .Spoofings
                    .Where(sp => filteredParameters.Contains(sp.Id, StringComparer.InvariantCultureIgnoreCase))
                    .ToList();

            var spoofingParameters = _ruleParameterMapper.Map(execution, dtos);
            var subscriptionRequests = SubscribeToUniverse(execution, opCtx, alertStream, dataRequestSubscriber, spoofingParameters);

            return subscriptionRequests;
        }

        private IReadOnlyCollection<IObserver<IUniverseEvent>> SubscribeToUniverse(
            ScheduledExecution execution,
            ISystemProcessOperationContext opCtx,
            IUniverseAlertStream alertStream,
            IUniverseDataRequestsSubscriber universeDataRequestsSubscriber,
            IReadOnlyCollection<ISpoofingRuleEquitiesParameters> spoofingParameters)
        {
            var subscriptions = new List<IObserver<IUniverseEvent>>();

            if (spoofingParameters != null
                && spoofingParameters.Any())
            {
                foreach (var param in spoofingParameters)
                {
                    var paramSubscriptions = SubscribeForParams(execution, opCtx, alertStream, universeDataRequestsSubscriber, param);
                    subscriptions.Add(paramSubscriptions);
                }
            }
            else
            {
                const string errorMessage = "tried to schedule a spoofing rule execution with no parameters set";
                _logger.LogError(errorMessage);
                opCtx.EventError(errorMessage);
            }

            return subscriptions;
        }

        private IUniverseRule SubscribeForParams(
            ScheduledExecution execution,
            ISystemProcessOperationContext opCtx,
            IUniverseAlertStream alertStream,
            IUniverseDataRequestsSubscriber universeDataRequestsSubscriber,
            ISpoofingRuleEquitiesParameters param)
        {
            var ruleCtx = opCtx
                .CreateAndStartRuleRunContext(
                    Domain.Surveillance.Scheduling.Rules.Spoofing.GetDescription(),
                    EquityRuleSpoofingFactory.Version,
                    param.Id,
                    (int)Domain.Surveillance.Scheduling.Rules.Spoofing,
                    execution.IsBackTest,
                    execution.TimeSeriesInitiation.DateTime,
                    execution.TimeSeriesTermination.DateTime,
                    execution.CorrelationId,
                    execution.IsForceRerun);

            var runMode = execution.IsForceRerun ? RuleRunMode.ForceRun : RuleRunMode.ValidationRun;
            var spoofingRule = _equityRuleSpoofingFactory.Build(param, ruleCtx, alertStream, runMode);
            var spoofingRuleOrgFactors =
                _brokerServiceFactory.Build(
                    spoofingRule,
                    param.Factors,
                    param.AggregateNonFactorableIntoOwnCategory);

            var filteredSpoofingRule = DecorateWithFilters(opCtx, param, spoofingRuleOrgFactors, universeDataRequestsSubscriber, ruleCtx, runMode);

            return filteredSpoofingRule;
        }

        private IUniverseRule DecorateWithFilters(
            ISystemProcessOperationContext opCtx,
            ISpoofingRuleEquitiesParameters param,
            IUniverseRule spoofingRule,
            IUniverseDataRequestsSubscriber universeDataRequestsSubscriber,
            ISystemProcessOperationRunRuleContext processOperationRunRuleContext,
            RuleRunMode ruleRunMode)
        {
            if (param.HasInternalFilters() 
                || param.HasReferenceDataFilters() 
                || param.HasMarketCapFilters() 
                || param.HasVenueVolumeFilters())
            {
                _logger.LogInformation($"parameters had filters. Inserting filtered universe in {opCtx.Id} OpCtx");

                var filteredUniverse = _universeFilterFactory.Build(
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
                {
                    decoratedFilter = _decoratorFilterFactory.Build(
                        param.Windows,
                        filteredUniverse,
                        param.VenueVolumeFilter,
                        processOperationRunRuleContext,
                        universeDataRequestsSubscriber,
                        DataSourceForWindow(param.Windows),
                        ruleRunMode);
                }

                decoratedFilter.Subscribe(spoofingRule);

                return decoratedFilter;
            }
            else
            {
                return spoofingRule;
            }
        }
    }
}

