using System;
using Surveillance.DataLayer.Aurora.Exceptions;
using Surveillance.DataLayer.Aurora.Exceptions.Interfaces;
using Surveillance.Logging.Interfaces;
using Surveillance.System.DataLayer.Processes.Interfaces;

namespace Surveillance.Logging
{
    public class OperationLogging : IOperationLogging
    {
        private readonly IExceptionRepository _exceptionRepository;

        public OperationLogging(IExceptionRepository exceptionRepository)
        {
            _exceptionRepository = exceptionRepository ?? throw new ArgumentNullException(nameof(exceptionRepository));
        }

        public void Log(Exception e)
        {
            var dto =
                new ExceptionDto
                {
                    ExceptionMessage = e.Message,
                    InnerExceptionMessage = e.InnerException?.Message,
                    StackTrace = e.StackTrace
                };

            _exceptionRepository.Save(dto);
        }

        public void Log(Exception e, ISystemProcess process)
        {
            if (process == null)
            {
                Log(e);
                return;
            }

            var dto =
                new ExceptionDto
                {
                    ExceptionMessage = e.Message,
                    InnerExceptionMessage = e.InnerException?.Message,
                    StackTrace = e.StackTrace,
                    SystemProcessId = process?.Id
                };

            _exceptionRepository.Save(dto);
        }

        public void Log(Exception e, ISystemProcessOperation operation)
        {
            if (operation == null)
            {
                Log(e);
                return;
            }

            var dto =
                new ExceptionDto
                {
                    ExceptionMessage = e.Message,
                    InnerExceptionMessage = e.InnerException?.Message,
                    StackTrace = e.StackTrace,
                    SystemProcessOperationId = operation.Id,
                    SystemProcessId = operation.SystemProcessId,
                };

            _exceptionRepository.Save(dto);
        }

        public void Log(Exception e, ISystemProcessOperationRuleRun ruleRun)
        {
            if (ruleRun == null)
            {
                Log(e);
                return;
            }

            var dto =
                new ExceptionDto
                {
                    ExceptionMessage = e.Message,
                    InnerExceptionMessage = e.InnerException?.Message,
                    StackTrace = e.StackTrace,
                    SystemProcessOperationRuleRunId = ruleRun.Id,
                    SystemProcessOperationId = ruleRun.SystemProcessOperationId,
                    SystemProcessId = ruleRun.SystemProcessId
                };

            _exceptionRepository.Save(dto);
        }

        public void Log(Exception e, ISystemProcessOperationDistributeRule distributeRule)
        {
            if (distributeRule == null)
            {
                Log(e);
                return;
            }

            var dto =
                new ExceptionDto
                {
                    ExceptionMessage = e.Message,
                    InnerExceptionMessage = e.InnerException?.Message,
                    StackTrace = e.StackTrace,
                    SystemProcessOperationDistributeRuleId = distributeRule.Id,
                    SystemProcessOperationId = distributeRule.SystemProcessOperationId,
                    SystemProcessId = distributeRule.SystemProcessId
                };

            _exceptionRepository.Save(dto);
        }
    }
}
