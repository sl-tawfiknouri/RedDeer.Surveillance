using System;
using System.Threading.Tasks;
using Domain.Scheduling;
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

            var spoofingSubscriptions = _spoofingSubscriber.CollateSubscriptions(execution, ruleParameters, opCtx, alertStream);

            var cancelledSubscriptions = _cancelledOrderSubscriber.CollateSubscriptions(execution, ruleParameters, opCtx, alertStream);

            var highProfitSubscriptions = _highProfitSubscriber.CollateSubscriptions(execution, ruleParameters, opCtx, alertStream);

            var highVolumeSubscriptions = _highVolumeSubscriber.CollateSubscriptions(execution, ruleParameters, opCtx, alertStream);

            var markingTheCloseSubscriptions = _markingTheCloseSubscriber.CollateSubscriptions(execution, ruleParameters, opCtx, alertStream);

            var layeringSubscriptions = _layeringSubscriber.CollateSubscriptions(execution, ruleParameters, opCtx, alertStream);

            var washTradeSubscriptions = _washTradeSubscriber.CollateSubscriptions(execution, ruleParameters, opCtx, alertStream);
        }
    }
}