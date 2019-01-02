using TestHarness.Engine.EquitiesGenerator.Interfaces;

namespace TestHarness.Factory.EquitiesFactory.Interfaces
{
    public interface IEquitiesFileDataImportProcessFactory
    {
        IEquityDataGenerator Create(string filePath);
    }
}