using System;
using System.Threading.Tasks;
using DataSynchroniser.Interfaces;
using Microsoft.Extensions.Logging;

namespace DataSynchroniser.App
{
    public class DataSynchroniserRunner : IStartUpTaskRunner
    {
        private readonly IMediator _mediator;
        private readonly ILogger _logger;

        public DataSynchroniserRunner(
            IMediator mediator,
            ILogger<DataSynchroniserRunner> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Run()
        {
            await Task.Run(() =>
            {
                try
                {
                    _logger.LogInformation($"StartUpTaskRunner initiating mediator");
                    _mediator.Initiate();
                    _logger.LogInformation($"StartUpTaskRunner initiated mediator");
                }
                catch (Exception e)
                {
                    _logger.LogCritical("A critical error bubbled to data synchroniser runner in third party surveillance data synchroniser app", e);
                    throw;
                }
            });
        }
    }
}