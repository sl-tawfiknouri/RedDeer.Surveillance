using Surveillance.Services.Interfaces;
using System;
using Surveillance.Interfaces;
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
        private readonly IReddeerSmartRuleScheduler _smartRuleScheduler;

        public Mediator(
            IReddeerTradeService reddeerTradeService,
            IReddeerRuleScheduler ruleScheduler,
            IReddeerSmartRuleScheduler smartRuleScheduler)
        {
            _reddeerTradeService =
                reddeerTradeService 
                ?? throw new ArgumentNullException(nameof(reddeerTradeService));
            _ruleScheduler =
                ruleScheduler
                ?? throw new ArgumentNullException(nameof(ruleScheduler));
            _smartRuleScheduler =
                smartRuleScheduler
                ?? throw new ArgumentNullException(nameof(smartRuleScheduler));
        }

        public void Initiate()
        {
            _ruleScheduler.Initiate();
            _smartRuleScheduler.Initiate();
            _reddeerTradeService.Initialise();
        }

        public void Terminate()
        {
            _reddeerTradeService.Dispose();
            _ruleScheduler.Terminate();
            _smartRuleScheduler.Terminate();
        }
    }
}
