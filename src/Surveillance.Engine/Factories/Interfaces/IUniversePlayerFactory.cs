using System.Threading;
using Surveillance.Engine.Rules.Universe.Interfaces;

namespace Surveillance.Engine.Rules.Factories.Interfaces
{
    public interface IUniversePlayerFactory
    {
        IUniversePlayer Build(CancellationToken ct);
    }
}