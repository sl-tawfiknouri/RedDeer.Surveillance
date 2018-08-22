﻿using NLog;
using TestHarness.Commands;
using TestHarness.Engine.EquitiesGenerator.Interfaces;
using TestHarness.Network_IO;

namespace TestHarness.Factory
{
    public interface IAppFactory
    {
        IEquityDataGenerator Build();

        ILogger Logger { get; }

        INetworkManager NetworkManager { get; }

        ICommandManager CommandManager { get; }
    }
}