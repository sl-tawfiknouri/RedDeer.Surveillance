using TestHarness.Engine.EquitiesStorage.Interfaces;

namespace TestHarness.Factory.EquitiesFactory.Interfaces
{
    public interface IEquitiesFileStorageProcessFactory
    {
        IEquityDataStorage Create(string path);
    }
}
