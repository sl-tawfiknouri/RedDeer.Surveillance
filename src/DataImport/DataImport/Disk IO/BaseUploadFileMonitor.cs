using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DataImport.Disk_IO.Interfaces;
using Infrastructure.Network.Disk.Interfaces;
using Microsoft.Extensions.Logging;

namespace DataImport.Disk_IO
{
    public abstract class BaseUploadFileMonitor : IBaseUploadFileMonitor
    {
        protected readonly IReddeerDirectory ReddeerDirectory;
        protected readonly ILogger Logger;
        private FileSystemWatcher _fileSystemWatcher;
        private readonly string _uploadFileMonitorName;
        private int _retryRestart = 3;

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
                var failedReadsPath = GetFailedReadsPath();
                var uploadDirectoryPath = UploadDirectoryPath();

                Logger.Log(LogLevel.Information, $"{_uploadFileMonitorName} Creating reddeer directory folders at {uploadDirectoryPath} - {failedReadsPath}");
                ReddeerDirectory.Create(uploadDirectoryPath);
                Logger.LogInformation($"Created {uploadDirectoryPath}");
                ReddeerDirectory.Create(failedReadsPath);
                Logger.LogInformation($"Created {failedReadsPath}");
                Logger.Log(LogLevel.Information, $"{_uploadFileMonitorName} Completed creating reddeer directory folders");

                var files = ReddeerDirectory.GetFiles(UploadDirectoryPath(), "*.csv");

                if (files.Any())
                {
                    Logger.LogInformation($"BaseUploadFileMonitor for {_uploadFileMonitorName} detected existing files on initiation. About to process {files.Count} files.");   
                    ProcessInitialStartupFiles(files);
                }

                Logger.LogInformation($"BaseUploadFileMonitor for {_uploadFileMonitorName} setting file system watch");
                SetFileSystemWatch();
            }
            catch (Exception e)
            {
                Logger.LogError($"Exception in {_uploadFileMonitorName} {UploadDirectoryPath()} {e.Message}");
            }
        }

        protected abstract string UploadDirectoryPath();

        protected string GetFailedReadsPath()
        {
            return Path.Combine(UploadDirectoryPath(), "FailedReads");
        }

        private void DetectedFileChange(object source, FileSystemEventArgs e)
        {
            try
            {
                Logger.LogInformation($"BaseUploadFileMonitor detected a file change at {e.FullPath}.");
                ProcessFile(e.FullPath);
            }
            catch (Exception a)
            {
                Logger.LogError("BaseUploadFileMonitor had an error in detected file change", a);
            }
        }

        private void ProcessInitialStartupFiles(IReadOnlyCollection<string> files)
        {
            try
            {
                Logger.LogInformation($"{_uploadFileMonitorName} found some existing files on start up. Processing now");

                foreach (var filePath in files)
                {
                    Logger.LogInformation($"BaseUploadFileMonitor About to process {filePath}");
                    ProcessFile(filePath);
                    Logger.LogInformation($"BaseUploadFileMonitor Completed processing {filePath}");
                }

                Logger.LogInformation($"{_uploadFileMonitorName} has completed processing the initial start up files");
            }
            catch (Exception e)
            {
                Logger.LogError("Base upload file monitor had an error whilst process initial start up files", e);
            }
        }

        public abstract bool ProcessFile(string path);

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
                NotifyFilter = NotifyFilters.FileName,
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
            if (_retryRestart > 0)
            {
                Logger.LogError($"BaseUploadFileMonitor encountered an exception! INVESTIGATE {_retryRestart} retries left", e.GetException());
                _retryRestart -= 1;
                SetFileSystemWatch();
                return;
            }

            Logger.LogCritical($"BaseUploadFileMonitor encountered an exception! RAN OUT OF RETRIES RESTART THE DATA IMPORT SERVICE", e.GetException());

            var exception = e.GetException();
            if (exception.InnerException != null && !string.IsNullOrWhiteSpace(exception.InnerException.Message))
            {
                Logger.LogCritical($"INNER EXCEPTION FOR DATA IMPORT SERVICE FAILURE {exception.InnerException.Message}");
            }
        }

        public void Dispose()
        {
            Logger.LogInformation("BaseUploadFileMonitor called dispose on file monitor.");
            _fileSystemWatcher?.Dispose();
        }
    }
}
