namespace DataImport.Disk_IO
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using DataImport.Disk_IO.Interfaces;

    using Infrastructure.Network.Disk.Interfaces;

    using Microsoft.Extensions.Logging;

    public abstract class BaseUploadFileMonitor : IBaseUploadFileMonitor
    {
        protected readonly ILogger Logger;

        protected readonly IReddeerDirectory ReddeerDirectory;

        private readonly string _uploadFileMonitorName;

        private FileSystemWatcher _fileSystemWatcher;

        private int _retryRestart = 3;

        protected BaseUploadFileMonitor(IReddeerDirectory directory, ILogger logger, string uploadFileMonitorName)
        {
            this.ReddeerDirectory = directory ?? throw new ArgumentNullException(nameof(directory));
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._uploadFileMonitorName =
                uploadFileMonitorName ?? throw new ArgumentNullException(nameof(uploadFileMonitorName));
        }

        public void Dispose()
        {
            this.Logger.LogInformation("BaseUploadFileMonitor called dispose on file monitor.");
            this._fileSystemWatcher?.Dispose();
        }

        /// <summary>
        ///     Initiate monitoring
        /// </summary>
        public void Initiate()
        {
            if (string.IsNullOrWhiteSpace(this.UploadDirectoryPath()))
            {
                this.Logger.Log(
                    LogLevel.Information,
                    $"{this._uploadFileMonitorName} - no path in upload directory. Will not monitor unspecified upload folder path");
                return;
            }

            this.Logger.Log(LogLevel.Information, $"{this._uploadFileMonitorName} Initiating monitoring process.");

            try
            {
                var failedReadsPath = this.GetFailedReadsPath();
                var uploadDirectoryPath = this.UploadDirectoryPath();

                this.Logger.Log(
                    LogLevel.Information,
                    $"{this._uploadFileMonitorName} Creating reddeer directory folders at {uploadDirectoryPath} - {failedReadsPath}");
                this.ReddeerDirectory.Create(uploadDirectoryPath);
                this.Logger.LogInformation($"Created {uploadDirectoryPath}");
                this.ReddeerDirectory.Create(failedReadsPath);
                this.Logger.LogInformation($"Created {failedReadsPath}");
                this.Logger.Log(
                    LogLevel.Information,
                    $"{this._uploadFileMonitorName} Completed creating reddeer directory folders");

                var files = this.ReddeerDirectory.GetFiles(this.UploadDirectoryPath(), "*.csv");

                if (files.Any())
                {
                    this.Logger.LogInformation(
                        $"BaseUploadFileMonitor for {this._uploadFileMonitorName} detected existing files on initiation. About to process {files.Count} files.");
                    this.ProcessInitialStartupFiles(files);
                }

                this.Logger.LogInformation(
                    $"BaseUploadFileMonitor for {this._uploadFileMonitorName} setting file system watch");
                this.SetFileSystemWatch();
            }
            catch (Exception e)
            {
                this.Logger.LogError(
                    $"Exception in {this._uploadFileMonitorName} {this.UploadDirectoryPath()} {e.Message}");
            }
        }

        public abstract bool ProcessFile(string path);

        protected string GetFailedReadsPath()
        {
            return Path.Combine(this.UploadDirectoryPath(), "FailedReads");
        }

        protected abstract string UploadDirectoryPath();

        private void DetectedFileChange(object source, FileSystemEventArgs e)
        {
            try
            {
                this.Logger.LogInformation($"BaseUploadFileMonitor detected a file change at {e.FullPath}.");
                this.ProcessFile(e.FullPath);
            }
            catch (Exception a)
            {
                this.Logger.LogError("BaseUploadFileMonitor had an error in detected file change", a);
            }
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            if (this._retryRestart > 0)
            {
                this.Logger.LogError(
                    $"BaseUploadFileMonitor encountered an exception! INVESTIGATE {this._retryRestart} retries left",
                    e.GetException());
                this._retryRestart -= 1;
                this.SetFileSystemWatch();
                return;
            }

            this.Logger.LogCritical(
                "BaseUploadFileMonitor encountered an exception! RAN OUT OF RETRIES RESTART THE DATA IMPORT SERVICE",
                e.GetException());

            var exception = e.GetException();
            if (exception.InnerException != null && !string.IsNullOrWhiteSpace(exception.InnerException.Message))
                this.Logger.LogCritical(
                    $"INNER EXCEPTION FOR DATA IMPORT SERVICE FAILURE {exception.InnerException.Message}");
        }

        private void ProcessInitialStartupFiles(IReadOnlyCollection<string> files)
        {
            try
            {
                this.Logger.LogInformation(
                    $"{this._uploadFileMonitorName} found some existing files on start up. Processing now");

                foreach (var filePath in files)
                {
                    this.Logger.LogInformation($"BaseUploadFileMonitor About to process {filePath}");
                    this.ProcessFile(filePath);
                    this.Logger.LogInformation($"BaseUploadFileMonitor Completed processing {filePath}");
                }

                this.Logger.LogInformation(
                    $"{this._uploadFileMonitorName} has completed processing the initial start up files");
            }
            catch (Exception e)
            {
                this.Logger.LogError("Base upload file monitor had an error whilst process initial start up files", e);
            }
        }

        private void SetFileSystemWatch()
        {
            this.Logger.LogInformation("BaseUploadFileMonitor setting file system watch");

            if (!this.ReddeerDirectory.DirectoryExists(this.UploadDirectoryPath()))
            {
                this.Logger.LogInformation(
                    $"BaseUploadFileMonitor did not find the {this.UploadDirectoryPath()} not setting file watch.");

                return;
            }

            if (this._fileSystemWatcher != null)
            {
                this.Logger.LogInformation("BaseUploadFileMonitor disposing an old file system watcher.");

                this._fileSystemWatcher.Dispose();
                this._fileSystemWatcher = null;
            }

            this._fileSystemWatcher = new FileSystemWatcher(this.UploadDirectoryPath())
                                          {
                                              NotifyFilter = NotifyFilters.FileName,
                                              Filter = "*.csv",
                                              IncludeSubdirectories = false
                                          };

            this._fileSystemWatcher.Error += this.OnError;
            this._fileSystemWatcher.Changed += this.DetectedFileChange;
            this._fileSystemWatcher.Renamed += this.DetectedFileChange;
            this._fileSystemWatcher.Created += this.DetectedFileChange;

            this._fileSystemWatcher.EnableRaisingEvents = true;
            this.Logger.LogInformation(
                "BaseUploadFileMonitor set file system watch events and now enabled raising events.");
        }
    }
}