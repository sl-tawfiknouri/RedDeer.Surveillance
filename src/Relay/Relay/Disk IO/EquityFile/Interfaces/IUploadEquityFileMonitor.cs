using System;

namespace Relay.Disk_IO.EquityFile.Interfaces
{
    public interface IUploadEquityFileMonitor : IDisposable
    {
        void Initiate();
        bool ProcessFile(string path);
    }
}
