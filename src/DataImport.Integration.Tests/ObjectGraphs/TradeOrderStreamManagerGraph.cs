using DataImport.Configuration.Interfaces;
using DataImport.Disk_IO.TradeFile;
using DataImport.Managers;
using DataImport.MessageBusIO.Interfaces;
using DataImport.Recorders;
using DataImport.Services.Interfaces;
using DomainV2.Files;
using DomainV2.Streams;
using DomainV2.Trading;
using FakeItEasy;
using Microsoft.Extensions.Logging.Abstractions;
using Surveillance.DataLayer.Aurora.Trade.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
using Utilities.Disk_IO;

namespace DataImport.Integration.Tests.ObjectGraphs
{
    public class TradeOrderStreamManagerGraph
    {
        public IOrdersRepository OrdersRepository { get; private set; }

        public TradeOrderStreamManager Build()
        {
            var uploadTradeFileMonitor =
                new TradeOrderStreamManager(
                    OrderStream(),
                    UploadTradeFileMonitorFactory(),
                    TradeRecorder());

            return uploadTradeFileMonitor;
        }

        private OrderStream<Order> OrderStream()
        {
            return new OrderStream<Order>(new UnsubscriberFactory<Order>());
        }

        private UploadTradeFileMonitorFactory UploadTradeFileMonitorFactory()
        {
            var ctx = A.Fake<ISystemProcessContext>();

            return new UploadTradeFileMonitorFactory(
                new Configuration.Configuration(),
                new ReddeerDirectory(),
                new UploadTradeFileProcessor(
                    new TradeFileCsvToOrderMapper(),
                    new TradeFileCsvValidator(),
                    new NullLogger<UploadTradeFileProcessor>()),
                ctx,
                new NullLogger<UploadTradeFileMonitor>());
        }

        private RedDeerAuroraTradeRecorderAutoSchedule TradeRecorder()
        {
            var enrichmentService = A.Fake<IEnrichmentService>();
            var ordersRepository = A.Fake<IOrdersRepository>();
            var messageSender = A.Fake<IScheduleRuleMessageSender>();
            var uploadConfiguration = A.Fake<IUploadConfiguration>();

            OrdersRepository = ordersRepository;

            return new RedDeerAuroraTradeRecorderAutoSchedule(
                enrichmentService,
                ordersRepository,
                messageSender,
                uploadConfiguration,
                new NullLogger<RedDeerAuroraTradeRecorderAutoSchedule>());
        }
    }
}
