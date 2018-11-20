using Surveillance.Rules.HighVolume.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.Rules.HighVolume
{
    public interface IHighVolumeRuleCachedMessageSender
    {
        void Send(IHighVolumeRuleBreach ruleBreach);
        int Flush(ISystemProcessOperationRunRuleContext ruleCtx);
    }
}