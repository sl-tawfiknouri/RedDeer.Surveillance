using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Surveillance.Engine.DataCoordinator.Configuration.Interfaces;
using Surveillance.Engine.DataCoordinator.Coordinator.Interfaces;

namespace Surveillance.Engine.DataCoordinator.Coordinator
{
    public class DataVerifier : IDataVerifier
    {
        private readonly IRuleConfiguration _ruleConfiguration;
        private readonly ILogger<DataVerifier> _logger;

        public DataVerifier(
            IRuleConfiguration ruleConfiguration,
            ILogger<DataVerifier> logger)
        {
            _ruleConfiguration = ruleConfiguration ?? throw new ArgumentNullException(nameof(ruleConfiguration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Scan()
        {
            if (_ruleConfiguration.AutoScheduleRules == null
                || !_ruleConfiguration.AutoScheduleRules.GetValueOrDefault(false))
            {
                _logger.LogInformation($"UploadCoordinator AnalyseFileId received a message to auto schedule but auto scheduling was turned off. Exiting.");
                return;
            }

            // bit complex leave to last?


         }
    }
}
