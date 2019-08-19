namespace Surveillance.Auditing.Context
{
    using System;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Auditing.DataLayer.Processes.Interfaces;
    using Surveillance.Auditing.DataLayer.Repositories.Interfaces;
    using Surveillance.Auditing.Logging.Interfaces;

    public class SystemProcessOperationUploadFileContext : ISystemProcessOperationUploadFileContext
    {
        private readonly IOperationLogging _operationLogging;

        private readonly ISystemProcessOperationContext _processOperationContext;

        private readonly ISystemProcessOperationUploadFileRepository _repository;

        public SystemProcessOperationUploadFileContext(
            ISystemProcessOperationContext processOperationContext,
            IOperationLogging operationLogging,
            ISystemProcessOperationUploadFileRepository repository)
        {
            this._processOperationContext = processOperationContext
                                            ?? throw new ArgumentNullException(nameof(processOperationContext));

            this._operationLogging = operationLogging ?? throw new ArgumentNullException(nameof(operationLogging));

            this._repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public ISystemProcessOperationUploadFile FileUpload { get; private set; }

        public ISystemProcessOperationContext EndEvent()
        {
            return this._processOperationContext;
        }

        public void EventException(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;

            this._operationLogging.Log(new Exception(message), this.FileUpload);
        }

        public void EventException(Exception e)
        {
            this._operationLogging.Log(e, this.FileUpload);
        }

        public void StartEvent(ISystemProcessOperationUploadFile upload)
        {
            if (upload == null) return;

            this.FileUpload = upload;
            this._repository.Create(upload).Wait();
        }
    }
}