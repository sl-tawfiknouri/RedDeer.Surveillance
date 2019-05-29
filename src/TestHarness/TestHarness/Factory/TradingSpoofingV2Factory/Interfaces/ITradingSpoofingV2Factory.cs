using System.Collections.Generic;
using TestHarness.Engine.OrderGenerator.Interfaces;

namespace TestHarness.Factory.TradingSpoofingV2Factory.Interfaces
{
    public interface ITradingSpoofingV2Factory
    {
        IOrderDataGenerator Build(IReadOnlyCollection<string> sedols);
    }
}