namespace Surveillance.Engine.Rules.Universe.Subscribers.Interfaces
{
    /// <summary>
    /// The UniversePercentageOfEventCompletionLoggerFactory interface.
    /// </summary>
    public interface IUniversePercentageOfEventCompletionLoggerFactory
    {
        /// <summary>
        /// The build.
        /// </summary>
        /// <returns>
        /// The <see cref="IUniversePercentageOfEventCompletionLogger"/>.
        /// </returns>
        IUniversePercentageOfEventCompletionLogger Build();
    }
}