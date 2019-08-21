namespace Surveillance.Engine.Rules.Rules.Equity.PlacingOrderNoIntentToExecute.Interfaces
{
    using System.Threading.Tasks;

    public interface IPlacingOrdersWithNoIntentToExecuteMessageSender
    {
        Task Send(IPlacingOrdersWithNoIntentToExecuteRuleBreach ruleBreach);
    }
}