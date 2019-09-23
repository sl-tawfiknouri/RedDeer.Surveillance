namespace RedDeer.Surveillance.App
{
    using System;
    using System.Threading.Tasks;

    using global::Surveillance.Interfaces;

    using Microsoft.Extensions.Logging;

    using RedDeer.Surveillance.App.Interfaces;

    public class MediatorBootstrapper : IStartUpTaskRunner
    {
        private readonly ILogger<MediatorBootstrapper> _logger;

        private readonly IMediator _mediator;

        public MediatorBootstrapper(IMediator mediator, ILogger<MediatorBootstrapper> logger)
        {
            this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Run()
        {
            await Task.Run(
                () =>
                    {
                        try
                        {
                            this._logger.LogInformation("MediatorBootstrapper bootstrapping the mediator");
                            this._mediator.Initiate();
                            this._logger.LogInformation("MediatorBootstrapper completed bootstrapping the mediator");
                        }
                        catch (Exception e)
                        {
                            this._logger.LogCritical(
                                "Critical error bubbled to mediator bootstrapper in surveillance",
                                e);
                            throw;
                        }
                    });
        }
    }
}