using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using RedDeer.Surveillance.App.Interfaces;
using Surveillance.Interfaces;

namespace RedDeer.Surveillance.App
{
    public class MediatorBootstrapper : IStartUpTaskRunner
    {
        private readonly IMediator _mediator;
        private readonly ILogger<MediatorBootstrapper> _logger;

        public MediatorBootstrapper(
            IMediator mediator,
            ILogger<MediatorBootstrapper> logger
            )
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
                    _logger.LogInformation($"MediatorBootstrapper bootstrapping the mediator");
                    _mediator.Initiate();
                    _logger.LogInformation($"MediatorBootstrapper completed bootstrapping the mediator");
                }
                catch (Exception e)
                {
                    _logger.LogCritical("Critical error bubbled to mediator bootstrapper in surveillance", e);
                    throw;
                }
            });
        }
    }
}
