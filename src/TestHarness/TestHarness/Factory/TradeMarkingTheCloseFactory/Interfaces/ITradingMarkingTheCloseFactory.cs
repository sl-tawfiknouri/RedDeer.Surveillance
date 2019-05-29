using System.Collections.Generic;
using RedDeer.Contracts.SurveillanceService.Api.Markets;
using TestHarness.Engine.OrderGenerator.Interfaces;

namespace TestHarness.Factory.TradeMarkingTheCloseFactory.Interfaces
{
    public interface ITradingMarkingTheCloseFactory
    {
        IOrderDataGenerator Build(IReadOnlyCollection<string> sedols, ExchangeDto market);
    }
}