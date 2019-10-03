namespace Surveillance.Engine.Rules.Universe.Subscribers.Interfaces
{
    /// <summary>
    /// The UniversePercentageCompletionLoggerFactory interface.
    /// </summary>
    public interface IUniversePercentageCompletionLoggerFactory
    {
        /// <summary>
        /// The build.
        /// </summary>
        /// <returns>
        /// The <see cref="IUniversePercentageCompletionLogger"/>.
        /// </returns>
        IUniversePercentageCompletionLogger Build();
    }
}