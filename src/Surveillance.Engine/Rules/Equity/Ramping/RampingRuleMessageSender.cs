using System.Threading.Tasks;
using Surveillance.Engine.Rules.Rules.Equity.Ramping.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Equity.Ramping
{
    public class RampingRuleMessageSender : IRampingRuleMessageSender
    {
        public async Task Send(IRampingRuleBreach breach)
        {
            
        }

        public int Flush()
        {
            return -1;
        }
    }
}
