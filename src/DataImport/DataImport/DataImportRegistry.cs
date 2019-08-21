namespace DataImport
{
    using DataImport.Disk_IO.AllocationFile;
    using DataImport.Disk_IO.AllocationFile.Interfaces;
    using DataImport.Disk_IO.EtlFile;
    using DataImport.Disk_IO.EtlFile.Interfaces;
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

    public class DataImportRegistry : Registry
    {
        public DataImportRegistry()
        {
            var loggerFactory = new NLogLoggerFactory();
            this.For(typeof(ILoggerFactory)).Use(loggerFactory);
            this.For(typeof(ILogger<>)).Use(typeof(Logger<>));

            this.For<IMediator>().Use<Mediator>();

            this.For(typeof(IOrderAllocationStream<>)).Use(typeof(OrderAllocationStream<>));
            this.For(typeof(IOrderStream<>)).Use(typeof(OrderStream<>));
            this.For<IStockExchangeStream>().Use<ExchangeStream>();

            this.For(typeof(IUnsubscriberFactory<>)).Use(typeof(UnsubscriberFactory<>));

            this.For<IReddeerDirectory>().Use<ReddeerDirectory>();
            this.For<IUploadTradeFileProcessor>().Use<UploadTradeFileProcessor>();
            this.For<IUploadTradeFileMonitor>().Use<UploadTradeFileMonitor>();
            this.For<ISecurityCsvToDtoMapper>().Use<SecurityCsvToDtoMapper>();

            this.For<IS3FileUploadMonitoringProcess>().Use<S3FileUploadMonitoringProcess>();
            this.For<IAwsQueueClient>().Use<AwsQueueClient>();
            this.For<IFileUploadMessageMapper>().Use<FileUploadMessageMapper>();
            this.For<IAwsS3Client>().Use<AwsS3Client>();

            this.For<IScheduledExecutionMessageBusSerialiser>().Use<ScheduledExecutionMessageBusSerialiser>();
            this.For<IScheduleExecutionDtoMapper>().Use<ScheduleExecutionDtoMapper>();

            this.For<IOrderFileValidator>().Use<OrderFileValidator>();
            this.For<IOrderFileToOrderSerialiser>().Use<OrderFileToOrderSerialiser>();

            this.For<IEnrichmentService>().Use<EnrichmentService>();

            this.For<IUploadAllocationFileMonitor>().Use<AllocationFileMonitor>();
            this.For<IAllocationFileValidator>().Use<AllocationFileValidator>();
            this.For<IAllocationFileCsvToOrderAllocationSerialiser>()
                .Use<AllocationFileCsvToOrderAllocationSerialiser>();
            this.For<IAllocationFileProcessor>().Use<AllocationFileProcessor>();
            this.For<IUploadCoordinatorMessageSender>().Use<UploadCoordinatorMessageSender>();
            this.For<IEmailNotificationMessageSender>().Use<EmailNotificationMessageSender>();
            this.For<IMessageBusSerialiser>().Use<MessageBusSerialiser>();
            this.For<Contracts.Email.IMessageBusSerialiser>().Use<Contracts.Email.MessageBusSerialiser>();

            this.For<IUploadEtlFileMonitor>().Use<UploadEtlFileMonitor>();
            this.For<IUploadEtlFileProcessor>().Use<UploadEtlFileProcessor>();
            this.For<IEtlFileValidator>().Use<EtlFileValidator>();
            this.For<IEtlUploadErrorStore>().Use<EtlUploadErrorStore>();

            this.For<IFileScanner>().Use<FileScanner>();
            this.For<IFileScannerScheduler>().Use<FileScannerScheduler>();

            this.For<IOmsOrderFieldCompression>().Use<OmsOrderFieldCompression>();
            this.For<IOmsVersioner>().Use<OmsVersioner>();
        }
    }
}