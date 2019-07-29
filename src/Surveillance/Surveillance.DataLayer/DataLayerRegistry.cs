using Domain.Core.Financial.Cfis;
using Domain.Core.Financial.Cfis.Interfaces;
using Infrastructure.Network.Aws;
using Infrastructure.Network.Aws.Interfaces;
using Infrastructure.Network.HttpClient.Interfaces;
using PollyFacade.Policies;
using PollyFacade.Policies.Interfaces;
using StructureMap;
using Surveillance.DataLayer.Aurora;
using Surveillance.DataLayer.Aurora.Analytics;
using Surveillance.DataLayer.Aurora.Analytics.Interfaces;
using Surveillance.DataLayer.Aurora.BMLL;
using Surveillance.DataLayer.Aurora.BMLL.Interfaces;
using Surveillance.DataLayer.Aurora.Files;
using Surveillance.DataLayer.Aurora.Files.Interfaces;
using Surveillance.DataLayer.Aurora.Interfaces;
using Surveillance.DataLayer.Aurora.Market;
using Surveillance.DataLayer.Aurora.Market.Interfaces;
using Surveillance.DataLayer.Aurora.Orders;
using Surveillance.DataLayer.Aurora.Orders.Interfaces;
using Surveillance.DataLayer.Aurora.Rules;
using Surveillance.DataLayer.Aurora.Rules.Interfaces;
using Infrastructure.Network.HttpClient;
using Surveillance.DataLayer.Aurora.Scheduler;
using Surveillance.DataLayer.Aurora.Scheduler.Interfaces;
using Surveillance.DataLayer.Aurora.Tuning;
using Surveillance.DataLayer.Aurora.Tuning.Interfaces;
using Surveillance.DataLayer.Aurora.Judgements.Interfaces;
using Surveillance.DataLayer.Aurora.Judgements;

namespace Surveillance.DataLayer
{
    public class DataLayerRegistry : Registry
    {
        public DataLayerRegistry()
        {
            For<IAwsSnsClient>().Use<AwsSnsClient>();
            For<IAwsQueueClient>().Use<AwsQueueClient>();
            For<IConnectionStringFactory>().Use<ConnectionStringFactory>();
            For<IOrdersRepository>().Use<OrdersRepository>().Singleton();
            For<IReddeerMarketRepository>().Use<ReddeerMarketRepository>().Singleton();
            For<IRuleAnalyticsUniverseRepository>().Use<RuleAnalyticsUniverseRepository>();
            For<IRuleAnalyticsAlertsRepository>().Use<RuleAnalyticsAlertsRepository>();
            For<ICfiInstrumentTypeMapper>().Use<CfiInstrumentTypeMapper>();
            For<IRuleRunDataRequestRepository>().Use<RuleRunDataRequestRepository>();
            For<IStubRuleRunDataRequestRepository>().Use<StubRuleRunDataRequestRepository>();
            For<IReddeerMarketDailySummaryRepository>().Use<ReddeerMarketDailySummaryRepository>();
            For<IReddeerMarketTimeBarRepository>().Use<ReddeerMarketTimeBarRepository>();
            For<IOrderAllocationRepository>().Use<OrderAllocationRepository>();
            For<IRuleBreachRepository>().Use<RuleBreachRepository>();
            For<IRuleBreachOrdersRepository>().Use<RuleBreachOrdersRepository>();
            For<IFileUploadOrdersRepository>().Use<FileUploadOrdersRepository>();
            For<IFileUploadOrderAllocationRepository>().Use<FileUploadOrderAllocationRepository>();
            For<ITaskSchedulerRepository>().Use<TaskSchedulerRepository>();
            For<IPolicyFactory>().Use<PolicyFactory>();
            For<IHttpClientFactory>().Use<HttpClientFactory>();
            For<ITuningRepository>().Use<TuningRepository>();
            For<IOrderBrokerRepository>().Use<OrderBrokerRepository>();
            For<IJudgementRepository>().Use<JudgementRepository>();
        }
    }
}
