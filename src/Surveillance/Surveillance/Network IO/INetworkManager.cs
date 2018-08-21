using Domain.Streams;

namespace Surveillance.Network_IO
{
    public interface INetworkManager<U, V>
        where U : PublishingStream<V>
        where V : class
    {
        void InitiateConnections(U stream);
        void TerminateConnections();
    }
}