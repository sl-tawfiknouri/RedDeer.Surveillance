using System;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.System.DataLayer.Processes.Interfaces;

namespace Surveillance.System.Auditing.Context
{
    public class SystemProcessOperationFileUploadContext : ISystemProcessOperationFileUploadContext
    {
        private readonly ISystemProcessOperationContext _fileUploadContext;
        private ISystemProcessOperationFileUpload _fileUpload;

        public SystemProcessOperationFileUploadContext(ISystemProcessOperationContext fileUploadContext)
        {
            _fileUploadContext = fileUploadContext ?? throw new ArgumentNullException(nameof(fileUploadContext));
        }

        public void StartEvent(ISystemProcessOperationFileUpload fileUpload)
        {
            _fileUpload = fileUpload;
        }

        public ISystemProcessOperationContext EndEvent()
        {
            return _fileUploadContext;
        }
    }
}
