using Contracts.SurveillanceService;
using Contracts.SurveillanceService.Interfaces;
using DomainV2.Equity.Streams.Interfaces;
using DomainV2.Scheduling;
using DomainV2.Scheduling.Interfaces;
using DomainV2.Streams;
using StructureMap;
using NLog.Extensions.Logging;
using Microsoft.Extensions.Logging;
using Surveillance.Analytics.Streams.Factory;
using Surveillance.Analytics.Streams.Factory.Interfaces;
using Surveillance.Analytics.Subscriber.Factory;
using Surveillance.Analytics.Subscriber.Factory.Interfaces;
using Surveillance.Currency;
using Surveillance.Currency.Interfaces;
using Surveillance.Services;
using Surveillance.Services.Interfaces;
using Surveillance.Factories.Interfaces;
using Surveillance.Factories;
using Surveillance.Interfaces;
using Surveillance.Mappers;
using Surveillance.Mappers.Interfaces;
using Surveillance.Markets;
using Surveillance.Markets.Interfaces;
using Surveillance.MessageBusIO;
using Surveillance.MessageBusIO.Interfaces;
using Surveillance.RuleParameters;
using Surveillance.RuleParameters.Filter;
using Surveillance.RuleParameters.Filter.Interfaces;
using Surveillance.RuleParameters.Interfaces;
using Surveillance.Rules.CancelledOrders;
using Surveillance.Rules.CancelledOrders.Interfaces;
using Surveillance.Rules.HighProfits;
using Surveillance.Rules.HighProfits.Calculators;
using Surveillance.Rules.HighProfits.Calculators.Factories;
using Surveillance.Rules.HighProfits.Calculators.Factories.Interfaces;
using Surveillance.Rules.HighProfits.Calculators.Interfaces;
using Surveillance.Rules.HighProfits.Interfaces;
using Surveillance.Rules.HighVolume;
using Surveillance.Rules.HighVolume.Interfaces;
using Surveillance.Rules.Layering;
using Surveillance.Rules.Layering.Interfaces;
using Surveillance.Rules.MarkingTheClose;
using Surveillance.Rules.MarkingTheClose.Interfaces;
using Surveillance.Trades;
using Surveillance.Trades.Interfaces;
using Surveillance.Rules.Spoofing.Interfaces;
using Surveillance.Rules.Spoofing;
using Surveillance.Rules.WashTrade;
using Surveillance.Rules.WashTrade.Interfaces;
using Surveillance.Scheduler;
using Surveillance.Scheduler.Interfaces;
using Surveillance.Universe;
using Surveillance.Universe.Filter;
using Surveillance.Universe.Filter.Interfaces;
using Surveillance.Universe.Interfaces;
using Surveillance.Universe.MarketEvents;
using Surveillance.Universe.MarketEvents.Interfaces;
using Surveillance.Universe.Multiverse;
using Surveillance.Universe.Multiverse.Interfaces;
using Surveillance.Universe.OrganisationalFactors;
using Surveillance.Universe.OrganisationalFactors.Interfaces;
using Surveillance.Universe.Subscribers;
using Surveillance.Universe.Subscribers.Factories;
using Surveillance.Universe.Subscribers.Interfaces;
using Surveillance.Utility;
using Surveillance.Utility.Interfaces;

namespace Surveillance
{
    public class SurveillanceRegistry : Registry
    {
        public SurveillanceRegistry()
        {
            var loggerFactory = new NLogLoggerFactory();
            For(typeof(ILoggerFactory)).Use(loggerFactory);
            For(typeof(ILogger<>)).Use(typeof(Logger<>));

            For<IMediator>().Use<Mediator>();
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

            For<ICaseMessageBusSerialiser>().Use<CaseMessageBusSerialiser>();
            For<ICaseMessageSender>().Use<CaseMessageSender>();

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
            For<IReddeerDistributedRuleScheduler>().Use<ReddeerDistributedRuleScheduler>();
            For<IExchangeRates>().Use<ExchangeRates>();
            For<ITradePositionWeightedAverageExchangeRateCalculator>().Use<TradePositionWeightedAverageExchangeRateCalculator>();

            For<IApiHeartbeat>().Use<ApiHeartbeat>();
            For<IApplicationHeartbeatService>().Use<ApplicationHeartbeatService>();
            For<IRuleProjector>().Use<RuleProjector>();

            For<IEnrichmentService>().Use<EnrichmentService>();
            For<IUniversePercentageCompletionLogger>().Use<UniversePercentageCompletionLogger>();
            For<IUniversePercentageOfEventCompletionLogger>().Use<UniversePercentageOfEventCompletionLogger>();
            For<IUniverseMarketCacheFactory>().Use<UniverseMarketCacheFactory>();
            For<IMarketTradingHoursManager>().Use<MarketTradingHoursManager>();
            For<IUniversePercentageOfTimeCompletionLogger>().Use<UniversePercentageOfTimeCompletionLogger>();

            For<IUniversePercentageOfEventCompletionLoggerFactory>().Use<UniversePercentageOfEventCompletionLoggerFactory>();
            For<IUniversePercentageOfTimeCompletionLoggerFactory>().Use<UniversePercentageOfTimeCompletionLoggerFactory>();
            For<IUniversePercentageCompletionLoggerFactory>().Use<UniversePercentageCompletionLoggerFactory>();
            For<IUniverseOrderFilter>().Use<UniverseOrderFilter>();
        }
    }
}