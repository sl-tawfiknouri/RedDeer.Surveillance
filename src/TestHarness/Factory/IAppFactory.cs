using NLog;
using TestHarness.Engine.EquitiesGenerator.Interfaces;

namespace TestHarness.Factory
{
    public interface IAppFactory
    {
        IEquityDataGenerator Build();

        ILogger Logger { get; }
    }
}