using TestHarness.Network_IO.Interfaces;

namespace TestHarness.Factory.NetworkFactory.Interfaces
{
    public interface INetworkManagerFactory
    {
        INetworkManager CreateStub();
        INetworkManager CreateWebsockets();
    }
}