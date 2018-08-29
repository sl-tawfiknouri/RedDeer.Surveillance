using Surveillance.Services.Interfaces;
using System;

namespace Surveillance
{
    /// <summary>
    /// The mediator orchestrates program components; factory; services; display
    /// </summary>
    public class Mediator : IMediator
    {
        private IReddeerTradeService _reddeerTradeService;
        private IReddeerEquityService _reddeerEquityService;

        public Mediator(
            IReddeerTradeService reddeerTradeService,
            IReddeerEquityService reddeerEquityService)
        {
            _reddeerTradeService =
                reddeerTradeService 
                ?? throw new ArgumentNullException(nameof(reddeerTradeService));

            _reddeerEquityService =
                reddeerEquityService
                ?? throw new ArgumentNullException(nameof(reddeerEquityService));
        }

        public void Initiate()
        {
            _reddeerTradeService.Initialise();
            _reddeerEquityService.Initialise();
        }

        public void Terminate()
        {
            _reddeerTradeService.Dispose();
            _reddeerEquityService.Dispose();
        }
    }
}
