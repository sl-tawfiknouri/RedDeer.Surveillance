using NLog;
using TestHarness.Engine.EquitiesGenerator.Interfaces;
using TestHarness.Network_IO;

namespace TestHarness.Factory
{
    public interface IAppFactory
    {
        IEquityDataGenerator Build();

        ILogger Logger { get; }

        INetworkManager NetworkManager { get; }
    }
}