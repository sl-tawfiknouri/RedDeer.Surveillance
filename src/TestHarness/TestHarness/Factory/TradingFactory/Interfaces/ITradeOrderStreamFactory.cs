﻿using Domain.Core.Trading.Orders;
using Domain.Surveillance.Streams.Interfaces;
using TestHarness.Display.Interfaces;

namespace TestHarness.Factory.TradingFactory.Interfaces
{
    public interface ITradeOrderStreamFactory
    {
        IOrderStream<Order> Create();
        IOrderStream<Order> CreateDisplayable(IConsole console);
    }
}