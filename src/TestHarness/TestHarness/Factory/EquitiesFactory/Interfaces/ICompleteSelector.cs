using TestHarness.Engine.EquitiesGenerator.Interfaces;

namespace TestHarness.Factory.EquitiesFactory.Interfaces
{
    public interface ICompleteSelector
    {
        IEquityDataGenerator Finish();
    }
}
