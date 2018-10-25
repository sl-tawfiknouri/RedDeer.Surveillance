using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.Rules.HighVolume.Interfaces
{
    public interface IHighVolumeMessageSender
    {
        void Send(IHighVolumeRuleBreach ruleBreach, ISystemProcessOperationRunRuleContext ruleCtx);
    }
}