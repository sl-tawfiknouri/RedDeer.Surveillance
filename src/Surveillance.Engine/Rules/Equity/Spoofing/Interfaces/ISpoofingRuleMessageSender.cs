using System.Threading.Tasks;

namespace Surveillance.Engine.Rules.Rules.Equity.Spoofing.Interfaces
{
    public interface ISpoofingRuleMessageSender
    {
        Task Send(ISpoofingRuleBreach ruleBreach);
    }
}