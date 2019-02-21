using System;

namespace DataImport.Disk_IO.AllocationFile.Interfaces
{
    public interface IUploadAllocationFileMonitor : IDisposable
    {
        bool ProcessFile(string path);
        void Initiate();
    }
}