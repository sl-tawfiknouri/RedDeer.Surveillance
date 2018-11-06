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

            Logger.Log(LogLevel.Information, $"{_uploadFileMonitorName} Initiating monitoring process.");

            try
            {
                var archivePath = GetArchivePath();
                var failedReadsPath = GetFailedReadsPath();

                Logger.Log(LogLevel.Information, $"{_uploadFileMonitorName} Creating reddeer directory folders");
                ReddeerDirectory.Create(UploadDirectoryPath());
                ReddeerDirectory.Create(archivePath);
                ReddeerDirectory.Create(failedReadsPath);
                Logger.Log(LogLevel.Information, $"{_uploadFileMonitorName} Completed creating reddeer directory folders");

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
                Logger.LogInformation($"BaseUploadFileMonitor detected a file change at {e.FullPath}.");
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

                Logger.LogInformation($"{_uploadFileMonitorName} has completed processing the initial start up files");
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
            Logger.LogInformation("BaseUploadFileMonitor setting file system watch");

            if (!ReddeerDirectory.DirectoryExists(UploadDirectoryPath()))
            {
                Logger.LogInformation($"BaseUploadFileMonitor did not find the {UploadDirectoryPath()} not setting file watch.");

                return;
            }

            if (_fileSystemWatcher != null)
            {
                Logger.LogInformation("BaseUploadFileMonitor disposing an old file system watcher.");

                _fileSystemWatcher.Dispose();
                _fileSystemWatcher = null;
            }

            _fileSystemWatcher = new FileSystemWatcher(UploadDirectoryPath())
            {
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.Size,
                Filter = "*.csv",
                IncludeSubdirectories = false
            };

            _fileSystemWatcher.Error += OnError;
            _fileSystemWatcher.Changed += DetectedFileChange;
            _fileSystemWatcher.Renamed += DetectedFileChange;
            _fileSystemWatcher.Created += DetectedFileChange;

            _fileSystemWatcher.EnableRaisingEvents = true;
            Logger.LogInformation("BaseUploadFileMonitor set file system watch events and now enabled raising events.");
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            Logger.LogError("BaseUploadFileMonitor encountered an exception! RESTART RELAY SERVICE", e.GetException());
        }

        public void Dispose()
        {
            Logger.LogInformation("BaseUploadFileMonitor called dispose on file monitor.");
            _fileSystemWatcher?.Dispose();
        }
    }
}
