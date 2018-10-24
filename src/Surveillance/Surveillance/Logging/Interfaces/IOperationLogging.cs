using System;
using Surveillance.System.DataLayer.Processes.Interfaces;

namespace Surveillance.Logging.Interfaces
{
    public interface IOperationLogging
    {
        void Log(Exception e);
        void Log(Exception e, ISystemProcess process);
        void Log(Exception e, ISystemProcessOperation operation);
        void Log(Exception e, ISystemProcessOperationDistributeRule distributeRule);
        void Log(Exception e, ISystemProcessOperationRuleRun ruleRun);
    }
}