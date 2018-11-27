﻿using System;
using System.Threading.Tasks;
using Domain.Scheduling;
using Microsoft.Extensions.Logging;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;
using Surveillance.DataLayer.Api.RuleParameter.Interfaces;
using Surveillance.Factories.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.Universe.Interfaces;
using Utilities.Extensions;
using System.Linq;
using Surveillance.Analytics.Streams.Interfaces;
using Surveillance.Universe.Filter.Interfaces;
using Surveillance.Factories;
using Surveillance.RuleParameters.Interfaces;
using Surveillance.Universe.Subscribers.Interfaces;

namespace Surveillance.Universe
{
    public class UniverseRuleSubscriber : IUniverseRuleSubscriber
    {
        private readonly ISpoofingSubscriber _spoofingSubscriber;
        private readonly ICancelledOrderRuleFactory _cancelledOrderRuleFactory;
        private readonly IHighProfitRuleFactory _highProfitRuleFactory;
        private readonly IHighVolumeRuleFactory _highVolumeRuleFactory;
        private readonly IMarkingTheCloseRuleFactory _markingTheCloseFactory;
        private readonly ILayeringRuleFactory _layeringRuleFactory;
        private readonly IWashTradeRuleFactory _washTradeRuleFactory;

        private readonly IRuleParameterApiRepository _ruleParameterApiRepository;
        private readonly IRuleParameterToRulesMapper _ruleParameterMapper;
        private readonly IUniverseFilterFactory _universeFilterFactory;
        private readonly ILogger _logger;

