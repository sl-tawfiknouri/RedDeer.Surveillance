namespace DataImport.Disk_IO.Interfaces
{
    using System;

    public interface IBaseUploadFileMonitor : IDisposable
    {
        void Initiate();
    }
}