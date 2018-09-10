using Surveillance.Services.Interfaces;
using System;
using Surveillance.Scheduler.Interfaces;

namespace Surveillance
{
    /// <summary>
    /// The mediator orchestrates program components; factory; services; display
    /// </summary>
    public class Mediator : IMediator
    {
        private readonly IReddeerTradeService _reddeerTradeService;
        private readonly IReddeerRuleScheduler _ruleScheduler;

        public Mediator(
            IReddeerTradeService reddeerTradeService,
            IReddeerRuleScheduler ruleScheduler)
        {
            _reddeerTradeService =
                reddeerTradeService 
                ?? throw new ArgumentNullException(nameof(reddeerTradeService));
            _ruleScheduler =
                ruleScheduler
                ?? throw new ArgumentNullException(nameof(ruleScheduler));
        }

        public void Initiate()
        {
            _ruleScheduler.Initiate();
            _reddeerTradeService.Initialise();
        }

        public void Terminate()
        {
            _reddeerTradeService.Dispose();
            _ruleScheduler.Terminate();
        }
    }
}
