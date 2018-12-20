using System;
using Surveillance.System.DataLayer.Processes.Interfaces;

namespace Surveillance.System.Auditing.Context.Interfaces
{
    public interface ISystemProcessOperationRunRuleContext
    {
        void StartEvent(ISystemProcessOperationRuleRun ruleRun);
        ISystemProcessOperationContext EndEvent();
        void EventException(string message);
        void EventException(Exception e);
        string Id();
    }
}