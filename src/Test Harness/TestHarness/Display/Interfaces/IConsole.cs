using System;
using Domain.Core.Markets.Collections;
using Domain.Core.Trading.Orders;

// ReSharper disable UnusedMember.Global

namespace TestHarness.Display.Interfaces
{
    public interface IConsole
    {
        void OutputMarketFrame(EquityIntraDayTimeBarCollection frame);

        void OutputTradeFrame(Order frame);

        void OutputException(Exception e);

        void ClearCommandInputLine();

        void WriteToUserFeedbackLine(string feedback);
    }
}