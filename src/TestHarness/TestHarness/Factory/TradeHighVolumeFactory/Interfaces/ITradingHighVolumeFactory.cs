using System.Collections.Generic;
using TestHarness.Engine.OrderGenerator.Interfaces;

namespace TestHarness.Factory.TradeHighVolumeFactory.Interfaces
{
    public interface ITradingHighVolumeFactory
    {
        IOrderDataGenerator Build(IReadOnlyCollection<string> sedols);
    }
}