using System;

namespace DataImport.Disk_IO.Interfaces
{
    public interface IUploadTradeFileMonitor : IDisposable
    {
        void Initiate();
        bool ProcessFile(string path);
    }
}