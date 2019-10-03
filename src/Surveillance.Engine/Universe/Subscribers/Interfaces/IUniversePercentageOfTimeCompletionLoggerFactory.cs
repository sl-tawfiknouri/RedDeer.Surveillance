namespace Surveillance.Engine.Rules.Universe.Subscribers.Interfaces
{
    /// <summary>
    /// The UniversePercentageOfTimeCompletionLoggerFactory interface.
    /// </summary>
    public interface IUniversePercentageOfTimeCompletionLoggerFactory
    {
        /// <summary>
        /// The build.
        /// </summary>
        /// <returns>
        /// The <see cref="IUniversePercentageOfTimeCompletionLogger"/>.
        /// </returns>
        IUniversePercentageOfTimeCompletionLogger Build();
    }
}