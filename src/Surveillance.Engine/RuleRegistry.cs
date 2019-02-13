using Contracts.SurveillanceService;
using Contracts.SurveillanceService.Interfaces;
using DomainV2.DTO;
using DomainV2.DTO.Interfaces;
using DomainV2.Equity.Streams.Interfaces;
using DomainV2.Scheduling;
using DomainV2.Scheduling.Interfaces;
using DomainV2.Streams;
using StructureMap;
using Surveillance.Engine.Rules.Analytics.Streams.Factory;
using Surveillance.Engine.Rules.Analytics.Streams.Factory.Interfaces;
using Surveillance.Engine.Rules.Analytics.Subscriber.Factory;
using Surveillance.Engine.Rules.Analytics.Subscriber.Factory.Interfaces;
using Surveillance.Engine.Rules.Currency;
using Surveillance.Engine.Rules.Currency.Interfaces;
using Surveillance.Engine.Rules.Data.Subscribers;
using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
using Surveillance.Engine.Rules.Factories;
using Surveillance.Engine.Rules.Factories.Interfaces;
using Surveillance.Engine.Rules.Interfaces;
using Surveillance.Engine.Rules.Mappers;
using Surveillance.Engine.Rules.Mappers.Interfaces;
using Surveillance.Engine.Rules.Mappers.RuleBreach;
using Surveillance.Engine.Rules.Mappers.RuleBreach.Interfaces;
using Surveillance.Engine.Rules.Markets;
using Surveillance.Engine.Rules.Markets.Interfaces;
using Surveillance.Engine.Rules.MessageBusIO;
using Surveillance.Engine.Rules.MessageBusIO.Interfaces;
using Surveillance.Engine.Rules.RuleParameters;
using Surveillance.Engine.Rules.RuleParameters.Filter;
using Surveillance.Engine.Rules.RuleParameters.Filter.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Manager;
using Surveillance.Engine.Rules.RuleParameters.Manager.Interfaces;
using Surveillance.Engine.Rules.Rules.CancelledOrders;
using Surveillance.Engine.Rules.Rules.CancelledOrders.Interfaces;
using Surveillance.Engine.Rules.Rules.HighProfits;
using Surveillance.Engine.Rules.Rules.HighProfits.Calculators;
using Surveillance.Engine.Rules.Rules.HighProfits.Calculators.Factories;
using Surveillance.Engine.Rules.Rules.HighProfits.Calculators.Factories.Interfaces;
using Surveillance.Engine.Rules.Rules.HighProfits.Calculators.Interfaces;
using Surveillance.Engine.Rules.Rules.HighProfits.Interfaces;
using Surveillance.Engine.Rules.Rules.HighVolume;
using Surveillance.Engine.Rules.Rules.HighVolume.Interfaces;
using Surveillance.Engine.Rules.Rules.Layering;
using Surveillance.Engine.Rules.Rules.Layering.Interfaces;
using Surveillance.Engine.Rules.Rules.MarkingTheClose;
using Surveillance.Engine.Rules.Rules.MarkingTheClose.Interfaces;
using Surveillance.Engine.Rules.Rules.Spoofing;
using Surveillance.Engine.Rules.Rules.Spoofing.Interfaces;
using Surveillance.Engine.Rules.Rules.WashTrade;
using Surveillance.Engine.Rules.Rules.WashTrade.Interfaces;
using Surveillance.Engine.Rules.Scheduler;
using Surveillance.Engine.Rules.Scheduler.Interfaces;
using Surveillance.Engine.Rules.test;
using Surveillance.Engine.Rules.test.Interfaces;
using Surveillance.Engine.Rules.Trades;
using Surveillance.Engine.Rules.Trades.Interfaces;
using Surveillance.Engine.Rules.Universe;
using Surveillance.Engine.Rules.Universe.Filter;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
using Surveillance.Engine.Rules.Universe.Interfaces;
using Surveillance.Engine.Rules.Universe.MarketEvents;
using Surveillance.Engine.Rules.Universe.MarketEvents.Interfaces;
using Surveillance.Engine.Rules.Universe.Multiverse;
using Surveillance.Engine.Rules.Universe.Multiverse.Interfaces;
using Surveillance.Engine.Rules.Universe.OrganisationalFactors;
using Surveillance.Engine.Rules.Universe.OrganisationalFactors.Interfaces;
using Surveillance.Engine.Rules.Universe.Subscribers;
using Surveillance.Engine.Rules.Universe.Subscribers.Factories;
using Surveillance.Engine.Rules.Universe.Subscribers.Interfaces;
using Surveillance.Engine.Rules.Utility;
using Surveillance.Engine.Rules.Utility.Interfaces;
using Surveillance.Engines.Interfaces.Mediator;

