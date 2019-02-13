using System.Threading.Tasks;

namespace Surveillance.Engine.Rules.Rules.HighProfits.Interfaces
{
    public interface IHighProfitMessageSender
    {
        Task Send(IHighProfitRuleBreach ruleBreach);
    }
}