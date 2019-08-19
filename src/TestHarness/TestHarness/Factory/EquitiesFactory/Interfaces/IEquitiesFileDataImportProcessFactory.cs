namespace TestHarness.Factory.EquitiesFactory.Interfaces
{
    using TestHarness.Engine.EquitiesGenerator.Interfaces;

    public interface IEquitiesFileDataImportProcessFactory
    {
        IEquityDataGenerator Create(string filePath);
    }
}