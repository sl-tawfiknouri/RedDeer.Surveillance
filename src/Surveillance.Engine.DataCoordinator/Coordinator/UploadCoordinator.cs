using System;
using Surveillance.Engine.DataCoordinator.Configuration.Interfaces;
using Surveillance.Engine.DataCoordinator.Coordinator.Interfaces;

namespace Surveillance.Engine.DataCoordinator.Coordinator
{
    public class UploadCoordinator : IUploadCoordinator
    {
        private readonly IRuleConfiguration _ruleConfiguration;

        public UploadCoordinator(IRuleConfiguration ruleConfiguration)
        {
            _ruleConfiguration = ruleConfiguration ?? throw new ArgumentNullException(nameof(ruleConfiguration));
        }

        public void AnalyseFileId()
        {

        }
    }
}
