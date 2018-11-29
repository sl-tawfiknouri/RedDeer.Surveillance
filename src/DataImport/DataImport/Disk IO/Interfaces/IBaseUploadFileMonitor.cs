using System;

namespace Relay.Disk_IO.Interfaces
{
    public interface IBaseUploadFileMonitor : IDisposable
    {
        void Initiate();
    }
}