using System;

namespace DataImport.Disk_IO.AllocationFile
{
    public interface IUploadAllocationFileMonitor : IDisposable
    {
        bool ProcessFile(string path);
    }
}