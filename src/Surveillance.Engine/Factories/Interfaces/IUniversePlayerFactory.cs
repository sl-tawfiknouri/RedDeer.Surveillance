namespace Surveillance.Engine.Rules.Factories.Interfaces
{
    using System.Threading;

    using Surveillance.Engine.Rules.Universe.Interfaces;

    /// <summary>
    /// The UniversePlayerFactory interface.
    /// </summary>
    public interface IUniversePlayerFactory
    {
        /// <summary>
        /// The build universe player functionality.
        /// </summary>
        /// <param name="ct">
        /// The ct.
        /// </param>
        /// <returns>
        /// The <see cref="IUniversePlayer"/>.
        /// </returns>
        IUniversePlayer Build(CancellationToken ct);
    }
}