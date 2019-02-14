using DataImport.Disk_IO.AllocationFile;
using DataImport.Disk_IO.AllocationFile.Interfaces;
using DataImport.Disk_IO.EquityFile;
using DataImport.Disk_IO.EquityFile.Interfaces;
using DataImport.Disk_IO.Interfaces;
using DataImport.Disk_IO.TradeFile;
using DataImport.Disk_IO.TradeFile.Interfaces;
using DataImport.Interfaces;
using DataImport.Managers;
using DataImport.Managers.Interfaces;
using DataImport.MessageBusIO;
using DataImport.MessageBusIO.Interfaces;
using DataImport.Recorders;
using DataImport.Recorders.Interfaces;
using DataImport.S3_IO;
using DataImport.S3_IO.Interfaces;
using DataImport.Services;
using DataImport.Services.Interfaces;
using DomainV2.Equity.Streams;
using DomainV2.Equity.Streams.Interfaces;
using DomainV2.Equity.TimeBars;
using DomainV2.Equity.TimeBars.Interfaces;
using DomainV2.Files;
using DomainV2.Files.AllocationFile;
using DomainV2.Files.AllocationFile.Interfaces;
using DomainV2.Files.Interfaces;
using DomainV2.Scheduling;
using DomainV2.Scheduling.Interfaces;
using DomainV2.Streams;
using DomainV2.Streams.Interfaces;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using StructureMap;
using Utilities.Aws_IO;
using Utilities.Aws_IO.Interfaces;
using Utilities.Disk_IO;
using Utilities.Disk_IO.Interfaces;

namespace DataImport
{
    public class DataImportRegistry : Registry
    {
        public DataImportRegistry()
        {
            var loggerFactory = new NLogLoggerFactory();
            For(typeof(ILoggerFactory)).Use(loggerFactory);
            For(typeof(ILogger<>)).Use(typeof(Logger<>));

            For<IMediator>().Use<Mediator>();

            For(typeof(IOrderAllocationStream<>)).Use(typeof(OrderAllocationStream<>));
            For(typeof(IOrderStream<>)).Use(typeof(OrderStream<>));
            For<IStockExchangeStream>().Use<ExchangeStream>();

            For<IStockExchangeStreamManager>().Use<StockExchangeStreamManager>();
            For(typeof(IUnsubscriberFactory<>)).Use(typeof(UnsubscriberFactory<>));

            For<IReddeerDirectory>().Use<ReddeerDirectory>();

            For<IUploadTradeFileProcessor>().Use<UploadTradeFileProcessor>();
            For<IUploadTradeFileMonitor>().Use<UploadTradeFileMonitor>();

            For<IUploadEquityFileProcessor>().Use<UploadEquityFileProcessor>();
            For<ISecurityCsvToDtoMapper>().Use<SecurityCsvToDtoMapper>();
            For<IUploadEquityFileMonitor>().Use<UploadEquityFileMonitor>();
            For<IUploadEquityFileMonitorFactory>().Use<UploadEquityFileMonitorFactory>();

            For<IS3FileUploadMonitoringProcess>().Use<S3FileUploadMonitoringProcess>();
            For<IAwsQueueClient>().Use<AwsQueueClient>();
            For<IFileUploadMessageMapper>().Use<FileUploadMessageMapper>();
            For<IAwsS3Client>().Use<AwsS3Client>();

            For<IRedDeerAuroraStockExchangeRecorder>().Use<RedDeerAuroraStockExchangeRecorder>();
            For<IScheduleRuleMessageSender>().Use<ScheduleRuleMessageSender>();
            For<IScheduledExecutionMessageBusSerialiser>().Use<ScheduledExecutionMessageBusSerialiser>();
            For<IScheduleExecutionDtoMapper>().Use<ScheduleExecutionDtoMapper>();

            For<ITradeFileCsvValidator>().Use<TradeFileCsvValidator>();
            For<ITradeFileCsvToOrderMapper>().Use<TradeFileCsvToOrderMapper>();

            For<IEnrichmentService>().Use<EnrichmentService>();

            For<IUploadAllocationFileMonitor>().Use<AllocationFileMonitor>();
            For<IAllocationFileCsvValidator>().Use<AllocationFileCsvValidator>();
            For<IAllocationFileCsvToOrderAllocationMapper>().Use<AllocationFileCsvToOrderAllocationMapper>();
            For<IAllocationFileProcessor>().Use<AllocationFileProcessor>();
        }
    }
}