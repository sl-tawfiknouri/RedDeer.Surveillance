using System.Threading.Tasks;

namespace Surveillance.Engine.Rules.Rules.Spoofing.Interfaces
{
    public interface ISpoofingRuleMessageSender
    {
        Task Send(ISpoofingRuleBreach ruleBreach);
    }
}