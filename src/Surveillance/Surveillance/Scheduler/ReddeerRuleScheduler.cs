using System;
using Surveillance.Factories.Interfaces;
using Surveillance.Scheduler.Interfaces;
using Surveillance.Universe.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace Surveillance.Scheduler
{
    public class ReddeerRuleScheduler : IReddeerRuleScheduler
    {
        private readonly ISpoofingRuleFactory _spoofingRuleFactory;
        private readonly IUniverseBuilder _universeBuilder;
        private readonly IUniversePlayerFactory _universePlayerFactory;

        public ReddeerRuleScheduler(
            ISpoofingRuleFactory spoofingRuleFactory,
            IUniverseBuilder universeBuilder,
            IUniversePlayerFactory universePlayerFactory)
        {
            _spoofingRuleFactory = 
                spoofingRuleFactory
                ?? throw new ArgumentNullException(nameof(spoofingRuleFactory));

            _universeBuilder = universeBuilder ?? throw new ArgumentNullException(nameof(universeBuilder));

            _universePlayerFactory =
                universePlayerFactory
                ?? throw new ArgumentNullException(nameof(universePlayerFactory));
        }
       
        public void Initiate()
        {
            // begin subscription to message bus
        }

        public void Terminate()
        {
            // end subscription to the message bus
        }

        /// <summary>
        /// Once a message is picked up, deserialise the scheduled execution object
        /// and run execute
        /// </summary>
        public async Task Execute(ScheduledExecution execution)
        {
            if (execution?.Rules == null
                || !execution.Rules.Any())
            {
                return;
            }

            var universe = await _universeBuilder.Summon(execution);
            var player = _universePlayerFactory.Build();

            SubscribeRules(execution, player);
            player.Play(universe);
        }

        private void SubscribeRules(
            ScheduledExecution execution,
            IUniversePlayer player)
        {
            if (execution == null
                || player == null)
            {
                return;
            }

            if (execution.Rules.Contains(Rules.Rules.Spoofing))
            {
                var spoofingRule = _spoofingRuleFactory.Build();
                player.Subscribe(spoofingRule);
            }
        }
    }
}
