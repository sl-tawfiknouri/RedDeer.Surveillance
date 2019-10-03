namespace Surveillance.Engine.Rules.Universe.Subscribers.Factories
{
    using Surveillance.Engine.Rules.Universe.Subscribers.Interfaces;

    /// <summary>
    /// The universe percentage completion logger factory.
    /// </summary>
    public class UniversePercentageCompletionLoggerFactory : IUniversePercentageCompletionLoggerFactory
    {
        /// <summary>
        /// The event factory.
        /// </summary>
        private readonly IUniversePercentageOfEventCompletionLoggerFactory eventFactory;

        /// <summary>
        /// The time factory.
        /// </summary>
        private readonly IUniversePercentageOfTimeCompletionLoggerFactory timeFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="UniversePercentageCompletionLoggerFactory"/> class.
        /// </summary>
        /// <param name="eventFactory">
        /// The event factory.
        /// </param>
        /// <param name="timeFactory">
        /// The time factory.
        /// </param>
        public UniversePercentageCompletionLoggerFactory(
            IUniversePercentageOfEventCompletionLoggerFactory eventFactory,
            IUniversePercentageOfTimeCompletionLoggerFactory timeFactory)
        {
            this.eventFactory = eventFactory;
            this.timeFactory = timeFactory;
        }

        /// <summary>
        /// The build.
        /// </summary>
        /// <returns>
        /// The <see cref="IUniversePercentageCompletionLogger"/>.
        /// </returns>
        public IUniversePercentageCompletionLogger Build()
        {
            return new UniversePercentageCompletionLogger(this.eventFactory.Build(), this.timeFactory.Build());
        }
    }
}