namespace Surveillance.Engine.Rules
{
    public class RuleRegistry : Registry
    {
        public RuleRegistry()
        {
            For<IRulesEngineMediator>().Use<Mediator>();
            For<IOriginFactory>().Use<OriginFactory>();
            For<ISpoofingRule>().Use<SpoofingRule>();

            For<ITradingHistory>().Use<TradingHistory>();
            For<ITradingHistoryStack>().Use<TradingHistoryStack>();
            For(typeof(IUnsubscriberFactory<>)).Use(typeof(UnsubscriberFactory<>));

            For<IReddeerRuleScheduler>().Use<ReddeerRuleScheduler>();
            For<ISpoofingRuleFactory>().Use<SpoofingRuleFactory>();
            For<IUniversePlayerFactory>().Use<UniversePlayerFactory>();
            For<IOrganisationalFactorBrokerFactory>().Use<OrganisationalFactorBrokerFactory>();

            For<ISpoofingSubscriber>().Use<SpoofingSubscriber>();
            For<ICancelledOrderSubscriber>().Use<CancelledOrderSubscriber>();
            For<IHighProfitsSubscriber>().Use<HighProfitsSubscriber>();
            For<IHighVolumeSubscriber>().Use<HighVolumeSubscriber>();
            For<IMarkingTheCloseSubscriber>().Use<MarkingTheCloseSubscriber>();
            For<ILayeringSubscriber>().Use<LayeringSubscriber>();
            For<IWashTradeSubscriber>().Use<WashTradeSubscriber>();

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

            For<IMarketOpenCloseEventManager>().Use<MarketOpenCloseEventManager>();
            For<IClientOrganisationalFactorMapper>().Use<ClientOrganisationalFactorMapper>();

            For<IScheduleRuleMessageSender>().Use<ScheduleRuleMessageSender>();
            For<IScheduledExecutionMessageBusSerialiser>().Use<ScheduledExecutionMessageBusSerialiser>();
            For<IScheduleExecutionDtoMapper>().Use<ScheduleExecutionDtoMapper>();
            For<IExchangeRateProfitCalculator>().Use<ExchangeRateProfitCalculator>();

            For<IMessageBusSerialiser>().Use<MessageBusSerialiser>();
            For<ICaseMessageSender>().Use<CaseMessageSender>();

            For<IThirdPartyDataRequestSerialiser>().Use<ThirdPartyDataRequestSerialiser>();
            For<IDataRequestMessageSender>().Use<DataRequestMessageSender>();

            For<ISpoofingRuleParameters>().Use<SpoofingRuleParameters>();
            For<ISpoofingRuleMessageSender>().Use<SpoofingRuleMessageSender>();

            For<ICancelledOrderMessageSender>().Use<CancelledOrderMessageSender>();
            For<ICancelledOrderRuleParameters>().Use<CancelledOrderRuleParameters>();
            For<ICancelledOrderRule>().Use<CancelledOrderRule>();
            For<ICancelledOrderRuleCachedMessageSender>().Use<CancelledOrderRuleCachedMessageSender>();
            For<ICancelledOrderRuleFactory>().Use<CancelledOrderRuleFactory>();

            For<IHighProfitRuleFactory>().Use<HighProfitRuleFactory>();
            For<IHighProfitMessageSender>().Use<HighProfitMessageSender>();
            For<IHighProfitRule>().Use<HighProfitsRule>();
            For<IHighProfitRuleCachedMessageSender>().Use<HighProfitRuleCachedMessageSender>();
            For<ICostCalculatorFactory>().Use<CostCalculatorFactory>();
            For<IRevenueCalculatorFactory>().Use<RevenueCalculatorFactory>();

            For<IMarkingTheCloseRule>().Use<MarkingTheCloseRule>();
            For<IMarkingTheCloseRuleFactory>().Use<MarkingTheCloseRuleFactory>();
            For<IMarkingTheCloseMessageSender>().Use<MarkingTheCloseMessageSender>();

            For<ILayeringRule>().Use<LayeringRule>();
            For<ILayeringRuleFactory>().Use<LayeringRuleFactory>();
            For<ILayeringAlertSender>().Use<LayeringAlertSender>();
            For<ILayeringCachedMessageSender>().Use<LayeringCachedMessageSender>();

            For<IHighVolumeRule>().Use<HighVolumeRule>();
            For<IHighVolumeRuleFactory>().Use<HighVolumeRuleFactory>();
            For<IHighVolumeMessageSender>().Use<HighVolumeMessageSender>();
            For<IHighVolumeRuleCachedMessageSender>().Use<HighVolumeRuleCachedMessageSender>();

            For<IHighProfitStreamRule>().Use<HighProfitStreamRule>();
            For<IMarketCloseMultiverseTransformer>()
                .Use<MarketCloseMultiverseTransformer>();

            For<IWashTradeRuleFactory>().Use<WashTradeRuleFactory>();
            For<IWashTradeRuleMessageSender>().Use<WashTradeRuleMessageSender>();
            For<IWashTradeCachedMessageSender>().Use<WashTradeCachedMessageSender>();
            For<IWashTradePositionPairer>().Use<WashTradePositionPairer>();
            For<IWashTradeClustering>().Use<WashTradeClustering>();

            For<IRuleParameterToRulesMapper>().Use<RuleParameterToRulesMapper>();
            For<ICurrencyConverter>().Use<CurrencyConverter>();
            For<IExchangeRates>().Use<ExchangeRates>();
            For<ITradePositionWeightedAverageExchangeRateCalculator>().Use<TradePositionWeightedAverageExchangeRateCalculator>();

            For<IApiHeartbeat>().Use<ApiHeartbeat>();
            For<IRuleProjector>().Use<RuleProjector>();

            For<IUniversePercentageCompletionLogger>().Use<UniversePercentageCompletionLogger>();
            For<IUniversePercentageOfEventCompletionLogger>().Use<UniversePercentageOfEventCompletionLogger>();
            For<IUniverseMarketCacheFactory>().Use<UniverseMarketCacheFactory>();
            For<IMarketTradingHoursManager>().Use<MarketTradingHoursManager>();
            For<IUniversePercentageOfTimeCompletionLogger>().Use<UniversePercentageOfTimeCompletionLogger>();

            For<IUniversePercentageOfEventCompletionLoggerFactory>().Use<UniversePercentageOfEventCompletionLoggerFactory>();
            For<IUniversePercentageOfTimeCompletionLoggerFactory>().Use<UniversePercentageOfTimeCompletionLoggerFactory>();
            For<IUniversePercentageCompletionLoggerFactory>().Use<UniversePercentageCompletionLoggerFactory>();
            For<IUniverseOrderFilter>().Use<UniverseOrderFilter>();

            For<IRuleRunUpdateMessageSender>().Use<RuleRunUpdateMessageSender>();
            For<IRuleParameterDtoIdExtractor>().Use<RuleParameterDtoIdExtractor>();
            For<IUniverseEquityInterDayCache>().Use<UniverseEquityInterDayCache>();

            For<IOrdersToAllocatedOrdersProjector>().Use<OrdersToAllocatedOrdersProjector>();
            For<IUniverseDataRequestsSubscriber>().Use<UniverseDataRequestsSubscriber>();
            For<IUniverseDataRequestsSubscriberFactory>().Use<UniverseDataRequestsSubscriberFactory>();

            For<IMarketDataCacheStrategyFactory>().Use<MarketDataCacheStrategyFactory>();
            For<IRuleParameterManager>().Use<RuleParameterManager>();
            For<IRuleParameterLeadingTimespanCalculator>().Use<RuleParameterLeadingTimespanCalculator>();


            For<IRuleBreachToRuleBreachOrdersMapper>().Use<RuleBreachToRuleBreachOrdersMapper>();
            For<IRuleBreachToRuleBreachMapper>().Use<RuleBreachToRuleBreachMapper>();

            For<IAnalysisEngine>().Use<AnalysisEngine>();
        }
    }
}
