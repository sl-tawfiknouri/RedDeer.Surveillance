using TestHarness.Network_IO.Interfaces;

namespace TestHarness.Factory.NetworkFactory.Interfaces
{
    public interface INetworkManagerFactory
    {
        // ReSharper disable once UnusedMember.Global
        INetworkManager CreateStub();
        INetworkManager CreateWebsockets();
    }
}