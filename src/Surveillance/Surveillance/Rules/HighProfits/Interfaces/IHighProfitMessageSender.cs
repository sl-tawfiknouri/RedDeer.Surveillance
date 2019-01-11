using System.Threading.Tasks;

namespace Surveillance.Rules.HighProfits.Interfaces
{
    public interface IHighProfitMessageSender
    {
        Task Send(IHighProfitRuleBreach ruleBreach);
    }
}