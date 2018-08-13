using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Domain.Equity.Trading
{
    public class StockExchangeStream : IObservable<ExchangeTick>
    {
        private readonly IList<IObserver<ExchangeTick>> _observers;

        public StockExchangeStream()
        {
            _observers = new List<IObserver<ExchangeTick>>();
        }

        public IDisposable Subscribe(IObserver<ExchangeTick> observer)
        {
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
            }

            throw new ArgumentException();
        }
    }
}
