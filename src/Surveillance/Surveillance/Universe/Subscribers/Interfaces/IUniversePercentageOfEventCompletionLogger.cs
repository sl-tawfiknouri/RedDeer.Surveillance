using System;
using Surveillance.Universe.Interfaces;

namespace Surveillance.Universe.Subscribers.Interfaces
{
    public interface IUniversePercentageOfEventCompletionLogger : IObserver<IUniverseEvent>
    {
        void InitiateEventLogger(IUniverse universe);
    }
}
