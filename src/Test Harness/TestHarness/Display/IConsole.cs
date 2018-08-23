using Domain.Equity.Trading.Frames;
using Domain.Equity.Trading.Orders;
using System;
using Utilities.Network_IO.Interfaces;

namespace TestHarness.Display
{
    public interface IConsole : IMessageWriter
    {
        void OutputMarketFrame(ExchangeFrame frame);

        void OutputTradeFrame(TradeOrderFrame frame);

        void OutputException(Exception e);

        void ClearCommandInputLine();

        void WriteToUserFeedbackLine(string feedback);
    }
}