using System;
using System.IO;
using System.Linq;
using Domain.Trades.Orders;
using Domain.Trades.Streams.Interfaces;
using Microsoft.Extensions.Logging;
using Relay.Configuration.Interfaces;
using Relay.Disk_IO.Interfaces;
using Utilities.Disk_IO.Interfaces;

namespace Relay.Disk_IO
{
    public class UploadTradeFileMonitor : IUploadTradeFileMonitor
    {
        private readonly ITradeOrderStream<TradeOrderFrame> _stream;
        private readonly IUploadConfiguration _uploadConfiguration;
        private readonly IReddeerDirectory _reddeerDirectory;
        private readonly IUploadTradeFileProcessor _fileProcessor;
        private readonly ILogger _logger;
        private readonly object _lock = new object();

        public UploadTradeFileMonitor(
            ITradeOrderStream<TradeOrderFrame> stream,
            IUploadConfiguration uploadConfiguration,
            IReddeerDirectory directory,
            IUploadTradeFileProcessor fileProcessor,
            ILogger<UploadTradeFileMonitor> logger)
        {
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
            _uploadConfiguration = uploadConfiguration ?? throw new ArgumentNullException(nameof(uploadConfiguration));
            _reddeerDirectory = directory ?? throw new ArgumentNullException(nameof(directory));
            _fileProcessor = fileProcessor ?? throw new ArgumentNullException(nameof(fileProcessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Initiate monitoring
        /// </summary>
        public void Initiate()
        {
            if (string.IsNullOrWhiteSpace(_uploadConfiguration.RelayTradeFileUploadDirectoryPath))
            {
                _logger.Log(LogLevel.Information, "UploadTradeFileMonitor - no path in upload directory. Will not monitor unspecified upload folder path");
                return;
            }

            var archivePath = Path.Combine(_uploadConfiguration.RelayTradeFileUploadDirectoryPath, "Archive");

            try
            {
                _reddeerDirectory.Create(_uploadConfiguration.RelayTradeFileUploadDirectoryPath);
                _reddeerDirectory.Create(archivePath);
            }
            catch (ArgumentException e)
            {
                _logger.LogError($"Argument exception in upload trade file monitor {_uploadConfiguration.RelayTradeFileUploadDirectoryPath} {e.Message}");
            }

            var files = _reddeerDirectory.GetFiles(_uploadConfiguration.RelayTradeFileUploadDirectoryPath, "*.csv");

            if (files.Any())
            {
                _logger.LogInformation("Upload Trade File Monitor found some existing files on start up. Processing now");

                foreach (var filePath in files)
                {
                    var fileName = Path.GetFileName(filePath);
                    if (string.IsNullOrWhiteSpace(fileName))
                    {
                        continue;
                    }

                    fileName = "archived_" + fileName;
                    var archiveFilePath = Path.Combine(archivePath, fileName);
                    ProcessFile(filePath, archiveFilePath);
                }
            }
        }

        private void ProcessFile(string path, string archivePath)
        {
            lock (_lock)
            {
                var csvRecords = _fileProcessor.Process(path);

                if (csvRecords == null)
                {
                    return;
                }

                foreach (var item in csvRecords)
                {
                    _stream.Add(item);
                }

                _reddeerDirectory.Move(path, archivePath);
            }
        }

        public void Dispose()
        {
        }
    }
}