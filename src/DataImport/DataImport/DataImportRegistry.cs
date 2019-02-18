using Contracts.SurveillanceService;
using Contracts.SurveillanceService.Interfaces;
using DataImport.Disk_IO.AllocationFile;
using DataImport.Disk_IO.AllocationFile.Interfaces;
using DataImport.Disk_IO.Interfaces;
using DataImport.Disk_IO.TradeFile;
using DataImport.Disk_IO.TradeFile.Interfaces;
using DataImport.File_Scanner;
using DataImport.File_Scanner.Interfaces;
using DataImport.Interfaces;
using DataImport.MessageBusIO;
using DataImport.MessageBusIO.Interfaces;
using DataImport.S3_IO;
using DataImport.S3_IO.Interfaces;
using DataImport.Services;
using DataImport.Services.Interfaces;
using Domain.Equity.Streams;
using Domain.Equity.Streams.Interfaces;
using Domain.Equity.TimeBars;
using Domain.Equity.TimeBars.Interfaces;
using Domain.Files;
using Domain.Files.AllocationFile;
using Domain.Files.AllocationFile.Interfaces;
using Domain.Files.Interfaces;
using Domain.Scheduling;
using Domain.Scheduling.Interfaces;
using Domain.Streams;
using Domain.Streams.Interfaces;
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

            For(typeof(IUnsubscriberFactory<>)).Use(typeof(UnsubscriberFactory<>));

            For<IReddeerDirectory>().Use<ReddeerDirectory>();
            For<IUploadTradeFileProcessor>().Use<UploadTradeFileProcessor>();
            For<IUploadTradeFileMonitor>().Use<UploadTradeFileMonitor>();
            For<ISecurityCsvToDtoMapper>().Use<SecurityCsvToDtoMapper>();

            For<IS3FileUploadMonitoringProcess>().Use<S3FileUploadMonitoringProcess>();
            For<IAwsQueueClient>().Use<AwsQueueClient>();
            For<IFileUploadMessageMapper>().Use<FileUploadMessageMapper>();
            For<IAwsS3Client>().Use<AwsS3Client>();

            For<IScheduledExecutionMessageBusSerialiser>().Use<ScheduledExecutionMessageBusSerialiser>();
            For<IScheduleExecutionDtoMapper>().Use<ScheduleExecutionDtoMapper>();

            For<ITradeFileCsvValidator>().Use<TradeFileCsvValidator>();
            For<ITradeFileCsvToOrderMapper>().Use<TradeFileCsvToOrderMapper>();

            For<IEnrichmentService>().Use<EnrichmentService>();

            For<IUploadAllocationFileMonitor>().Use<AllocationFileMonitor>();
            For<IAllocationFileCsvValidator>().Use<AllocationFileCsvValidator>();
            For<IAllocationFileCsvToOrderAllocationMapper>().Use<AllocationFileCsvToOrderAllocationMapper>();
            For<IAllocationFileProcessor>().Use<AllocationFileProcessor>();
            For<IUploadCoordinatorMessageSender>().Use<UploadCoordinatorMessageSender>();
            For<IMessageBusSerialiser>().Use<MessageBusSerialiser>();

            For<IFileScanner>().Use<FileScanner>();
            For<IFileScannerScheduler>().Use<FileScannerScheduler>();
        }
    }
}