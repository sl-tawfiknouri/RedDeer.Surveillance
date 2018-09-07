using System;
using System.Collections.Generic;
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

            var archivePath = GetArchivePath();

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
                ProcessInitialStartupFiles(archivePath, files);
            }

            SetFileSystemWatch();
        }

        private string GetArchivePath()
        {
            return Path.Combine(_uploadConfiguration.RelayTradeFileUploadDirectoryPath, "Archive");
        }

        private void DetectedFileChange(object source, FileSystemEventArgs e)
        {
            var archivePath = ArchiveFilePath(GetArchivePath(), e.FullPath);
            ProcessFile(e.FullPath, archivePath);
        }

        private void ProcessInitialStartupFiles(string archivePath, IReadOnlyCollection<string> files)
        {
            _logger.LogInformation("Upload Trade File Monitor found some existing files on start up. Processing now");

            foreach (var filePath in files)
            {
                var archiveFilePath = ArchiveFilePath(archivePath, filePath);
                ProcessFile(filePath, archiveFilePath);
            }
        }

        private string ArchiveFilePath(string archivePath, string filePath)
        {
            var fileName = Path.GetFileName(filePath);
            fileName = "archived_" + fileName;
            var archiveFilePath = Path.Combine(archivePath, fileName);

            return archiveFilePath;
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

        private void SetFileSystemWatch()
        {
            var fileSystemWatcher = new FileSystemWatcher(_uploadConfiguration.RelayTradeFileUploadDirectoryPath)
            {
                NotifyFilter = NotifyFilters.LastWrite,
                Filter = "*.csv"
            };

            fileSystemWatcher.Changed += DetectedFileChange;
            fileSystemWatcher.EnableRaisingEvents = true;
        }

        public void Dispose()
        {
        }
    }
}