// ReSharper disable UnusedMember.Global

namespace TestHarness.Display.Interfaces
{
    using System;

    using Domain.Core.Markets.Collections;
    using Domain.Core.Trading.Orders;

    public interface IConsole
    {
        void ClearCommandInputLine();

        void OutputException(Exception e);

        void OutputMarketFrame(EquityIntraDayTimeBarCollection frame);

        void OutputTradeFrame(Order frame);

        void WriteToUserFeedbackLine(string feedback);
    }
}