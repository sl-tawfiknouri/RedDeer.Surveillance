using System;

namespace Domain.Equity.Trading
{
    public class StockExchangeStream : IObservable<ExchangeTick>
    {
        public IDisposable Subscribe(IObserver<ExchangeTick> observer)
        {
            throw new NotImplementedException();
        }
    }
}
