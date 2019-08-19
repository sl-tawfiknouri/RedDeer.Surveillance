namespace DataSynchroniser.App
{
    using System;
    using System.Threading.Tasks;

    using DataSynchroniser.Interfaces;

    using Microsoft.Extensions.Logging;

    public class DataSynchroniserRunner : IStartUpTaskRunner
    {
        private readonly ILogger _logger;

        private readonly IMediator _mediator;

        public DataSynchroniserRunner(IMediator mediator, ILogger<DataSynchroniserRunner> logger)
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
                            this._logger.LogInformation("StartUpTaskRunner initiating mediator");
                            this._mediator.Initiate();
                            this._logger.LogInformation("StartUpTaskRunner initiated mediator");
                        }
                        catch (Exception e)
                        {
                            this._logger.LogCritical(
                                "A critical error bubbled to data synchroniser runner in third party surveillance data synchroniser app",
                                e);
                            throw;
                        }
                    });
        }
    }
}