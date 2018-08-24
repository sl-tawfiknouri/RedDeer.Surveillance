using System;
using System.Timers;

namespace Utilities.Network_IO.Websocket_Connections.Interfaces
{
    public interface INetworkSwitch : IDisposable
    {
        bool Add<T>(T value);
        void MonitorFailover(object sender, ElapsedEventArgs e);
    }
}