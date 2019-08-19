namespace Surveillance.Engine.Rules.Rules.Equity.Ramping.Interfaces
{
    using System.Threading.Tasks;

    public interface IRampingRuleMessageSender
    {
        int Flush();

        Task Send(IRampingRuleBreach breach);
    }
}