namespace TestHarness.Repository.Interfaces
{
    using System;

    public interface IAuroraRepository
    {
        void DeleteTradingAndMarketData();

        void DeleteTradingAndMarketDataForMarketOnDate(string market, DateTime date);
    }
}