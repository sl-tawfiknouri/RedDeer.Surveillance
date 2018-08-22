using TestHarness.Network_IO;

namespace TestHarness.Factory.NetworkFactory.Interfaces
{
    public interface INetworkManagerFactory
    {
        INetworkManager CreateStub();
        INetworkManager CreateWebsockets();
    }
}