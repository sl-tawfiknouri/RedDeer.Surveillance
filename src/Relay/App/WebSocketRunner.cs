using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Relay.Interfaces;

namespace RedDeer.DataImport.DataImport.App
{
    public class WebSocketRunner : IStartUpTaskRunner
    {
        readonly IMediator _mediator;
        private readonly ILogger _logger;

        public WebSocketRunner(
            IMediator mediator,
            ILogger<WebSocketRunner> logger)
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
                    _mediator.Initiate();
                }
                catch (Exception e)
                {
                    _logger.LogCritical("A critical error bubbled to web socket runner in relay app", e);
                    throw;
                }
            });
        }
    }
}