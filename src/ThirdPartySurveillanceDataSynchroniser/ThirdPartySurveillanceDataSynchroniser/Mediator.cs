using System.Diagnostics;
using System.Threading;
using Microsoft.Extensions.Logging;
using ThirdPartySurveillanceDataSynchroniser.Interfaces;

namespace ThirdPartySurveillanceDataSynchroniser
{
    public class Mediator : IMediator
    {
        private readonly ILogger<Mediator> _logger;

        public Mediator(ILogger<Mediator> logger)
        {
            _logger = logger;
        }

        public void Initiate()
        {
            while (true)
            {
                _logger.LogInformation($"   D A T A   S Y N C H R O N I S E R  | process-id {Process.GetCurrentProcess().Id} | started-at {Process.GetCurrentProcess().StartTime}");

                Thread.Sleep(15 * 1000);
            }
        }
    }
}
