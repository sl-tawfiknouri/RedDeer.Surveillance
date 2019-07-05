﻿using Domain.Core.Trading.Execution;
using Domain.Core.Trading.Execution.Interfaces;
using Domain.Core.Trading.Factories;
using Domain.Core.Trading.Factories.Interfaces;
using Domain.Surveillance.Scheduling;
using Domain.Surveillance.Scheduling.Interfaces;
using Domain.Surveillance.Streams;
using Domain.Surveillance.Streams.Interfaces;
using RedDeer.Contracts.SurveillanceService;
using RedDeer.Contracts.SurveillanceService.Interfaces;
using SharedKernel.Contracts.Queues;
using SharedKernel.Contracts.Queues.Interfaces;
using StructureMap;
using Surveillance.Engine.Rules.Analysis;
using Surveillance.Engine.Rules.Analysis.Interfaces;
using Surveillance.Engine.Rules.Analytics.Streams.Factory;
using Surveillance.Engine.Rules.Analytics.Streams.Factory.Interfaces;
using Surveillance.Engine.Rules.Analytics.Subscriber.Factory;
using Surveillance.Engine.Rules.Analytics.Subscriber.Factory.Interfaces;
using Surveillance.Engine.Rules.Currency;
using Surveillance.Engine.Rules.Currency.Interfaces;
using Surveillance.Engine.Rules.Data.Subscribers;
using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
using Surveillance.Engine.Rules.Factories;
using Surveillance.Engine.Rules.Factories.Equities;
using Surveillance.Engine.Rules.Factories.Equities.Interfaces;
using Surveillance.Engine.Rules.Factories.FixedIncome;
using Surveillance.Engine.Rules.Factories.FixedIncome.Interfaces;
using Surveillance.Engine.Rules.Factories.Interfaces;
using Surveillance.Engine.Rules.Interfaces;
using Surveillance.Engine.Rules.Mappers;
using Surveillance.Engine.Rules.Mappers.Interfaces;
using Surveillance.Engine.Rules.Mappers.RuleBreach;
using Surveillance.Engine.Rules.Mappers.RuleBreach.Interfaces;
using Surveillance.Engine.Rules.Markets;
using Surveillance.Engine.Rules.Markets.Interfaces;
using Surveillance.Engine.Rules.Queues;
using Surveillance.Engine.Rules.Queues.Interfaces;
using Surveillance.Engine.Rules.RuleParameters;
using Surveillance.Engine.Rules.RuleParameters.Equities;
using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Filter;
using Surveillance.Engine.Rules.RuleParameters.Filter.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Services;
using Surveillance.Engine.Rules.RuleParameters.Services.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Tuning;
using Surveillance.Engine.Rules.RuleParameters.Tuning.Interfaces;
using Surveillance.Engine.Rules.Rules.Cancellation;
using Surveillance.Engine.Rules.Rules.Cancellation.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.CancelledOrders;
using Surveillance.Engine.Rules.Rules.Equity.CancelledOrders.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.HighProfits;
using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators;
using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators.Factories;
using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators.Factories.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.HighVolume;
using Surveillance.Engine.Rules.Rules.Equity.HighVolume.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.Layering;
using Surveillance.Engine.Rules.Rules.Equity.Layering.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.MarkingTheClose;
using Surveillance.Engine.Rules.Rules.Equity.MarkingTheClose.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.Ramping;
using Surveillance.Engine.Rules.Rules.Equity.Ramping.Analysis;
using Surveillance.Engine.Rules.Rules.Equity.PlacingOrderNoIntentToExecute;
using Surveillance.Engine.Rules.Rules.Equity.PlacingOrderNoIntentToExecute.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.Ramping.Analysis.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.Ramping.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.Ramping.OrderAnalysis;
using Surveillance.Engine.Rules.Rules.Equity.Ramping.OrderAnalysis.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.Ramping.TimeSeries;
using Surveillance.Engine.Rules.Rules.Equity.Ramping.TimeSeries.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.Spoofing;
using Surveillance.Engine.Rules.Rules.Equity.Spoofing.Interfaces;
using Surveillance.Engine.Rules.Rules.FixedIncome.HighProfits;
using Surveillance.Engine.Rules.Rules.FixedIncome.HighProfits.Interfaces;
using Surveillance.Engine.Rules.Rules.FixedIncome.HighVolumeIssuance;
using Surveillance.Engine.Rules.Rules.FixedIncome.HighVolumeIssuance.Interfaces;
using Surveillance.Engine.Rules.Rules.FixedIncome.WashTrade;
using Surveillance.Engine.Rules.Rules.FixedIncome.WashTrade.Interfaces;
using Surveillance.Engine.Rules.Rules.Shared.WashTrade;
using Surveillance.Engine.Rules.Rules.Shared.WashTrade.Interfaces;
using Surveillance.Engine.Rules.Trades;
using Surveillance.Engine.Rules.Trades.Interfaces;
using Surveillance.Engine.Rules.Universe;
using Surveillance.Engine.Rules.Universe.Filter;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
using Surveillance.Engine.Rules.Universe.Interfaces;
using Surveillance.Engine.Rules.Universe.Lazy;
using Surveillance.Engine.Rules.Universe.Lazy.Interfaces;
using Surveillance.Engine.Rules.Universe.MarketEvents;
using Surveillance.Engine.Rules.Universe.MarketEvents.Interfaces;
using Surveillance.Engine.Rules.Universe.OrganisationalFactors;
using Surveillance.Engine.Rules.Universe.OrganisationalFactors.Interfaces;
using Surveillance.Engine.Rules.Universe.Subscribers;
using Surveillance.Engine.Rules.Universe.Subscribers.Equity;
using Surveillance.Engine.Rules.Universe.Subscribers.Equity.Interfaces;
using Surveillance.Engine.Rules.Universe.Subscribers.Factories;
using Surveillance.Engine.Rules.Universe.Subscribers.FixedIncome;
using Surveillance.Engine.Rules.Universe.Subscribers.FixedIncome.Interfaces;
using Surveillance.Engine.Rules.Universe.Subscribers.Interfaces;
using Surveillance.Engine.Rules.Utility;
using Surveillance.Engine.Rules.Utility.Interfaces;
using Surveillance.Engine.Rules.Judgements.Interfaces;
using Surveillance.Engine.Rules.Judgements;

