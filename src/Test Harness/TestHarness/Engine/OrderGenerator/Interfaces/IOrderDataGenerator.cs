using System;
using Domain.Equity.Frames;
using Domain.Equity.Streams.Interfaces;
using Domain.Trades.Orders;
using Domain.Trades.Streams.Interfaces;

namespace TestHarness.Engine.OrderGenerator.Interfaces
{
    public interface IOrderDataGenerator : IObserver<ExchangeFrame>
    {
        void InitiateTrading(IStockExchangeStream stockStream, ITradeOrderStream<TradeOrderFrame> tradeStream);
        void TerminateTrading();
    }
}