using System;

namespace DataImport.Disk_IO.Interfaces
{
    public interface IBaseUploadFileMonitor : IDisposable
    {
        void Initiate();
    }
}