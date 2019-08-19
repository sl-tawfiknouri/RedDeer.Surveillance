namespace Surveillance.Auditing.Logging.Interfaces
{
    using System;

    using Surveillance.Auditing.DataLayer.Processes.Interfaces;

    public interface IOperationLogging
    {
        void Log(Exception e, ISystemProcess process);

        void Log(Exception e, ISystemProcessOperation operation);

        void Log(Exception e, ISystemProcessOperationDistributeRule distributeRule);

        void Log(Exception e, ISystemProcessOperationRuleRun ruleRun);

        void Log(Exception e, ISystemProcessOperationUploadFile uploadRule);

        void Log(Exception e, ISystemProcessOperationThirdPartyDataRequest request);
    }
}