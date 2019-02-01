using System;
using Surveillance.Systems.Auditing.Context.Interfaces;
using Surveillance.Systems.Auditing.Logging.Interfaces;
using Surveillance.Systems.DataLayer.Processes.Interfaces;
using Surveillance.Systems.DataLayer.Repositories.Interfaces;

namespace Surveillance.Systems.Auditing.Context
{
    public class SystemProcessOperationUploadFileContext : ISystemProcessOperationUploadFileContext
    {
        private readonly ISystemProcessOperationUploadFileRepository _repository;
        private readonly ISystemProcessOperationContext _processOperationContext;
        private ISystemProcessOperationUploadFile _fileUpload;
        private readonly IOperationLogging _operationLogging;

        public SystemProcessOperationUploadFileContext(
            ISystemProcessOperationContext processOperationContext,
            IOperationLogging operationLogging,
            ISystemProcessOperationUploadFileRepository repository)
        {
            _processOperationContext =
                processOperationContext
                ?? throw new ArgumentNullException(nameof(processOperationContext));

            _operationLogging =
                operationLogging
                ?? throw new ArgumentNullException(nameof(operationLogging));

            _repository =
                repository
                ?? throw new ArgumentNullException(nameof(repository));
        }

        public void StartEvent(ISystemProcessOperationUploadFile upload)
        {
            if (upload == null)
            {
                return;
            }

            _fileUpload = upload;
            _repository.Create(upload);
        }

        public void EventException(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            _operationLogging.Log(new Exception(message), _fileUpload);
        }

        public void EventException(Exception e)
        {
            _operationLogging.Log(e, _fileUpload);
        }

        public ISystemProcessOperationContext EndEvent()
        {
            return _processOperationContext;
        }
    }
}
