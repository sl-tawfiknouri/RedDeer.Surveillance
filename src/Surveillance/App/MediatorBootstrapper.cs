using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using RedDeer.Surveillance.App.Interfaces;
using RedDeer.Surveillance.App.ScriptRunner.Interfaces;
using Surveillance.Interfaces;

namespace RedDeer.Surveillance.App
{
    public class MediatorBootstrapper : IStartUpTaskRunner
    {
        private readonly IMediator _mediator;
        private readonly IScriptRunner _scriptRunner;
        private readonly ILogger<MediatorBootstrapper> _logger;

        public MediatorBootstrapper(
            IMediator mediator,
            IScriptRunner scriptRunner,
            ILogger<MediatorBootstrapper> logger
            )
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _scriptRunner = scriptRunner ?? throw new ArgumentNullException(nameof(scriptRunner));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Run()
        {
            await Task.Run(() => 
            {
                // trades on 69, stocks on 70 [ports]
                try
                {
                    var runner = _scriptRunner.Run();
                    runner.Wait();
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
