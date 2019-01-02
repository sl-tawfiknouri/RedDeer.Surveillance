using System;
using System.Threading.Tasks;
using DataImport.Interfaces;
using Microsoft.Extensions.Logging;

namespace RedDeer.DataImport.DataImport.App
{
    public class DataImportRunner : IStartUpTaskRunner
    {
        readonly IMediator _mediator;
        private readonly ILogger _logger;

        public DataImportRunner(
            IMediator mediator,
            ILogger<DataImportRunner> logger)
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
                    _logger.LogCritical("A critical error bubbled to web socket runner in data import app", e);
                    throw;
                }
            });
        }
    }
}