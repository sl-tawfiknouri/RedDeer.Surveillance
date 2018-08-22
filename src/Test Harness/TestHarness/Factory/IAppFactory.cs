using NLog;
using TestHarness.Commands;
using TestHarness.Commands.Interfaces;
using TestHarness.Engine.EquitiesGenerator.Interfaces;
using TestHarness.Interfaces;
using TestHarness.Network_IO;

namespace TestHarness.Factory
{
    public interface IAppFactory
    {
        void Build();

        ILogger Logger { get; }

        INetworkManager NetworkManager { get; }

        ICommandManager CommandManager { get; }

        ICommandManifest CommandManifest { get; }

        IProgramState State { get; }

        IEquityDataGenerator EquityDataGenerator { get; }
    }
}