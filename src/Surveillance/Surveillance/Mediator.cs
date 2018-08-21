using Surveillance.Services;
using System;

namespace Surveillance
{
    /// <summary>
    /// The mediator orchestrates program components; factory; services; display
    /// </summary>
    public class Mediator : IMediator
    {
        private IReddeerTradeService _reddeerTradeService;

        public Mediator(IReddeerTradeService reddeerTradeService)
        {
            _reddeerTradeService =
                reddeerTradeService 
                ?? throw new ArgumentNullException(nameof(reddeerTradeService));
        }

        public void Initiate()
        {
            _reddeerTradeService.Initialise();
        }

        public void Terminate()
        {
            _reddeerTradeService.Dispose();
        }
    }
}
