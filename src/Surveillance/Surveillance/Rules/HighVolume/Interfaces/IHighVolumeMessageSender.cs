using System.Threading.Tasks;
using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.Rules.HighVolume.Interfaces
{
    public interface IHighVolumeMessageSender
    {
        Task Send(IHighVolumeRuleBreach ruleBreach, ISystemProcessOperationRunRuleContext ruleCtx);
    }
}