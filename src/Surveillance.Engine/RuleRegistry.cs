using Surveillance.Engine.Rules.Rules.Shared.HighProfits.Calculators;
using Surveillance.Engine.Rules.Rules.Shared.HighProfits.Calculators.Factories;
using Surveillance.Engine.Rules.Rules.Shared.HighProfits.Calculators.Factories.Interfaces;
using Surveillance.Engine.Rules.Rules.Shared.HighProfits.Calculators.Interfaces;

namespace Surveillance.Engine.Rules
{
    using Domain.Core.Trading.Execution;
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

    using Surveillance.Data.Universe;
    using Surveillance.Data.Universe.Interfaces;
    using Surveillance.Data.Universe.Lazy;
    using Surveillance.Data.Universe.Lazy.Interfaces;
    using Surveillance.Data.Universe.MarketEvents;
    using Surveillance.Data.Universe.MarketEvents.Interfaces;
    using Surveillance.Data.Universe.Trades;
    using Surveillance.Data.Universe.Trades.Interfaces;
    using Surveillance.DataLayer.Aurora.Judgements;
    using Surveillance.DataLayer.Aurora.Judgements.Interfaces;
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
    using Surveillance.Engine.Rules.Judgements;
    using Surveillance.Engine.Rules.Judgements.Interfaces;
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
    using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.HighVolume;
    using Surveillance.Engine.Rules.Rules.Equity.HighVolume.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.Layering;
    using Surveillance.Engine.Rules.Rules.Equity.Layering.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.MarkingTheClose;
    using Surveillance.Engine.Rules.Rules.Equity.MarkingTheClose.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.PlacingOrderNoIntentToExecute;
    using Surveillance.Engine.Rules.Rules.Equity.PlacingOrderNoIntentToExecute.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.Ramping;
    using Surveillance.Engine.Rules.Rules.Equity.Ramping.Analysis;
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
    using Surveillance.Engine.Rules.Rules.FixedIncome.HighVolume;
    using Surveillance.Engine.Rules.Rules.FixedIncome.HighVolume.Interfaces;
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

