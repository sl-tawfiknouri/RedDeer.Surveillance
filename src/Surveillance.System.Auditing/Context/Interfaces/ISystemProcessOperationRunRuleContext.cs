using System;
using Surveillance.Auditing.DataLayer.Processes.Interfaces;

namespace Surveillance.Auditing.Context.Interfaces
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