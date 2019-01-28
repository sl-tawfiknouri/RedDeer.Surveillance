using System;
using Surveillance.Systems.DataLayer.Processes.Interfaces;

namespace Surveillance.Systems.Auditing.Context.Interfaces
{
    public interface ISystemProcessOperationRunRuleContext
    {
        void StartEvent(ISystemProcessOperationRuleRun ruleRun);
        ISystemProcessOperationContext EndEvent();
        void EventException(string message);
        void EventException(Exception e);
        string Id();
        string CorrelationId();
        ISystemProcessOperationContext SystemProcessOperationContext();
    }
}