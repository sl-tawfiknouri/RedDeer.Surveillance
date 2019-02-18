using System;
using Surveillance.Engine.Rules.Universe.Interfaces;

namespace Surveillance.Engine.Rules.Universe.Subscribers.Interfaces
{
    public interface IUniversePercentageOfEventCompletionLogger : IObserver<IUniverseEvent>
    {
        void InitiateEventLogger(IUniverse universe);
    }
}
