namespace Surveillance.Engine.Rules.Factories.Interfaces
{
    using System.Threading;

    using Surveillance.Engine.Rules.Universe.Interfaces;

    public interface IUniversePlayerFactory
    {
        IUniversePlayer Build(CancellationToken ct);
    }
}