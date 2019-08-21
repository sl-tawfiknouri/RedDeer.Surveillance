namespace Surveillance.Auditing.Logging
{
    using System;

    using Surveillance.Auditing.DataLayer.Processes.Interfaces;
    using Surveillance.Auditing.DataLayer.Repositories.Exceptions;
    using Surveillance.Auditing.DataLayer.Repositories.Exceptions.Interfaces;
    using Surveillance.Auditing.Logging.Interfaces;

    public class OperationLogging : IOperationLogging
    {
        private readonly IExceptionRepository _exceptionRepository;

        public OperationLogging(IExceptionRepository exceptionRepository)
        {
            this._exceptionRepository =
                exceptionRepository ?? throw new ArgumentNullException(nameof(exceptionRepository));
        }

        public void Log(Exception e, ISystemProcess process)
        {
            if (process == null)
            {
                this.Log(e);
                return;
            }

            var dto = new ExceptionDto
                          {
                              ExceptionMessage = e.Message,
                              InnerExceptionMessage = e.InnerException?.Message,
                              StackTrace = e.StackTrace,
                              SystemProcessId = process?.Id
                          };

            this._exceptionRepository.Save(dto);
        }

        public void Log(Exception e, ISystemProcessOperation operation)
        {
            if (operation == null)
            {
                this.Log(e);
                return;
            }

            var dto = new ExceptionDto
                          {
                              ExceptionMessage = e.Message,
                              InnerExceptionMessage = e.InnerException?.Message,
                              StackTrace = e.StackTrace,
                              SystemProcessOperationId = operation.Id,
                              SystemProcessId = operation.SystemProcessId
                          };

            this._exceptionRepository.Save(dto);
        }

        public void Log(Exception e, ISystemProcessOperationRuleRun ruleRun)
        {
            if (ruleRun == null)
            {
                this.Log(e);
                return;
            }

            var dto = new ExceptionDto
                          {
                              ExceptionMessage = e.Message,
                              InnerExceptionMessage = e.InnerException?.Message,
                              StackTrace = e.StackTrace,
                              SystemProcessOperationRuleRunId = ruleRun.Id,
                              SystemProcessOperationId = ruleRun.SystemProcessOperationId,
                              SystemProcessId = ruleRun.SystemProcessId
                          };

            this._exceptionRepository.Save(dto);
        }

        public void Log(Exception e, ISystemProcessOperationDistributeRule distributeRule)
        {
            if (distributeRule == null)
            {
                this.Log(e);
                return;
            }

            var dto = new ExceptionDto
                          {
                              ExceptionMessage = e.Message,
                              InnerExceptionMessage = e.InnerException?.Message,
                              StackTrace = e.StackTrace,
                              SystemProcessOperationDistributeRuleId = distributeRule.Id,
                              SystemProcessOperationId = distributeRule.SystemProcessOperationId,
                              SystemProcessId = distributeRule.SystemProcessId
                          };

            this._exceptionRepository.Save(dto);
        }

        public void Log(Exception e, ISystemProcessOperationUploadFile uploadRule)
        {
            if (uploadRule == null)
            {
                this.Log(e);
                return;
            }

            var dto = new ExceptionDto
                          {
                              ExceptionMessage = e.Message,
                              InnerExceptionMessage = e.InnerException?.Message,
                              StackTrace = e.StackTrace,
                              SystemProcessOperationUploadFileRuleId = uploadRule.Id,
                              SystemProcessOperationId = uploadRule.SystemProcessOperationId,
                              SystemProcessId = uploadRule.SystemProcessId
                          };

            this._exceptionRepository.Save(dto);
        }

        public void Log(Exception e, ISystemProcessOperationThirdPartyDataRequest request)
        {
            if (request == null)
            {
                this.Log(e);
                return;
            }

            var dto = new ExceptionDto
                          {
                              ExceptionMessage = e.Message,
                              InnerExceptionMessage = e.InnerException?.Message,
                              StackTrace = e.StackTrace,
                              SystemProcessId = request.SystemProcessId,
                              SystemProcessOperationId = request.SystemProcessOperationId,
                              SystemProcessOperationThirdPartyDataRequestId = request.Id
                          };

            this._exceptionRepository.Save(dto);
        }

        private void Log(Exception e)
        {
            var dto = new ExceptionDto
                          {
                              ExceptionMessage = e.Message,
                              InnerExceptionMessage = e.InnerException?.Message,
                              StackTrace = e.StackTrace
                          };

            this._exceptionRepository.Save(dto);
        }
    }
}