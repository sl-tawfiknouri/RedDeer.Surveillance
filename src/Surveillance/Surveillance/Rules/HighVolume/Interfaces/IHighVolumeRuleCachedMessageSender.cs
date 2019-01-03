using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.Rules.HighVolume.Interfaces
{
    public interface IHighVolumeRuleCachedMessageSender
    {
        void Send(IHighVolumeRuleBreach ruleBreach);
        int Flush(ISystemProcessOperationRunRuleContext ruleCtx);
        void Delete();
    }
}