        public UniverseRuleSubscriber(
            ISpoofingSubscriber spoofingSubscriber,
            ICancelledOrderRuleFactory cancelledOrderRuleFactory,
            IHighProfitRuleFactory highProfitRuleFactory,
            IMarkingTheCloseRuleFactory markingTheCloseFactory,
            ILayeringRuleFactory layeringRuleFactory,
            IHighVolumeRuleFactory highVolumeRuleFactory,
            IRuleParameterApiRepository ruleParameterApiRepository,
            IRuleParameterToRulesMapper ruleParameterMapper,
            IUniverseFilterFactory universeFilterFactory,
            IWashTradeRuleFactory washTradeRuleFactory,
            ILogger<UniverseRuleSubscriber> logger)
        {
            _spoofingSubscriber = spoofingSubscriber ?? throw new ArgumentNullException(nameof(spoofingSubscriber));
            _cancelledOrderRuleFactory = cancelledOrderRuleFactory ?? throw new ArgumentNullException(nameof(cancelledOrderRuleFactory));
            _highProfitRuleFactory = highProfitRuleFactory ?? throw new ArgumentNullException(nameof(highProfitRuleFactory));
            _highVolumeRuleFactory =
                highVolumeRuleFactory
                ?? throw new ArgumentNullException(nameof(highVolumeRuleFactory));
            _markingTheCloseFactory = markingTheCloseFactory ?? throw new ArgumentNullException(nameof(markingTheCloseFactory));
            _layeringRuleFactory = layeringRuleFactory ?? throw new ArgumentNullException(nameof(layeringRuleFactory));
            _ruleParameterApiRepository = ruleParameterApiRepository
                ?? throw new ArgumentNullException(nameof(ruleParameterApiRepository));
            _ruleParameterMapper = ruleParameterMapper ?? throw new ArgumentNullException(nameof(ruleParameterMapper));
            _universeFilterFactory = universeFilterFactory ?? throw new ArgumentNullException(nameof(universeFilterFactory));
            _washTradeRuleFactory = washTradeRuleFactory ?? throw new ArgumentNullException(nameof(washTradeRuleFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task SubscribeRules(
             ScheduledExecution execution,
             IUniversePlayer player,
             IUniverseAlertStream alertStream,
             ISystemProcessOperationContext opCtx)
        {
            if (execution == null
                || player == null)
            {
                return;
            }

            var ruleParameters = await _ruleParameterApiRepository.Get();

            _spoofingSubscriber.SpoofingRule(execution, player, ruleParameters, opCtx, alertStream);
            CancelledOrdersRule(execution, player, ruleParameters, opCtx, alertStream);
            HighProfitsRule(execution, player, ruleParameters, opCtx, alertStream);
            MarkingTheCloseRule(execution, player, ruleParameters, opCtx, alertStream);
            LayeringRule(execution, player, ruleParameters, opCtx, alertStream);
            HighVolumeRule(execution, player, ruleParameters, opCtx, alertStream);
            WashTradeRule(execution, player, ruleParameters, opCtx, alertStream);
        }

        private void CancelledOrdersRule(
            ScheduledExecution execution,
            IUniversePlayer player,
            RuleParameterDto ruleParameters,
            ISystemProcessOperationContext opCtx,
            IUniverseAlertStream alertStream)
        {
            if (!execution.Rules?.Select(ab => ab.Rule)?.Contains(Domain.Scheduling.Rules.CancelledOrders) ?? true)
            {
                return;
            }

            var filteredParameters = execution.Rules.SelectMany(ru => ru.Ids).Where(ru => ru != null).ToList();
            var dtos =
                ruleParameters
                    .CancelledOrders
                    .Where(co => filteredParameters.Contains(co.Id, StringComparer.InvariantCultureIgnoreCase))
                    .ToList();

            var cancelledOrderParameters = _ruleParameterMapper.Map(dtos);

            if (cancelledOrderParameters != null
                && cancelledOrderParameters.Any())
            {
                foreach (var param in cancelledOrderParameters)
                {
                    var ruleCtx = opCtx
                        .CreateAndStartRuleRunContext(
                            Domain.Scheduling.Rules.CancelledOrders.GetDescription(),
                            CancelledOrderRuleFactory.Version,
                            execution.TimeSeriesInitiation.DateTime,
                            execution.TimeSeriesTermination.DateTime,
                            execution.CorrelationId);

                    var cancelledOrderRule = _cancelledOrderRuleFactory.Build(param, ruleCtx, alertStream);

                    if (param.HasFilters())
                    {
                        var filteredUniverse = _universeFilterFactory.Build(param.Accounts, param.Traders, param.Markets);
                        filteredUniverse.Subscribe(cancelledOrderRule);
                        player.Subscribe(filteredUniverse);
                    }
                    else
                    {
                        player.Subscribe(cancelledOrderRule);
                    }
                }
            }
            else
            {
                _logger.LogError("Rule Scheduler - tried to schedule a cancelled order rule execution with no parameters set");
                opCtx.EventError("Rule Scheduler - tried to schedule a cancelled order rule execution with no parameters set");
            }
        }

        private void HighProfitsRule(
            ScheduledExecution execution,
            IUniversePlayer player,
            RuleParameterDto ruleParameters,
            ISystemProcessOperationContext opCtx,
            IUniverseAlertStream alertStream)
        {
            if (!execution.Rules?.Select(ru => ru.Rule)?.Contains(Domain.Scheduling.Rules.HighProfits) ?? true)
            {
                return;
            }

            var filteredParameters = execution.Rules.SelectMany(ru => ru.Ids).Where(ru => ru != null).ToList();
            var dtos =
                ruleParameters
                    .HighProfits
                    .Where(hp => filteredParameters.Contains(hp.Id, StringComparer.InvariantCultureIgnoreCase))
                    .ToList();
            
            var highProfitParameters = _ruleParameterMapper.Map(dtos);

            if (highProfitParameters != null
                && highProfitParameters.Any())
            {
                foreach (var param in highProfitParameters)
                {

                    var ruleCtxStream = opCtx
                            .CreateAndStartRuleRunContext(
                                Domain.Scheduling.Rules.HighProfits.GetDescription(),
                                HighProfitRuleFactory.Version,
                                execution.TimeSeriesInitiation.DateTime,
                                execution.TimeSeriesTermination.DateTime,
                                execution.CorrelationId);

                    var ruleCtxMarketClosure = opCtx
                        .CreateAndStartRuleRunContext(
                            Domain.Scheduling.Rules.HighProfits.GetDescription(),
                            HighProfitRuleFactory.Version,
                            execution.TimeSeriesInitiation.DateTime,
                            execution.TimeSeriesTermination.DateTime,
                            execution.CorrelationId);

                    var highProfitsRule = _highProfitRuleFactory.Build(param, ruleCtxStream, ruleCtxMarketClosure, alertStream);
                    if (param.HasFilters())
                    {
                        var filteredUniverse = _universeFilterFactory.Build(param.Accounts, param.Traders, param.Markets);
                        filteredUniverse.Subscribe(highProfitsRule);
                        player.Subscribe(filteredUniverse);
                    }
                    else
                    {
                        player.Subscribe(highProfitsRule);
                    }
                }
            }
            else
            {
                _logger.LogError("Rule Scheduler - tried to schedule a high profit rule execution with no parameters set");
                opCtx.EventError("Rule Scheduler - tried to schedule a high profit rule execution with no parameters set");
            }
        }

        private void MarkingTheCloseRule(
            ScheduledExecution execution,
            IUniversePlayer player,
            RuleParameterDto ruleParameters,
            ISystemProcessOperationContext opCtx,
            IUniverseAlertStream alertStream)
        {
            if (!execution.Rules?.Select(ru => ru.Rule)?.Contains(Domain.Scheduling.Rules.MarkingTheClose) ?? true)
            {
                return;
            }

            var filteredParameters = execution.Rules.SelectMany(ru => ru.Ids).Where(ru => ru != null).ToList();
            var dtos =
                ruleParameters
                    .MarkingTheCloses
                    .Where(mtc => filteredParameters.Contains(mtc.Id, StringComparer.InvariantCultureIgnoreCase))
                    .ToList();

            var markingTheCloseParameters = _ruleParameterMapper.Map(dtos);

            if (markingTheCloseParameters != null
                && markingTheCloseParameters.Any())
            {
                foreach (var param in markingTheCloseParameters)
                {
                    var ruleCtx = opCtx
                        .CreateAndStartRuleRunContext(
                            Domain.Scheduling.Rules.MarkingTheClose.GetDescription(),
                            MarkingTheCloseRuleFactory.Version,
                            execution.TimeSeriesInitiation.DateTime,
                            execution.TimeSeriesTermination.DateTime,
                            execution.CorrelationId);

                    var markingTheClose = _markingTheCloseFactory.Build(param, ruleCtx, alertStream);

                    if (param.HasFilters())
                    {
                        var filteredUniverse = _universeFilterFactory.Build(param.Accounts, param.Traders, param.Markets);
                        filteredUniverse.Subscribe(markingTheClose);
                        player.Subscribe(filteredUniverse);
                    }
                    else
                    {
                        player.Subscribe(markingTheClose);
                    }
                }
            }
            else
            {
                _logger.LogError("Rule Scheduler - tried to schedule a marking the close rule execution with no parameters set");
                opCtx.EventError("Rule Scheduler - tried to schedule a marking the close rule execution with no parameters set");
            }
        }

        private void LayeringRule(
            ScheduledExecution execution,
            IUniversePlayer player,
            RuleParameterDto ruleParameters,
            ISystemProcessOperationContext opCtx,
            IUniverseAlertStream alertStream)
        {
            if (!execution.Rules?.Select(ru => ru.Rule)?.Contains(Domain.Scheduling.Rules.Layering) ?? true)
            {
                return;
            }

            var filteredParameters = execution.Rules.SelectMany(ru => ru.Ids).Where(ru => ru != null).ToList();
            var dtos =
                ruleParameters
                    .Layerings
                    .Where(la => filteredParameters.Contains(la.Id, StringComparer.InvariantCultureIgnoreCase))
                    .ToList();

            var layeringParameters = _ruleParameterMapper.Map(dtos);

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (layeringParameters != null
                && layeringParameters.Any())
            {
                foreach (var param in layeringParameters)
                {
                    var ruleCtx = opCtx
                        .CreateAndStartRuleRunContext(
                            Domain.Scheduling.Rules.Layering.GetDescription(),
                            LayeringRuleFactory.Version,
                            execution.TimeSeriesInitiation.DateTime,
                            execution.TimeSeriesTermination.DateTime,
                            execution.CorrelationId);

                    var layering = _layeringRuleFactory.Build(param, ruleCtx, alertStream);

                    if (param.HasFilters())
                    {
                        var filteredUniverse = _universeFilterFactory.Build(param.Accounts, param.Traders, param.Markets);
                        filteredUniverse.Subscribe(layering);
                        player.Subscribe(filteredUniverse);
                    }
                    else
                    {
                        player.Subscribe(layering);
                    }
                }
            }
            else
            {
                _logger.LogError("Rule Scheduler - tried to schedule a layering rule execution with no parameters set");
                opCtx.EventError("Rule Scheduler - tried to schedule a layering rule execution with no parameters set");
            }
        }

        private void HighVolumeRule(
            ScheduledExecution execution,
            IUniversePlayer player,
            RuleParameterDto ruleParameters,
            ISystemProcessOperationContext opCtx,
            IUniverseAlertStream alertStream)
        {
            if (!execution.Rules?.Select(ru => ru.Rule)?.Contains(Domain.Scheduling.Rules.HighVolume) ?? true)
            {
                return;
            }

            var filteredParameters = execution.Rules.SelectMany(ru => ru.Ids).Where(ru => ru != null).ToList();
            var dtos =
                ruleParameters
                    .HighVolumes
                    .Where(hv => filteredParameters.Contains(hv.Id, StringComparer.InvariantCultureIgnoreCase))
                    .ToList();

            var highVolumeParameters = _ruleParameterMapper.Map(dtos);

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (highVolumeParameters != null
                && highVolumeParameters.Any())
            {
                foreach (var param in highVolumeParameters)
                {
                    var ruleCtx = opCtx
                        .CreateAndStartRuleRunContext(
                            Domain.Scheduling.Rules.HighVolume.GetDescription(),
                            HighVolumeRuleFactory.Version,
                            execution.TimeSeriesInitiation.DateTime,
                            execution.TimeSeriesTermination.DateTime,
                            execution.CorrelationId);

                    var highVolume = _highVolumeRuleFactory.Build(param, ruleCtx, alertStream);

                    if (param.HasFilters())
                    {
                        var filteredUniverse = _universeFilterFactory.Build(param.Accounts, param.Traders, param.Markets);
                        filteredUniverse.Subscribe(highVolume);
                        player.Subscribe(filteredUniverse);
                    }
                    else
                    {
                        player.Subscribe(highVolume);
                    }
                }
            }
            else
            {
                _logger.LogError("Rule Scheduler - tried to schedule a high volume rule execution with no parameters set");
                opCtx.EventError("Rule Scheduler - tried to schedule a high volume rule execution with no parameters set");
            }
        }

        private void WashTradeRule(
            ScheduledExecution execution,
            IUniversePlayer player,
            RuleParameterDto ruleParameters,
            ISystemProcessOperationContext opCtx,
            IUniverseAlertStream alertStream)
        {
            if (!execution.Rules?.Select(ru => ru.Rule).Contains(Domain.Scheduling.Rules.WashTrade) ?? true)
            {
                return;
            }

            var filteredParameters = execution.Rules.SelectMany(ru => ru.Ids).Where(ru => ru != null).ToList();
            var dtos =
                ruleParameters
                    .WashTrades
                    .Where(wt => filteredParameters.Contains(wt.Id, StringComparer.InvariantCultureIgnoreCase))
                    .ToList();

            var washTradeParameters = _ruleParameterMapper.Map(dtos);

            if (washTradeParameters != null
                && washTradeParameters.Any())
            {
                foreach (var param in washTradeParameters)
                {
                    var ctx = opCtx.CreateAndStartRuleRunContext(
                        Domain.Scheduling.Rules.WashTrade.GetDescription(),
                        WashTradeRuleFactory.Version,
                        execution.TimeSeriesInitiation.DateTime,
                        execution.TimeSeriesTermination.DateTime,
                        execution.CorrelationId);

                    var washTrade = _washTradeRuleFactory.Build(param, ctx, alertStream);

                    if (param.HasFilters())
                    {
                        var filteredUniverse = _universeFilterFactory.Build(param.Accounts, param.Traders, param.Markets);
                        filteredUniverse.Subscribe(washTrade);
                        player.Subscribe(filteredUniverse);
                    }
                    else
                    {
                        player.Subscribe(washTrade);
                    }
                }
            }
            else
            {
                _logger.LogError("Rule Scheduler - tried to schedule a wash trade rule execution with no parameters set");
                opCtx.EventError("Rule Scheduler - tried to schedule a wash trade rule execution with no parameters set");
            }
        }
    }
}