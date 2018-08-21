using Microsoft.Extensions.Logging;
using Surveillance;
using System;
using System.Threading.Tasks;

namespace RedDeer.Surveillance.App
{
    public class MediatorBootstrapper : IStartUpTaskRunner
    {
        private IMediator _mediator;
        private ILogger<MediatorBootstrapper> _logger;

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
                // trades on 69, stocks on 70 [ports]
                try
                {
                    _mediator.Initiate();
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
