using System;
using System.Threading.Tasks;
using DomainV2.Scheduling;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;
using Surveillance.DataLayer.Api.RuleParameter.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.Universe.Interfaces;
using Surveillance.Analytics.Streams.Interfaces;
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

            var highProfitSubscriptions = _highProfitSubscriber.CollateSubscriptions(execution, ruleParameters, opCtx, alertStream);
            var highVolumeSubscriptions = _highVolumeSubscriber.CollateSubscriptions(execution, ruleParameters, opCtx, alertStream);
            var washTradeSubscriptions = _washTradeSubscriber.CollateSubscriptions(execution, ruleParameters, opCtx, alertStream);

            foreach (var sub in highProfitSubscriptions)
                player.Subscribe(sub);

            foreach (var sub in highVolumeSubscriptions)
                player.Subscribe(sub);


            foreach (var sub in washTradeSubscriptions)
                player.Subscribe(sub);

             // _RegisterNotSupportedSubscriptions(ruleParameters, execution, player, alertStream, opCtx);
        }

        private void _RegisterNotSupportedSubscriptions(
            RuleParameterDto ruleParameters,
            ScheduledExecution execution,
            IUniversePlayer player,
            IUniverseAlertStream alertStream,
            ISystemProcessOperationContext opCtx)
        {
            var spoofingSubscriptions = _spoofingSubscriber.CollateSubscriptions(execution, ruleParameters, opCtx, alertStream);
            var cancelledSubscriptions = _cancelledOrderSubscriber.CollateSubscriptions(execution, ruleParameters, opCtx, alertStream);
            var markingTheCloseSubscriptions = _markingTheCloseSubscriber.CollateSubscriptions(execution, ruleParameters, opCtx, alertStream);
            var layeringSubscriptions = _layeringSubscriber.CollateSubscriptions(execution, ruleParameters, opCtx, alertStream);

            foreach (var sub in spoofingSubscriptions)
                player.Subscribe(sub);

            foreach (var sub in cancelledSubscriptions)
                player.Subscribe(sub);

            foreach (var sub in markingTheCloseSubscriptions)
                player.Subscribe(sub);

            foreach (var sub in layeringSubscriptions)
                player.Subscribe(sub);
        }
    }
}