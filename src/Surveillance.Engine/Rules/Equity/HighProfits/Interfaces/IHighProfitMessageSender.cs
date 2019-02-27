using System.Threading.Tasks;

namespace Surveillance.Engine.Rules.Rules.Equity.HighProfits.Interfaces
{
    public interface IHighProfitMessageSender
    {
        Task Send(IHighProfitRuleBreach ruleBreach);
    }
}