using SuperSocket.ClientEngine;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using WebSocket4Net;

namespace Utilities.Network_IO.Websocket_Connections
{
    public interface IConnectionWebsocket
    {
        DateTime LastActiveTime { get; }
        WebSocketVersion Version { get; }
        bool SupportBinary { get; }
        WebSocketState State { get; }
        bool Handshaked { get; }
        IProxyConnector Proxy { get; set; }
        int AutoSendPingInterval { get; set; }
        bool EnableAutoSendPing { get; set; }
        EndPoint LocalEndPoint { get; set; }
        SecurityOption Security { get; }
        int ReceiveBufferSize { get; set; }
        bool NoDelay { get; set; }

        event EventHandler Opened;
        event EventHandler<MessageReceivedEventArgs> MessageReceived;
        event EventHandler<DataReceivedEventArgs> DataReceived;
        event EventHandler<ErrorEventArgs> Error;
        event EventHandler Closed;

        void Close(int statusCode, string reason);
        void Close(string reason);
        void Close();
        Task<bool> CloseAsync();
        void Dispose();
        void Open();
        Task<bool> OpenAsync();
        void Send(IList<ArraySegment<byte>> segments);
        void Send(byte[] data, int offset, int length);
        void Send(string message);
    }
}
