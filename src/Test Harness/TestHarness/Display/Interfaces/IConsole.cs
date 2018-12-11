using System;
using DomainV2.Equity.Frames;
using DomainV2.Trading;
using Utilities.Network_IO.Interfaces;
// ReSharper disable UnusedMember.Global

namespace TestHarness.Display.Interfaces
{
    public interface IConsole : IMessageWriter
    {
        void OutputMarketFrame(ExchangeFrame frame);

        void OutputTradeFrame(Order frame);

        void OutputException(Exception e);

        void ClearCommandInputLine();

        void WriteToUserFeedbackLine(string feedback);
    }
}