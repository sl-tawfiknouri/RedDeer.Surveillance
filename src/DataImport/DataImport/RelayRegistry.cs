using DataImport.Disk_IO.EquityFile;
using DataImport.Disk_IO.EquityFile.Interfaces;
using DataImport.Disk_IO.Interfaces;
using DataImport.Disk_IO.TradeFile;
using DataImport.Disk_IO.TradeFile.Interfaces;
using DataImport.Interfaces;
using DataImport.Managers;
using DataImport.Managers.Interfaces;
using DataImport.Network_IO;
using DataImport.Network_IO.Interfaces;
using DataImport.Network_IO.RelaySubscribers;
using DataImport.Network_IO.RelaySubscribers.Interfaces;
using DataImport.Processors;
using DataImport.Processors.Interfaces;
using DataImport.S3_IO;
using DataImport.S3_IO.Interfaces;
using Domain.Equity.Frames;
using Domain.Equity.Frames.Interfaces;
using Domain.Equity.Streams;
using Domain.Equity.Streams.Interfaces;
using Domain.Streams;
using Domain.Trades.Orders;
using Domain.Trades.Orders.Interfaces;
using Domain.Trades.Streams;
using Domain.Trades.Streams.Interfaces;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using StructureMap;
using Utilities.Aws_IO;
using Utilities.Aws_IO.Interfaces;
using Utilities.Disk_IO;
using Utilities.Disk_IO.Interfaces;
using Utilities.Network_IO.Interfaces;
using Utilities.Network_IO.Websocket_Connections;
using Utilities.Network_IO.Websocket_Connections.Interfaces;
using Utilities.Network_IO.Websocket_Hosts;
using Utilities.Network_IO.Websocket_Hosts.Interfaces;

namespace DataImport
{
    public class RelayRegistry : Registry
    {
        public RelayRegistry()
        {
            var loggerFactory = new NLogLoggerFactory();
            For(typeof(ILoggerFactory)).Use(loggerFactory);
            For(typeof(ILogger<>)).Use(typeof(Logger<>));

            For<IMediator>().Use<Mediator>();

            For(typeof(ITradeProcessor<>)).Use(typeof(TradeProcessor<>));
            For<IEquityProcessor<ExchangeFrame>>().Use<EquityProcessor>();

            For(typeof(ITradeOrderStream<>)).Use(typeof(TradeOrderStream<>));
            For<IStockExchangeStream>().Use<StockExchangeStream>();

            For<ITradeRelaySubscriber>().Use<TradeRelaySubscriber>();
            For<IEquityRelaySubscriber>().Use<EquityRelaySubscriber>();

            For<IWebsocketConnectionFactory>().Use<WebsocketConnectionFactory>();
            For<IMessageWriter>().Use<LoggingMessageWriter>();

            For<INetworkTrunk>().Use<NetworkTrunk>();
            For<INetworkFailOver>().Use<NetworkFailOverLocalMemory>();
            For<INetworkSwitch>().Use<NetworkSwitch>();

            For<INetworkExchange>().Use<NetworkExchange>();
            For<IRelayTradeNetworkDuplexer>().Use<RelayTradeNetworkDuplexer>();
            For<IRelayEquityNetworkDuplexer>().Use<RelayEquityNetworkDuplexer>();

            For<ITradeOrderStreamManager>().Use<TradeOrderStreamManager>();
            For<IStockExchangeStreamManager>().Use<StockExchangeStreamManager>();

            For<IDuplexMessageFactory>().Use<DuplexMessageFactory>();
            For<IWebsocketHostFactory>().Use<WebsocketHostFactory>();
            For<IWebsocketHost>().Use<RedDeerWebsocketHost>();
            For(typeof(IUnsubscriberFactory<>)).Use(typeof(UnsubscriberFactory<>));

            For<IReddeerDirectory>().Use<ReddeerDirectory>();

            For<IUploadTradeFileProcessor>().Use<UploadTradeFileProcessor>();
            For<ITradeOrderCsvToDtoMapper>().Use<TradeOrderCsvToDtoMapper>();
            For<IUploadTradeFileMonitor>().Use<UploadTradeFileMonitor>();
            For<IUploadTradeFileMonitorFactory>().Use<UploadTradeFileMonitorFactory>();

            For<IUploadEquityFileProcessor>().Use<UploadEquityFileProcessor>();
            For<ISecurityCsvToDtoMapper>().Use<SecurityCsvToDtoMapper>();
            For<IUploadEquityFileMonitor>().Use<UploadEquityFileMonitor>();
            For<IUploadEquityFileMonitorFactory>().Use<UploadEquityFileMonitorFactory>();

            For<IS3FileUploadMonitoringProcess>().Use<S3FileUploadMonitoringProcess>();
            For<IAwsQueueClient>().Use<AwsQueueClient>();
            For<IFileUploadMessageMapper>().Use<FileUploadMessageMapper>();
            For<IAwsS3Client>().Use<AwsS3Client>();
        }
    }
}