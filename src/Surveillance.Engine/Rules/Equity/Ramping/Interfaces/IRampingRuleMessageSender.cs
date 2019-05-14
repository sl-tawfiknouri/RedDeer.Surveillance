using System.Threading.Tasks;

namespace Surveillance.Engine.Rules.Rules.Equity.Ramping.Interfaces
{
    public interface IRampingRuleMessageSender
    {
        Task Send(IRampingRuleBreach breach);
        int Flush();
    }
}
