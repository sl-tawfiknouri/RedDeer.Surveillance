namespace TestHarness.Factory.TradeHighVolumeFactory.Interfaces
{
    using System.Collections.Generic;

    using TestHarness.Engine.OrderGenerator.Interfaces;

    public interface ITradingHighVolumeFactory
    {
        IOrderDataGenerator Build(IReadOnlyCollection<string> sedols);
    }
}