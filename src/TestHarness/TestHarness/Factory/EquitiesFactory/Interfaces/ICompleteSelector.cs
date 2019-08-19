namespace TestHarness.Factory.EquitiesFactory.Interfaces
{
    using TestHarness.Engine.EquitiesGenerator.Interfaces;

    public interface ICompleteSelector
    {
        IEquityDataGenerator Finish();
    }
}