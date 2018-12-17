using System;
using Surveillance.Universe.Interfaces;

namespace Surveillance.Universe.Subscribers.Interfaces
{
    public interface IUniversePercentageCompletionLogger : IObserver<IUniverseEvent>
    {
    }
}
