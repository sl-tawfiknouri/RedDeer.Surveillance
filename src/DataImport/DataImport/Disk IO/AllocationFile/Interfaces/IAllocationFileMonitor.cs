namespace DataImport.Disk_IO.AllocationFile.Interfaces
{
    using System;

    public interface IUploadAllocationFileMonitor : IDisposable
    {
        void Initiate();

        bool ProcessFile(string path);
    }
}