    public class RuleRegistry : Registry
    {
        public RuleRegistry()
        {
            this.For<IRulesEngineMediator>().Use<Mediator>();
            this.For<IOriginFactory>().Use<OriginFactory>();
            this.For<ISpoofingRule>().Use<SpoofingRule>();

            this.For<ITradingHistoryStack>().Use<TradingHistoryStack>();
            this.For(typeof(IUnsubscriberFactory<>)).Use(typeof(UnsubscriberFactory<>));

            this.For<IQueueRuleSubscriber>().Use<QueueRuleSubscriber>();
            this.For<IEquityRuleSpoofingFactory>().Use<EquityRuleSpoofingFactory>();
            this.For<IUniversePlayerFactory>().Use<UniversePlayerFactory>();
            this.For<IOrganisationalFactorBrokerServiceFactory>().Use<OrganisationalFactorBrokerServiceFactory>();

            this.For<ISpoofingEquitySubscriber>().Use<SpoofingEquitySubscriber>();
            this.For<ICancelledOrderEquitySubscriber>().Use<CancelledOrderEquitySubscriber>();
            this.For<IHighProfitsEquitySubscriber>().Use<HighProfitsEquitySubscriber>();
            this.For<IHighVolumeEquitySubscriber>().Use<HighVolumeEquitySubscriber>();
            this.For<IMarkingTheCloseEquitySubscriber>().Use<MarkingTheCloseEquitySubscriber>();
            this.For<ILayeringEquitySubscriber>().Use<LayeringEquitySubscriber>();
            this.For<IWashTradeEquitySubscriber>().Use<WashTradeEquitySubscriber>();

            this.For<IUniverseEquityOrderFilterService>().Use<UniverseEquityOrderFilterService>();
            this.For<IUniverseFixedIncomeOrderFilterService>().Use<UniverseFixedIncomeOrderFilterService>();

            this.For<IFixedIncomeHighProfitJudgementMapper>().Use<FixedIncomeHighProfitJudgementMapper>();

            this.For<IUniverse>().Use<Surveillance.Data.Universe.Universe>();
            this.For<IUniverseBuilder>().Use<UniverseBuilder>();
            this.For<IUniverseEvent>().Use<UniverseEvent>();
            this.For<IUniversePlayer>().Use<UniversePlayer>();
            this.For<IUniverseRuleSubscriber>().Use<UniverseRuleSubscriber>();
            this.For<IUniverseFilterFactory>().Use<UniverseFilterFactory>();
            this.For<IUniverseSortComparer>().Use<UniverseEventComparer>();

            this.For<IUniverseAnalyticsSubscriberFactory>().Use<UniverseAnalyticsSubscriberFactory>();
            this.For<IUniverseAlertStreamFactory>().Use<UniverseAlertStreamFactory>();
            this.For<IUniverseAlertStreamSubscriberFactory>().Use<UniverseAlertStreamSubscriberFactory>();

            this.For<IMarketOpenCloseEventService>().Use<MarketOpenCloseEventService>();
            this.For<IClientOrganisationalFactorMapper>().Use<ClientOrganisationalFactorMapper>();

            this.For<IScheduledExecutionMessageBusSerialiser>().Use<ScheduledExecutionMessageBusSerialiser>();
            this.For<IScheduleExecutionDtoMapper>().Use<ScheduleExecutionDtoMapper>();
            this.For<IExchangeRateProfitCalculator>().Use<ExchangeRateProfitCalculator>();

            this.For<IMessageBusSerialiser>().Use<MessageBusSerialiser>();
            this.For<IQueueCasePublisher>().Use<QueueCasePublisher>();

            this.For<IThirdPartyDataRequestSerialiser>().Use<ThirdPartyDataRequestSerialiser>();
            this.For<IQueueDataSynchroniserRequestPublisher>().Use<QueueDataSynchroniserRequestPublisher>();

            this.For<ISpoofingRuleEquitiesParameters>().Use<SpoofingRuleEquitiesParameters>();
            this.For<ISpoofingRuleMessageSender>().Use<SpoofingRuleMessageSender>();

            this.For<ICancelledOrderMessageSender>().Use<CancelledOrderMessageSender>();
            this.For<ICancelledOrderRuleEquitiesParameters>().Use<CancelledOrderRuleEquitiesParameters>();
            this.For<ICancelledOrderRule>().Use<CancelledOrderRule>();
            this.For<ICancelledOrderRuleCachedMessageSender>().Use<CancelledOrderRuleCachedMessageSender>();
            this.For<IEquityRuleCancelledOrderFactory>().Use<EquityRuleCancelledOrderFactory>();

            this.For<IEquityRuleHighProfitFactory>().Use<EquityRuleHighProfitFactory>();
            this.For<IHighProfitJudgementMapper>().Use<HighProfitJudgementMapper>();
            this.For<IHighProfitRule>().Use<HighProfitsRule>();
            this.For<ICostCalculatorFactory>().Use<CostCalculatorFactory>();
            this.For<IRevenueCalculatorFactory>().Use<RevenueCalculatorFactory>();

            this.For<IMarkingTheCloseRule>().Use<MarkingTheCloseRule>();
            this.For<IEquityRuleMarkingTheCloseFactory>().Use<EquityRuleMarkingTheCloseFactory>();
            this.For<IMarkingTheCloseMessageSender>().Use<MarkingTheCloseMessageSender>();

            this.For<ILayeringRule>().Use<LayeringRule>();
            this.For<IEquityRuleLayeringFactory>().Use<EquityRuleLayeringFactory>();
            this.For<ILayeringAlertSender>().Use<LayeringAlertSender>();
            this.For<ILayeringCachedMessageSender>().Use<LayeringCachedMessageSender>();

            this.For<IHighVolumeRule>().Use<HighVolumeRule>();
            this.For<IEquityRuleHighVolumeFactory>().Use<EquityRuleHighVolumeFactory>();
            this.For<IHighVolumeMessageSender>().Use<HighVolumeMessageSender>();
            this.For<IHighVolumeRuleCachedMessageSender>().Use<HighVolumeRuleCachedMessageSender>();

            this.For<IHighProfitStreamRule>().Use<HighProfitStreamRule>();

            this.For<IEquityRuleWashTradeFactory>().Use<EquityRuleWashTradeFactory>();
            this.For<IWashTradeRuleMessageSender>().Use<WashTradeRuleMessageSender>();
            this.For<IWashTradeCachedMessageSender>().Use<WashTradeCachedMessageSender>();
            this.For<IClusteringService>().Use<ClusteringService>().Singleton();

            this.For<IHighVolumeFixedIncomeSubscriber>().Use<HighVolumeFixedIncomeSubscriber>();
            this.For<IFixedIncomeHighVolumeRule>().Use<FixedIncomeHighVolumeRule>();
            this.For<IFixedIncomeHighVolumeFactory>().Use<FixedIncomeHighVolumeFactory>();

            this.For<IHighProfitsFixedIncomeSubscriber>().Use<HighProfitsFixedIncomeSubscriber>();
            this.For<IFixedIncomeHighProfitsRule>().Use<FixedIncomeHighProfitsRule>();
            this.For<IFixedIncomeHighProfitFactory>().Use<FixedIncomeHighProfitFactory>();
            this.For<IFixedIncomeHighProfitsStreamRule>().Use<FixedIncomeHighProfitsStreamRule>();

            this.For<IWashTradeFixedIncomeSubscriber>().Use<WashTradeFixedIncomeSubscriber>();
            this.For<IFixedIncomeWashTradeRule>().Use<FixedIncomeWashTradeRule>();
            this.For<IFixedIncomeWashTradeFactory>().Use<FixedIncomeWashTradeFactory>();

            this.For<IRampingEquitySubscriber>().Use<RampingEquitySubscriber>();
            this.For<IRampingRule>().Use<RampingRule>();
            this.For<IEquityRuleRampingFactory>().Use<EquityRuleRampingFactory>();
            this.For<IRampingRuleMessageSender>().Use<RampingRuleMessageSender>();
            this.For<IRampingRuleCachedMessageSender>().Use<RampingRuleCachedMessageSender>();

            this.For<IPortfolioFactory>().Use<PortfolioFactory>();

            this.For<IRuleParameterToRulesMapper>().Use<RuleParameterDtoToRuleParameterMapper>();
            this.For<ICurrencyConverterService>().Use<CurrencyConverterService>();
            this.For<IExchangeRatesService>().Use<ExchangeRatesService>();
            this.For<ITradePositionWeightedAverageExchangeRateService>()
                .Use<TradePositionWeightedAverageExchangeRateService>();

            this.For<IApiHeartbeat>().Use<ApiHeartbeat>();
            this.For<IRuleFilterProjector>().Use<RuleFilterProjector>();
            this.For<IDecimalRangeRuleFilterProjector>().Use<DecimalRangeRuleFilterProjector>();

            this.For<IUniversePercentageCompletionLogger>().Use<UniversePercentageCompletionLogger>();
            this.For<IUniversePercentageOfEventCompletionLogger>().Use<UniversePercentageOfEventCompletionLogger>();
            this.For<IUniverseMarketCacheFactory>().Use<UniverseMarketCacheFactory>();
            this.For<IMarketTradingHoursService>().Use<MarketTradingHoursService>();
            this.For<IUniversePercentageOfTimeCompletionLogger>().Use<UniversePercentageOfTimeCompletionLogger>();

            this.For<IUniversePercentageOfEventCompletionLoggerFactory>()
                .Use<UniversePercentageOfEventCompletionLoggerFactory>();
            this.For<IUniversePercentageOfTimeCompletionLoggerFactory>()
                .Use<UniversePercentageOfTimeCompletionLoggerFactory>();
            this.For<IUniversePercentageCompletionLoggerFactory>().Use<UniversePercentageCompletionLoggerFactory>();

            this.For<IQueueRuleUpdatePublisher>().Use<QueueRuleUpdatePublisher>();
            this.For<IRuleParameterDtoIdExtractor>().Use<RuleParameterDtoIdExtractor>();
            this.For<IUniverseEquityInterDayCache>().Use<UniverseEquityInterDayCache>();

            this.For<IOrdersToAllocatedOrdersProjector>().Use<OrdersToAllocatedOrdersProjector>();
            this.For<IUniverseDataRequestsSubscriber>().Use<UniverseDataRequestsSubscriber>();
            this.For<IUniverseDataRequestsSubscriberFactory>().Use<UniverseDataRequestsSubscriberFactory>();

            this.For<IMarketDataCacheStrategyFactory>().Use<MarketDataCacheStrategyFactory>();
            this.For<IRuleParameterService>().Use<RuleParameterService>();
            this.For<IRuleParameterAdjustedTimespanService>().Use<RuleParameterAdjustedTimespanService>();

            this.For<IRuleBreachToRuleBreachOrdersMapper>().Use<RuleBreachToRuleBreachOrdersMapper>();
            this.For<IRuleBreachToRuleBreachMapper>().Use<RuleBreachToRuleBreachMapper>();

            this.For<IPlacingOrdersWithNoIntentToExecuteMessageSender>()
                .Use<PlacingOrdersWithNoIntentToExecuteMessageSender>();
            this.For<IPlacingOrdersWithNoIntentToExecuteCacheMessageSender>()
                .Use<PlacingOrdersWithNoIntentToExecuteCacheMessageSender>();
            this.For<IPlacingOrdersWithNoIntentToExecuteRule>().Use<PlacingOrdersWithNoIntentToExecuteRule>();
            this.For<IEquityRulePlacingOrdersWithoutIntentToExecuteFactory>()
                .Use<EquityRulePlacingOrdersWithoutIntentToExecuteFactory>();
            this.For<IPlacingOrdersWithNoIntentToExecuteEquitySubscriber>()
                .Use<PlacingOrdersWithNoIntentToExecuteEquitySubscriber>();

            this.For<ILazyScheduledExecutioner>().Use<LazyScheduledExecutioner>();
            this.For<IOrderAnalysisService>().Use<OrderAnalysisService>();
            this.For<ILazyTransientUniverseFactory>().Use<LazyTransientUniverseFactory>();
            this.For<ITimeSeriesTrendClassifier>().Use<TimeSeriesTrendClassifier>();

            this.For<IOrderAnalysisService>().Use<OrderAnalysisService>();
            this.For<IOrderPriceImpactClassifier>().Use<OrderPriceImpactClassifier>();

            this.For<IAnalysisEngine>().Use<AnalysisEngine>();
            this.For<IRampingAnalyser>().Use<RampingAnalyser>();

            this.For<IQueueRuleCancellationSubscriber>().Use<QueueRuleCancellationSubscriber>();
            this.For<IRuleCancellation>().Use<RuleCancellation>().Singleton();

            this.For<IRuleParameterTuner>().Use<RuleParameterTuner>();
            this.For<ITaskReSchedulerService>().Use<TaskReSchedulerService>();

            this.For<IHighMarketCapFilterFactory>().Use<HighMarketCapFilterFactory>();
            this.For<IRuleParameterToRulesMapperDecorator>().Use<RuleParameterToRulesMapperTuningDecorator>();

            this.For<IHighVolumeVenueFilter>().Use<HighVolumeVenueFilter>();
            this.For<IJudgementService>().Use<JudgementService>();
            this.For<IHighVolumeVenueDecoratorFilter>().Use<HighVolumeVenueDecoratorFilter>();

            this.For<IHighVolumeVenueDecoratorFilterFactory>().Use<HighVolumeVenueDecoratorFilterFactory>();
            this.For<IJudgementRepository>().Use<JudgementRepository>();
            this.For<IJudgementServiceFactory>().Use<JudgementServiceFactory>();
            this.For<IRuleViolationServiceFactory>().Use<RuleViolationServiceFactory>();

            this.For<ICancelledOrderJudgementService>().Use<JudgementService>();
            this.For<IHighProfitJudgementService>().Use<JudgementService>();
            this.For<IHighVolumeJudgementService>().Use<JudgementService>();
            this.For<ILayeringJudgementService>().Use<JudgementService>();
            this.For<IMarkingTheCloseJudgementService>().Use<JudgementService>();
            this.For<IPlacingOrdersJudgementService>().Use<JudgementService>();
            this.For<IRampingJudgementService>().Use<JudgementService>();
            this.For<ISpoofingJudgementService>().Use<JudgementService>();
            this.For<IFixedIncomeHighProfitJudgementService>().Use<JudgementService>();
            this.For<IFixedIncomeHighVolumeJudgementService>().Use<JudgementService>();
            this.For<IFixedIncomeHighVolumeJudgementMapper>().Use<FixedIncomeHighVolumeJudgementMapper>();
        }
    }
}