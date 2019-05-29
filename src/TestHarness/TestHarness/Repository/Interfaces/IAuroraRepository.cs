using System;

namespace TestHarness.Repository.Interfaces
{
    public interface IAuroraRepository
    {
        void DeleteTradingAndMarketData();
        void DeleteTradingAndMarketDataForMarketOnDate(string market, DateTime date);
    }
}