using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Api;
using Surveillance.DataLayer.Configuration.Interfaces;
using ThirdPartySurveillanceDataSynchroniser.Shell.Interfaces;

namespace ThirdPartySurveillanceDataSynchroniser.Shell
{
    public class ShellBmll : BaseApiRepository, IShellBmll
    {
        private const string HeartbeatRoute = "api/bmll/heartbeat";
        private readonly ILogger<ShellBmll> _logger;

        public ShellBmll(
            IDataLayerConfiguration dataLayerConfiguration,
            ILogger<ShellBmll> logger)
            : base(dataLayerConfiguration, logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task<bool> HeartBeating(CancellationToken token)
        {
            try
            {
                var httpClient = BuildBmllHttpClient();

                var response = await httpClient.GetAsync(HeartbeatRoute, token);

                if (!response.IsSuccessStatusCode)
                    _logger.LogError($"HEARTBEAT FOR BMLL TIME BAR DATA API REPOSITORY NEGATIVE");
                else
                    _logger.LogInformation($"HEARTBEAT POSITIVE FOR TIME BAR API REPOSITORY");

                return response.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                _logger.LogError($"HEARTBEAT FOR TIME BAR API REPOSITORY NEGATIVE", e);
            }

            return false;
        }
    }
}
