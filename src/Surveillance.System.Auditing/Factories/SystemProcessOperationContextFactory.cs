using System;
using Surveillance.Auditing.Context;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Auditing.DataLayer.Repositories.Interfaces;
using Surveillance.Auditing.Factories.Interfaces;
using Surveillance.Auditing.Logging.Interfaces;

namespace Surveillance.Auditing.Factories
{
    public class SystemProcessOperationContextFactory : ISystemProcessOperationContextFactory
    {
        private readonly ISystemProcessOperationRepository _operationRepository;
        private readonly ISystemProcessOperationRunRuleContextFactory _ruleRunFactory;
        private readonly ISystemProcessOperationDistributeRuleContextFactory _distributeRuleFactory;
        private readonly ISystemProcessOperationFileUploadContextFactory _fileUploadFactory;
        private readonly ISystemProcessOperationDataRequestContextFactory _dataRequestFactory;
        private readonly IOperationLogging _operationLogging;

        public SystemProcessOperationContextFactory(
            ISystemProcessOperationRepository operationContext,
            ISystemProcessOperationRunRuleContextFactory ruleRunRepository,
            ISystemProcessOperationDistributeRuleContextFactory distributeRuleFactory,
            IOperationLogging operationLogging,
            ISystemProcessOperationFileUploadContextFactory fileUploadFactory,
            ISystemProcessOperationDataRequestContextFactory dataRequestFactory)
        {
            _operationRepository = operationContext ?? throw new ArgumentNullException(nameof(operationContext));
            _ruleRunFactory = ruleRunRepository ?? throw new ArgumentNullException(nameof(ruleRunRepository));
            _distributeRuleFactory = distributeRuleFactory ?? throw new ArgumentNullException(nameof(distributeRuleFactory));
            _operationLogging = operationLogging ?? throw new ArgumentNullException(nameof(operationLogging));
            _fileUploadFactory = fileUploadFactory ?? throw new ArgumentNullException(nameof(fileUploadFactory));
            _dataRequestFactory = dataRequestFactory ?? throw new ArgumentNullException(nameof(dataRequestFactory));
        }

        public ISystemProcessOperationContext Build(ISystemProcessContext context)
        {
            return new SystemProcessOperationContext(
                context,
                _operationRepository,
                _ruleRunFactory,
                _distributeRuleFactory,
                _fileUploadFactory,
                _dataRequestFactory,
                _operationLogging);
        }
    }
}