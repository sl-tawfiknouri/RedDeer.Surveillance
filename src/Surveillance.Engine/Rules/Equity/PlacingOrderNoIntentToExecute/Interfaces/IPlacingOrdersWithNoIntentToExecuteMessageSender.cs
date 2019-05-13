using System.Threading.Tasks;

namespace Surveillance.Engine.Rules.Rules.Equity.PlacingOrderNoIntentToExecute.Interfaces
{
    public interface IPlacingOrdersWithNoIntentToExecuteMessageSender
    {
        Task Send(IPlacingOrdersWithNoIntentToExecuteRuleBreach ruleBreach);
    }
}
