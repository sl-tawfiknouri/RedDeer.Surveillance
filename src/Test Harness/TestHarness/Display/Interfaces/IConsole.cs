using System;
using DomainV2.Equity.TimeBars;
using DomainV2.Trading;
// ReSharper disable UnusedMember.Global

namespace TestHarness.Display.Interfaces
{
    public interface IConsole
    {
        void OutputMarketFrame(MarketTimeBarCollection frame);

        void OutputTradeFrame(Order frame);

        void OutputException(Exception e);

        void ClearCommandInputLine();

        void WriteToUserFeedbackLine(string feedback);
    }
}