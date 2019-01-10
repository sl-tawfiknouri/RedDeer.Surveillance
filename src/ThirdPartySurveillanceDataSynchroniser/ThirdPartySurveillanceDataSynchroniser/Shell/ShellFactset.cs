using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Api;
using Surveillance.DataLayer.Configuration.Interfaces;
using ThirdPartySurveillanceDataSynchroniser.Shell.Interfaces;

namespace ThirdPartySurveillanceDataSynchroniser.Shell
{
    public class ShellFactset : BaseApiRepository, IShellFactset
    {
        private const string HeartbeatRoute = "api/factset/heartbeat";
        private readonly ILogger<ShellFactset> _logger;

        public ShellFactset(
            IDataLayerConfiguration dataLayerConfiguration,
            ILogger<ShellFactset> logger)
            : base(dataLayerConfiguration, logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> HeartBeating(CancellationToken token)
        {
            try
            {
                var httpClient = BuildHttpClient();

                var response = await httpClient.GetAsync(HeartbeatRoute, token);

                if (!response.IsSuccessStatusCode)
                    _logger.LogError($"HEARTBEAT FOR FACTSET TIME BAR DATA API REPOSITORY NEGATIVE");
                else
                    _logger.LogInformation($"HEARTBEAT POSITIVE FOR FACTSET TIME BAR API REPOSITORY");

                return response.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                _logger.LogError($"HEARTBEAT FOR FACTSET TIME BAR API REPOSITORY NEGATIVE", e);
            }

            return false;
        }
    }
}
