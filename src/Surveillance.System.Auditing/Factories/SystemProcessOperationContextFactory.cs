namespace Surveillance.Auditing.Factories
{
    using System;

    using Surveillance.Auditing.Context;
    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Auditing.DataLayer.Repositories.Interfaces;
    using Surveillance.Auditing.Factories.Interfaces;
    using Surveillance.Auditing.Logging.Interfaces;

    public class SystemProcessOperationContextFactory : ISystemProcessOperationContextFactory
    {
        private readonly ISystemProcessOperationDataRequestContextFactory _dataRequestFactory;

        private readonly ISystemProcessOperationDistributeRuleContextFactory _distributeRuleFactory;

        private readonly ISystemProcessOperationFileUploadContextFactory _fileUploadFactory;

        private readonly IOperationLogging _operationLogging;

        private readonly ISystemProcessOperationRepository _operationRepository;

        private readonly ISystemProcessOperationRunRuleContextFactory _ruleRunFactory;

        public SystemProcessOperationContextFactory(
            ISystemProcessOperationRepository operationContext,
            ISystemProcessOperationRunRuleContextFactory ruleRunRepository,
            ISystemProcessOperationDistributeRuleContextFactory distributeRuleFactory,
            IOperationLogging operationLogging,
            ISystemProcessOperationFileUploadContextFactory fileUploadFactory,
            ISystemProcessOperationDataRequestContextFactory dataRequestFactory)
        {
            this._operationRepository = operationContext ?? throw new ArgumentNullException(nameof(operationContext));
            this._ruleRunFactory = ruleRunRepository ?? throw new ArgumentNullException(nameof(ruleRunRepository));
            this._distributeRuleFactory =
                distributeRuleFactory ?? throw new ArgumentNullException(nameof(distributeRuleFactory));
            this._operationLogging = operationLogging ?? throw new ArgumentNullException(nameof(operationLogging));
            this._fileUploadFactory = fileUploadFactory ?? throw new ArgumentNullException(nameof(fileUploadFactory));
            this._dataRequestFactory =
                dataRequestFactory ?? throw new ArgumentNullException(nameof(dataRequestFactory));
        }

        public ISystemProcessOperationContext Build(ISystemProcessContext context)
        {
            return new SystemProcessOperationContext(
                context,
                this._operationRepository,
                this._ruleRunFactory,
                this._distributeRuleFactory,
                this._fileUploadFactory,
                this._dataRequestFactory,
                this._operationLogging);
        }
    }
}