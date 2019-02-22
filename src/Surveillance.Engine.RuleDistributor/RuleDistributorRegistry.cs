﻿using Domain.Scheduling;
using Domain.Scheduling.Interfaces;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using StructureMap;
using Surveillance.Engine.RuleDistributor.Distributor;
using Surveillance.Engine.RuleDistributor.Distributor.Interfaces;
using Surveillance.Engine.RuleDistributor.Interfaces;
using Surveillance.Engine.RuleDistributor.Queues;
using Surveillance.Engine.RuleDistributor.Queues.Interfaces;

namespace Surveillance.Engine.RuleDistributor
{
    public class RuleDistributorRegistry : Registry
    {
        public RuleDistributorRegistry()
        {
            var loggerFactory = new NLogLoggerFactory();
            For(typeof(ILoggerFactory)).Use(loggerFactory);
            For(typeof(ILogger<>)).Use(typeof(Logger<>));

            For<IRuleDistributorMediator>().Use<Mediator>();
            For<IQueueDistributedRuleSubscriber>().Use<QueueDistributedRuleSubscriber>();

            For<IScheduledExecutionMessageBusSerialiser>().Use<ScheduledExecutionMessageBusSerialiser>();
            For<IScheduleExecutionDtoMapper>().Use<ScheduleExecutionDtoMapper>();
            For<IScheduleDisassembler>().Use<ScheduleDisassembler>();
            For<IQueueDistributedRulePublisher>().Use<QueueDistributedRulePublisher>();
        }
    }
}