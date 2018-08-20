using Domain.Equity.Trading.Frames;
using Domain.Equity.Trading.Orders;
using System;

namespace TestHarness.Display
{
    public interface IConsole
    {
        void OutputMarketFrame(ExchangeFrame frame);

        void OutputTradeFrame(TradeOrderFrame frame);

        void OutputException(Exception e);
    }
}