namespace Surveillance.Engine.Rules
{
    public class RuleRegistry : Registry
    {
        public RuleRegistry()
        {
            For<IRulesEngineMediator>().Use<Mediator>();
            For<IOriginFactory>().Use<OriginFactory>();
            For<ISpoofingRule>().Use<SpoofingRule>();

            For<ITradingHistoryStack>().Use<TradingHistoryStack>();
            For(typeof(IUnsubscriberFactory<>)).Use(typeof(UnsubscriberFactory<>));

            For<IQueueRuleSubscriber>().Use<QueueRuleSubscriber>();
            For<IEquityRuleSpoofingFactory>().Use<EquityRuleSpoofingFactory>();
            For<IUniversePlayerFactory>().Use<UniversePlayerFactory>();
            For<IOrganisationalFactorBrokerServiceFactory>().Use<OrganisationalFactorBrokerServiceFactory>();

            For<ISpoofingEquitySubscriber>().Use<SpoofingEquitySubscriber>();
            For<ICancelledOrderEquitySubscriber>().Use<CancelledOrderEquitySubscriber>();
            For<IHighProfitsEquitySubscriber>().Use<HighProfitsEquitySubscriber>();
            For<IHighVolumeEquitySubscriber>().Use<HighVolumeEquitySubscriber>();
            For<IMarkingTheCloseEquitySubscriber>().Use<MarkingTheCloseEquitySubscriber>();
            For<ILayeringEquitySubscriber>().Use<LayeringEquitySubscriber>();
            For<IWashTradeEquitySubscriber>().Use<WashTradeEquitySubscriber>();

            For<IUniverseEquityOrderFilterService>().Use<UniverseEquityOrderFilterService>();
            For<IUniverseFixedIncomeOrderFilterService>().Use<UniverseFixedIncomeOrderFilterService>();

            For<IUniverse>().Use<Universe.Universe>();
            For<IUniverseBuilder>().Use<UniverseBuilder>();
            For<IUniverseEvent>().Use<UniverseEvent>();
            For<IUniversePlayer>().Use<UniversePlayer>();
            For<IUniverseRuleSubscriber>().Use<UniverseRuleSubscriber>();
            For<IUniverseFilterFactory>().Use<UniverseFilterFactory>();
            For<IUniverseSortComparer>().Use<UniverseEventComparer>();

            For<IUniverseAnalyticsSubscriberFactory>().Use<UniverseAnalyticsSubscriberFactory>();
            For<IUniverseAlertStreamFactory>().Use<UniverseAlertStreamFactory>();
            For<IUniverseAlertStreamSubscriberFactory>().Use<UniverseAlertStreamSubscriberFactory>();

            For<IMarketOpenCloseEventService>().Use<MarketOpenCloseEventService>();
            For<IClientOrganisationalFactorMapper>().Use<ClientOrganisationalFactorMapper>();

            For<IScheduledExecutionMessageBusSerialiser>().Use<ScheduledExecutionMessageBusSerialiser>();
            For<IScheduleExecutionDtoMapper>().Use<ScheduleExecutionDtoMapper>();
            For<IExchangeRateProfitCalculator>().Use<ExchangeRateProfitCalculator>();

            For<IMessageBusSerialiser>().Use<MessageBusSerialiser>();
            For<IQueueCasePublisher>().Use<QueueCasePublisher>();

            For<IThirdPartyDataRequestSerialiser>().Use<ThirdPartyDataRequestSerialiser>();
            For<IQueueDataSynchroniserRequestPublisher>().Use<QueueDataSynchroniserRequestPublisher>();

            For<ISpoofingRuleEquitiesParameters>().Use<SpoofingRuleEquitiesParameters>();
            For<ISpoofingRuleMessageSender>().Use<SpoofingRuleMessageSender>();

            For<ICancelledOrderMessageSender>().Use<CancelledOrderMessageSender>();
            For<ICancelledOrderRuleEquitiesParameters>().Use<CancelledOrderRuleEquitiesParameters>();
            For<ICancelledOrderRule>().Use<CancelledOrderRule>();
            For<ICancelledOrderRuleCachedMessageSender>().Use<CancelledOrderRuleCachedMessageSender>();
            For<IEquityRuleCancelledOrderFactory>().Use<EquityRuleCancelledOrderFactory>();

            For<IEquityRuleHighProfitFactory>().Use<EquityRuleHighProfitFactory>();
            For<IHighProfitMessageSender>().Use<HighProfitMessageSender>();
            For<IHighProfitRule>().Use<HighProfitsRule>();
            For<IHighProfitRuleCachedMessageSender>().Use<HighProfitRuleCachedMessageSender>();
            For<ICostCalculatorFactory>().Use<CostCalculatorFactory>();
            For<IRevenueCalculatorFactory>().Use<RevenueCalculatorFactory>();

            For<IMarkingTheCloseRule>().Use<MarkingTheCloseRule>();
            For<IEquityRuleMarkingTheCloseFactory>().Use<EquityRuleMarkingTheCloseFactory>();
            For<IMarkingTheCloseMessageSender>().Use<MarkingTheCloseMessageSender>();

            For<ILayeringRule>().Use<LayeringRule>();
            For<IEquityRuleLayeringFactory>().Use<EquityRuleLayeringFactory>();
            For<ILayeringAlertSender>().Use<LayeringAlertSender>();
            For<ILayeringCachedMessageSender>().Use<LayeringCachedMessageSender>();

            For<IHighVolumeRule>().Use<HighVolumeRule>();
            For<IEquityRuleHighVolumeFactory>().Use<EquityRuleHighVolumeFactory>();
            For<IHighVolumeMessageSender>().Use<HighVolumeMessageSender>();
            For<IHighVolumeRuleCachedMessageSender>().Use<HighVolumeRuleCachedMessageSender>();

            For<IHighProfitStreamRule>().Use<HighProfitStreamRule>();

            For<IEquityRuleWashTradeFactory>().Use<EquityRuleWashTradeFactory>();
            For<IWashTradeRuleMessageSender>().Use<WashTradeRuleMessageSender>();
            For<IWashTradeCachedMessageSender>().Use<WashTradeCachedMessageSender>();
            For<IClusteringService>().Use<ClusteringService>().Singleton();


            For<IHighVolumeFixedIncomeSubscriber>().Use<HighVolumeFixedIncomeSubscriber>();
            For<IFixedIncomeHighVolumeRule>().Use<FixedIncomeHighVolumeIssuanceRule>();
            For<IFixedIncomeHighVolumeFactory>().Use<FixedIncomeHighVolumeFactory>();

            For<IHighProfitsFixedIncomeSubscriber>().Use<HighProfitsFixedIncomeSubscriber>();
            For<IFixedIncomeHighProfitsRule>().Use<FixedIncomeHighProfitsRule>();
            For<IFixedIncomeHighProfitFactory>().Use<FixedIncomeHighProfitFactory>();

            For<IWashTradeFixedIncomeSubscriber>().Use<WashTradeFixedIncomeSubscriber>();
            For<IFixedIncomeWashTradeRule>().Use<FixedIncomeWashTradeRule>();
            For<IFixedIncomeWashTradeFactory>().Use<FixedIncomeWashTradeFactory>();

            For<IRampingEquitySubscriber>().Use<RampingEquitySubscriber>();
            For<IRampingRule>().Use<RampingRule>();
            For<IEquityRuleRampingFactory>().Use<EquityRuleRampingFactory>();
            For<IRampingRuleMessageSender>().Use<RampingRuleMessageSender>();
            For<IRampingRuleCachedMessageSender>().Use<RampingRuleCachedMessageSender>();

            For<IPortfolioFactory>().Use<PortfolioFactory>();
            
            For<IRuleParameterToRulesMapper>().Use<RuleParameterDtoToRuleParameterMapper>();
            For<ICurrencyConverterService>().Use<CurrencyConverterService>();
            For<IExchangeRatesService>().Use<ExchangeRatesService>();
            For<ITradePositionWeightedAverageExchangeRateService>().Use<TradePositionWeightedAverageExchangeRateService>();

            For<IApiHeartbeat>().Use<ApiHeartbeat>();
            For<IRuleProjector>().Use<RuleProjector>();
            For<IDecimalRangeRuleFilterProjector>().Use<DecimalRangeRuleFilterProjector>();

            For<IUniversePercentageCompletionLogger>().Use<UniversePercentageCompletionLogger>();
            For<IUniversePercentageOfEventCompletionLogger>().Use<UniversePercentageOfEventCompletionLogger>();
            For<IUniverseMarketCacheFactory>().Use<UniverseMarketCacheFactory>();
            For<IMarketTradingHoursService>().Use<MarketTradingHoursService>();
            For<IUniversePercentageOfTimeCompletionLogger>().Use<UniversePercentageOfTimeCompletionLogger>();

            For<IUniversePercentageOfEventCompletionLoggerFactory>().Use<UniversePercentageOfEventCompletionLoggerFactory>();
            For<IUniversePercentageOfTimeCompletionLoggerFactory>().Use<UniversePercentageOfTimeCompletionLoggerFactory>();
            For<IUniversePercentageCompletionLoggerFactory>().Use<UniversePercentageCompletionLoggerFactory>();

            For<IQueueRuleUpdatePublisher>().Use<QueueRuleUpdatePublisher>();
            For<IRuleParameterDtoIdExtractor>().Use<RuleParameterDtoIdExtractor>();
            For<IUniverseEquityInterDayCache>().Use<UniverseEquityInterDayCache>();

            For<IOrdersToAllocatedOrdersProjector>().Use<OrdersToAllocatedOrdersProjector>();
            For<IUniverseDataRequestsSubscriber>().Use<UniverseDataRequestsSubscriber>();
            For<IUniverseDataRequestsSubscriberFactory>().Use<UniverseDataRequestsSubscriberFactory>();

            For<IMarketDataCacheStrategyFactory>().Use<MarketDataCacheStrategyFactory>();
            For<IRuleParameterService>().Use<RuleParameterService>();
            For<IRuleParameterAdjustedTimespanService>().Use<RuleParameterAdjustedTimespanService>();

            For<IRuleBreachToRuleBreachOrdersMapper>().Use<RuleBreachToRuleBreachOrdersMapper>();
            For<IRuleBreachToRuleBreachMapper>().Use<RuleBreachToRuleBreachMapper>();

            For<IPlacingOrdersWithNoIntentToExecuteMessageSender>().Use<PlacingOrdersWithNoIntentToExecuteMessageSender>();
            For<IPlacingOrdersWithNoIntentToExecuteCacheMessageSender>().Use<PlacingOrdersWithNoIntentToExecuteCacheMessageSender>();
            For<IPlacingOrdersWithNoIntentToExecuteRule>().Use<PlacingOrdersWithNoIntentToExecuteRule>();
            For<IEquityRulePlacingOrdersWithoutIntentToExecuteFactory>().Use<EquityRulePlacingOrdersWithoutIntentToExecuteFactory>();
            For<IPlacingOrdersWithNoIntentToExecuteEquitySubscriber>().Use<PlacingOrdersWithNoIntentToExecuteEquitySubscriber>();

            For<ILazyScheduledExecutioner>().Use<LazyScheduledExecutioner>();
            For<IOrderAnalysisService>().Use<OrderAnalysisService>();
            For<ILazyTransientUniverseFactory>().Use<LazyTransientUniverseFactory>();
            For<ITimeSeriesTrendClassifier>().Use<TimeSeriesTrendClassifier>();

            For<IOrderAnalysisService>().Use<OrderAnalysisService>();
            For<IOrderPriceImpactClassifier>().Use<OrderPriceImpactClassifier>();

            For<IAnalysisEngine>().Use<AnalysisEngine>();
            For<IRampingAnalyser>().Use<RampingAnalyser>();

            For<IQueueRuleCancellationSubscriber>().Use<QueueRuleCancellationSubscriber>();
            For<IRuleCancellation>().Use<RuleCancellation>().Singleton();

            For<IRuleParameterTuner>().Use<RuleParameterTuner>();
            For<ITaskReSchedulerService>().Use<TaskReSchedulerService>();

            For<IHighMarketCapFilterFactory>().Use<HighMarketCapFilterFactory>();
            For<IRuleParameterToRulesMapperDecorator>().Use<RuleParameterToRulesMapperTuningDecorator>();

            For<IHighVolumeVenueFilter>().Use<HighVolumeVenueFilter>();
            For<IJudgementService>().Use<JudgementService>();
            For<IHighVolumeVenueDecoratorFilter>().Use<HighVolumeVenueDecoratorFilter>();

            For<IHighVolumeVenueDecoratorFilterFactory>().Use<HighVolumeVenueDecoratorFilterFactory>();
        }
    }
}
