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
        protected readonly IReddeerDirectory ReddeerDirectory;
        protected readonly ILogger Logger;
        private FileSystemWatcher _fileSystemWatcher;
        private readonly string _uploadFileMonitorName;

        protected BaseUploadFileMonitor(
            IReddeerDirectory directory,
            ILogger logger,
            string uploadFileMonitorName)
        {
            ReddeerDirectory = directory ?? throw new ArgumentNullException(nameof(directory));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _uploadFileMonitorName = uploadFileMonitorName ?? throw new ArgumentNullException(nameof(uploadFileMonitorName));
        }

        /// <summary>
        /// Initiate monitoring
        /// </summary>
        public void Initiate()
        {
            if (string.IsNullOrWhiteSpace(UploadDirectoryPath()))
            {
                Logger.Log(LogLevel.Information, $"{_uploadFileMonitorName} - no path in upload directory. Will not monitor unspecified upload folder path");
                return;
            }

            try
            {
                var archivePath = GetArchivePath();
                var failedReadsPath = GetFailedReadsPath();

                ReddeerDirectory.Create(UploadDirectoryPath());
                ReddeerDirectory.Create(archivePath);
                ReddeerDirectory.Create(failedReadsPath);

                var files = ReddeerDirectory.GetFiles(UploadDirectoryPath(), "*.csv");

                if (files.Any())
                {
                    ProcessInitialStartupFiles(archivePath, files);
                }
                SetFileSystemWatch();
            }
            catch (Exception e)
            {
                Logger.LogError($"Exception in {_uploadFileMonitorName} {UploadDirectoryPath()} {e.Message}");
            }
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
            try
            {
                var archivePath = ArchiveFilePath(GetArchivePath(), e.FullPath);
                ProcessFile(e.FullPath, archivePath);
            }
            catch (Exception a)
            {
                Logger.LogError("BaseUploadFileMonitor had an error in detected file change", a);
            }
        }

        private void ProcessInitialStartupFiles(string archivePath, IReadOnlyCollection<string> files)
        {
            try
            {
                Logger.LogInformation($"{_uploadFileMonitorName} found some existing files on start up. Processing now");

                foreach (var filePath in files)
                {
                    var archiveFilePath = ArchiveFilePath(archivePath, filePath);
                    ProcessFile(filePath, archiveFilePath);
                }
            }
            catch (Exception e)
            {
                Logger.LogError("Base upload file monitor had an error whilst process initial start up files", e);
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
            if (!ReddeerDirectory.DirectoryExists(UploadDirectoryPath()))
            {
                return;
            }

            _fileSystemWatcher = new FileSystemWatcher(UploadDirectoryPath())
            {
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.Size,
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
