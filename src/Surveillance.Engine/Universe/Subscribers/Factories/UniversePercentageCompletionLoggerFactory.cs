namespace Surveillance.Engine.Rules.Universe.Subscribers.Factories
{
    using Surveillance.Engine.Rules.Universe.Subscribers.Interfaces;

    public class UniversePercentageCompletionLoggerFactory : IUniversePercentageCompletionLoggerFactory
    {
        private readonly IUniversePercentageOfEventCompletionLoggerFactory _eventFactory;

        private readonly IUniversePercentageOfTimeCompletionLoggerFactory _timeFactory;

        public UniversePercentageCompletionLoggerFactory(
            IUniversePercentageOfEventCompletionLoggerFactory eventFactory,
            IUniversePercentageOfTimeCompletionLoggerFactory timeFactory)
        {
            this._eventFactory = eventFactory;
            this._timeFactory = timeFactory;
        }

        public IUniversePercentageCompletionLogger Build()
        {
            return new UniversePercentageCompletionLogger(this._eventFactory.Build(), this._timeFactory.Build());
        }
    }
}