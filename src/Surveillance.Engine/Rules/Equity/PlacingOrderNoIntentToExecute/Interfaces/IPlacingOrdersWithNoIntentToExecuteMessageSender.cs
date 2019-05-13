using System.Threading.Tasks;

namespace Surveillance.Engine.Rules.Rules.Equity.PlacingOrderNoIntentToExecute.Interfaces
{
    interface IPlacingOrdersWithNoIntentToExecuteMessageSender
    {
        Task Send(IPlacingOrdersWithNoIntentToExecuteBreach breach);
    }
}
