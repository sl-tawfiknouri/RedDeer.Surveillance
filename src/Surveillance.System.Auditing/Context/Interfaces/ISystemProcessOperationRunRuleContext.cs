namespace Surveillance.Auditing.Context.Interfaces
{
    using System;

    using Surveillance.Auditing.DataLayer.Processes.Interfaces;

    public interface ISystemProcessOperationRunRuleContext
    {
        string CorrelationId();

        ISystemProcessOperationContext EndEvent();

        void EventException(string message);

        void EventException(Exception e);

        string Id();

        bool IsBackTest();

        string RuleParameterId();

        void StartEvent(ISystemProcessOperationRuleRun ruleRun);

        ISystemProcessOperationContext SystemProcessOperationContext();
    }
}