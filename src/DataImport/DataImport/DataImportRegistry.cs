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
using Domain.Surveillance.Scheduling;
using Domain.Surveillance.Scheduling.Interfaces;
using Domain.Surveillance.Streams;
using Domain.Surveillance.Streams.Interfaces;
using Infrastructure.Network.Aws;
using Infrastructure.Network.Aws.Interfaces;
using Infrastructure.Network.Disk;
using Infrastructure.Network.Disk.Interfaces;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using RedDeer.Contracts.SurveillanceService;
using RedDeer.Contracts.SurveillanceService.Interfaces;
using SharedKernel.Files.Allocations;
using SharedKernel.Files.Allocations.Interfaces;
using SharedKernel.Files.Orders;
using SharedKernel.Files.Orders.Interfaces;
using SharedKernel.Files.Security;
using SharedKernel.Files.Security.Interfaces;
using StructureMap;

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

            For<IOrderFileValidator>().Use<OrderFileValidator>();
            For<IOrderFileToOrderSerialiser>().Use<OrderFileToOrderSerialiser>();

            For<IEnrichmentService>().Use<EnrichmentService>();

            For<IUploadAllocationFileMonitor>().Use<AllocationFileMonitor>();
            For<IAllocationFileValidator>().Use<AllocationFileValidator>();
            For<IAllocationFileCsvToOrderAllocationSerialiser>().Use<AllocationFileCsvToOrderAllocationSerialiser>();
            For<IAllocationFileProcessor>().Use<AllocationFileProcessor>();
            For<IUploadCoordinatorMessageSender>().Use<UploadCoordinatorMessageSender>();
            For<IMessageBusSerialiser>().Use<MessageBusSerialiser>();

            For<IFileScanner>().Use<FileScanner>();
            For<IFileScannerScheduler>().Use<FileScannerScheduler>();
        }
    }
}