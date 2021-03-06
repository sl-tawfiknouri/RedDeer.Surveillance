﻿namespace Domain.Surveillance.Streams
{
    using System;
    using System.Collections.Concurrent;

    using Domain.Surveillance.Streams.Interfaces;

    public class UnsubscriberFactory<T> : IUnsubscriberFactory<T>
    {
        /// <summary>
        ///     Create unsubscriber instances
        /// </summary>
        public Unsubscriber<T> Create(ConcurrentDictionary<IObserver<T>, IObserver<T>> observers, IObserver<T> observer)
        {
            return new Unsubscriber<T>(observers, observer);
        }
    }
}