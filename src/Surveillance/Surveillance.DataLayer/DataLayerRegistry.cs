namespace Surveillance.DataLayer
{
    using Domain.Core.Financial.Cfis;
    using Domain.Core.Financial.Cfis.Interfaces;

    using Infrastructure.Network.Aws;
    using Infrastructure.Network.Aws.Interfaces;
    using Infrastructure.Network.HttpClient;
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
    using Surveillance.DataLayer.Aurora.Judgements;
    using Surveillance.DataLayer.Aurora.Judgements.Interfaces;
    using Surveillance.DataLayer.Aurora.Market;
    using Surveillance.DataLayer.Aurora.Market.Interfaces;
    using Surveillance.DataLayer.Aurora.Orders;
    using Surveillance.DataLayer.Aurora.Orders.Interfaces;
    using Surveillance.DataLayer.Aurora.Rules;
    using Surveillance.DataLayer.Aurora.Rules.Interfaces;
    using Surveillance.DataLayer.Aurora.Scheduler;
    using Surveillance.DataLayer.Aurora.Scheduler.Interfaces;
    using Surveillance.DataLayer.Aurora.Tuning;
    using Surveillance.DataLayer.Aurora.Tuning.Interfaces;

    public class DataLayerRegistry : Registry
    {
        public DataLayerRegistry()
        {
            this.For<IAwsSnsClient>().Use<AwsSnsClient>();
            this.For<IAwsQueueClient>().Use<AwsQueueClient>();
            this.For<IConnectionStringFactory>().Use<ConnectionStringFactory>();
            this.For<IOrdersRepository>().Use<OrdersRepository>().Singleton();
            this.For<IReddeerMarketRepository>().Use<ReddeerMarketRepository>().Singleton();
            this.For<IRuleAnalyticsUniverseRepository>().Use<RuleAnalyticsUniverseRepository>();
            this.For<IRuleAnalyticsAlertsRepository>().Use<RuleAnalyticsAlertsRepository>();
            this.For<ICfiInstrumentTypeMapper>().Use<CfiInstrumentTypeMapper>();
            this.For<IRuleRunDataRequestRepository>().Use<RuleRunDataRequestRepository>();
            this.For<IStubRuleRunDataRequestRepository>().Use<StubRuleRunDataRequestRepository>();
            this.For<IReddeerMarketDailySummaryRepository>().Use<ReddeerMarketDailySummaryRepository>();
            this.For<IReddeerMarketTimeBarRepository>().Use<ReddeerMarketTimeBarRepository>();
            this.For<IOrderAllocationRepository>().Use<OrderAllocationRepository>();
            this.For<IRuleBreachRepository>().Use<RuleBreachRepository>();
            this.For<IRuleBreachOrdersRepository>().Use<RuleBreachOrdersRepository>();
            this.For<IFileUploadOrdersRepository>().Use<FileUploadOrdersRepository>();
            this.For<IFileUploadOrderAllocationRepository>().Use<FileUploadOrderAllocationRepository>();
            this.For<ITaskSchedulerRepository>().Use<TaskSchedulerRepository>();
            this.For<IPolicyFactory>().Use<PolicyFactory>();
            this.For<IHttpClientFactory>().Use<HttpClientFactory>();
            this.For<ITuningRepository>().Use<TuningRepository>();
            this.For<IOrderBrokerRepository>().Use<OrderBrokerRepository>();
            this.For<IJudgementRepository>().Use<JudgementRepository>();
        }
    }
}