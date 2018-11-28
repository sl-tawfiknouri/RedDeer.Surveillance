﻿using System;
using System.Threading.Tasks;
using Domain.Scheduling;
using Surveillance.DataLayer.Api.RuleParameter.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.Universe.Interfaces;
using Surveillance.Analytics.Streams.Interfaces;
using Surveillance.Universe.OrganisationalFactors;
using Surveillance.Universe.Subscribers.Interfaces;

namespace Surveillance.Universe
{
    public class UniverseRuleSubscriber : IUniverseRuleSubscriber
    {
        private readonly ISpoofingSubscriber _spoofingSubscriber;
        private readonly ICancelledOrderSubscriber _cancelledOrderSubscriber;
        private readonly IHighProfitsSubscriber _highProfitSubscriber;
        private readonly IHighVolumeSubscriber _highVolumeSubscriber;
        private readonly IMarkingTheCloseSubscriber _markingTheCloseSubscriber;
        private readonly ILayeringSubscriber _layeringSubscriber;
        private readonly IWashTradeSubscriber _washTradeSubscriber;

        private readonly IRuleParameterApiRepository _ruleParameterApiRepository;

        public UniverseRuleSubscriber(
            ISpoofingSubscriber spoofingSubscriber,
            ICancelledOrderSubscriber cancelledOrderSubscriber,
            IHighProfitsSubscriber highProfitSubscriber,
            IHighVolumeSubscriber highVolumeSubscriber,
            IMarkingTheCloseSubscriber markingTheCloseSubscriber,
            ILayeringSubscriber layeringSubscriber,
            IWashTradeSubscriber washTradeSubscriber,
            IRuleParameterApiRepository ruleParameterApiRepository)
        {
            _spoofingSubscriber = spoofingSubscriber ?? throw new ArgumentNullException(nameof(spoofingSubscriber));
            _cancelledOrderSubscriber = cancelledOrderSubscriber ?? throw new ArgumentNullException(nameof(cancelledOrderSubscriber));
            _highProfitSubscriber = highProfitSubscriber ?? throw new ArgumentNullException(nameof(highProfitSubscriber));
            _highVolumeSubscriber = highVolumeSubscriber ?? throw new ArgumentNullException(nameof(highVolumeSubscriber));
            _markingTheCloseSubscriber = markingTheCloseSubscriber ?? throw new ArgumentNullException(nameof(markingTheCloseSubscriber));
            _layeringSubscriber = layeringSubscriber ?? throw new ArgumentNullException(nameof(layeringSubscriber));
            _washTradeSubscriber = washTradeSubscriber ?? throw new ArgumentNullException(nameof(washTradeSubscriber));

            _ruleParameterApiRepository = ruleParameterApiRepository
                ?? throw new ArgumentNullException(nameof(ruleParameterApiRepository));
        }

        public async Task SubscribeRules(
             ScheduledExecution execution,
             IUniversePlayer player,
             IUniverseAlertStream alertStream,
             ISystemProcessOperationContext opCtx)
        {
            if (execution == null
                || player == null)
            {
                return;
            }

            var ruleParameters = await _ruleParameterApiRepository.Get();

            var test = new OrganisationalFactorBroker(null, null, true, null);


            
            // ok so the best way to handle this is to pass back the subscription requests from the new objects
            // then we can wrap them in here
            // it could just be get rule parameters for obj, check if has org factors or not =)
            // sure but then we have a new switch to get the rule org factors back out
            // just stick it on the subscriber
            // or we could just pass the broker into the subscriber ?
            // would be nice to have it inside of the subscriber but I think it gives us a lot of complex objects
            // lets cut across the beam a bit...

            _spoofingSubscriber.SpoofingRule(execution, player, ruleParameters, opCtx, alertStream);
            _cancelledOrderSubscriber.CancelledOrdersRule(execution, player, ruleParameters, opCtx, alertStream);
            _highProfitSubscriber.HighProfitsRule(execution, player, ruleParameters, opCtx, alertStream);
            _highVolumeSubscriber.HighVolumeRule(execution, player, ruleParameters, opCtx, alertStream);
            _markingTheCloseSubscriber.MarkingTheCloseRule(execution, player, ruleParameters, opCtx, alertStream);
            _layeringSubscriber.LayeringRule(execution, player, ruleParameters, opCtx, alertStream);
            _washTradeSubscriber.WashTradeRule(execution, player, ruleParameters, opCtx, alertStream);
        }
    }
}