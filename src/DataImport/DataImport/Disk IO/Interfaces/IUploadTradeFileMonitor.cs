namespace DataImport.Disk_IO.Interfaces
{
    using System;

    public interface IUploadTradeFileMonitor : IDisposable
    {
        void Initiate();

        bool ProcessFile(string path);
    }
}