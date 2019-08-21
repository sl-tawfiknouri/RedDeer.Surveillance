namespace TestHarness.Factory.EquitiesFactory.Interfaces
{
    using TestHarness.Engine.EquitiesStorage.Interfaces;

    public interface IEquitiesFileStorageProcessFactory
    {
        IEquityDataStorage Create(string path);
    }
}