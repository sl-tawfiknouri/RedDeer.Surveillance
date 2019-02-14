using System;
using DomainV2.Contracts;
using Microsoft.Extensions.Logging;
using Surveillance.Engine.DataCoordinator.Configuration.Interfaces;
using Surveillance.Engine.DataCoordinator.Coordinator.Interfaces;

namespace Surveillance.Engine.DataCoordinator.Coordinator
{
    public class UploadCoordinator : IUploadCoordinator
    {
        private readonly IRuleConfiguration _ruleConfiguration;
        private readonly ILogger<UploadCoordinator> _logger;

        public UploadCoordinator(
            IRuleConfiguration ruleConfiguration,
            ILogger<UploadCoordinator> logger)
        {
            _ruleConfiguration = ruleConfiguration ?? throw new ArgumentNullException(nameof(ruleConfiguration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void AnalyseFileId(UploadCoordinatorMessage message)
        {
            if (message == null
                || string.IsNullOrWhiteSpace(message.FileId))
            {
                _logger.LogInformation($"UploadCoordinator AnalyseFileId received a null message. Exiting.");
                return;
            }

            if (_ruleConfiguration.AutoScheduleRules == null
                || !_ruleConfiguration.AutoScheduleRules.GetValueOrDefault(false))
            {
                _logger.LogInformation($"UploadCoordinator AnalyseFileId received a message with a file id of {message.FileId} but auto scheduling was turned off. Exiting.");
                return;
            }

            
        }
    }
}
