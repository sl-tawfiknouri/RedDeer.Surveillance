﻿using Surveillance.Universe.Subscribers.Interfaces;

namespace Surveillance.Universe.Subscribers
{
    public class UniversePercentageCompletionLoggerFactory : IUniversePercentageCompletionLoggerFactory
    {
        private readonly IUniversePercentageOfEventCompletionLoggerFactory _eventFactory;
        private readonly IUniversePercentageOfTimeCompletionLoggerFactory _timeFactory;

        public UniversePercentageCompletionLoggerFactory(
            IUniversePercentageOfEventCompletionLoggerFactory eventFactory,
            IUniversePercentageOfTimeCompletionLoggerFactory timeFactory)
        {
            _eventFactory = eventFactory;
            _timeFactory = timeFactory;
        }

        public IUniversePercentageCompletionLogger Build()
        {
            return new UniversePercentageCompletionLogger(_eventFactory.Build(), _timeFactory.Build());
        }
    }
}
