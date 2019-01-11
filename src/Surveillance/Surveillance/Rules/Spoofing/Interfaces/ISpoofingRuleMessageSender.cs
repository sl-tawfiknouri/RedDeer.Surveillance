using System.Threading.Tasks;

namespace Surveillance.Rules.Spoofing.Interfaces
{
    public interface ISpoofingRuleMessageSender
    {
        Task Send(ISpoofingRuleBreach ruleBreach);
    }
}