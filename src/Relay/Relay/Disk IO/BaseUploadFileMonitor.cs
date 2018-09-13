using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Relay.Disk_IO.Interfaces;
using Utilities.Disk_IO.Interfaces;

namespace Relay.Disk_IO
{
    public abstract class BaseUploadFileMonitor : IBaseUploadFileMonitor
    {
        protected readonly IReddeerDirectory _reddeerDirectory;
        protected readonly ILogger _logger;
        private FileSystemWatcher _fileSystemWatcher;
        private readonly string _uploadFileMonitorName;

        protected BaseUploadFileMonitor(
            IReddeerDirectory directory,
            ILogger logger,
            string uploadFileMonitorName)
        {
            _reddeerDirectory = directory ?? throw new ArgumentNullException(nameof(directory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _uploadFileMonitorName = uploadFileMonitorName ?? throw new ArgumentNullException(nameof(uploadFileMonitorName));
        }

        /// <summary>
        /// Initiate monitoring
        /// </summary>
        public void Initiate()
        {
            if (string.IsNullOrWhiteSpace(UploadDirectoryPath()))
            {
                _logger.Log(LogLevel.Information, $"{_uploadFileMonitorName} - no path in upload directory. Will not monitor unspecified upload folder path");
                return;
            }

            var archivePath = GetArchivePath();
            var failedReadsPath = GetFailedReadsPath();

            try
            {
                _reddeerDirectory.Create(UploadDirectoryPath());
                _reddeerDirectory.Create(archivePath);
                _reddeerDirectory.Create(failedReadsPath);
            }
            catch (ArgumentException e)
            {
                _logger.LogError($"Argument exception in {_uploadFileMonitorName} {UploadDirectoryPath()} {e.Message}");
            }

            var files = _reddeerDirectory.GetFiles(UploadDirectoryPath(), "*.csv");

            if (files.Any())
            {
                ProcessInitialStartupFiles(archivePath, files);
            }
            SetFileSystemWatch();
        }

        protected abstract string UploadDirectoryPath();

        protected string GetArchivePath()
        {
            return Path.Combine(UploadDirectoryPath(), "Archive");
        }

        protected string GetFailedReadsPath()
        {
            return Path.Combine(UploadDirectoryPath(), "FailedReads");
        }

        private void DetectedFileChange(object source, FileSystemEventArgs e)
        {
            var archivePath = ArchiveFilePath(GetArchivePath(), e.FullPath);
            ProcessFile(e.FullPath, archivePath);
        }

        private void ProcessInitialStartupFiles(string archivePath, IReadOnlyCollection<string> files)
        {
            _logger.LogInformation($"{_uploadFileMonitorName} found some existing files on start up. Processing now");

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

        protected abstract void ProcessFile(string path, string archivePath);

        private void SetFileSystemWatch()
        {
            if (!_reddeerDirectory.DirectoryExists(UploadDirectoryPath()))
            {
                return;
            }

            _fileSystemWatcher = new FileSystemWatcher(UploadDirectoryPath())
            {
                NotifyFilter = NotifyFilters.LastWrite,
                Filter = "*.csv",
                IncludeSubdirectories = false
            };

            _fileSystemWatcher.Changed += DetectedFileChange;
            _fileSystemWatcher.EnableRaisingEvents = true;
        }

        public void Dispose()
        {
            _fileSystemWatcher?.Dispose();
        }
    }
}
