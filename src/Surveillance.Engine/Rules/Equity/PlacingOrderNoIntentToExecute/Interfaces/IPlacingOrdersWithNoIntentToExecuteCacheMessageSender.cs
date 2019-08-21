namespace Surveillance.Engine.Rules.Rules.Equity.PlacingOrderNoIntentToExecute.Interfaces
{
    public interface IPlacingOrdersWithNoIntentToExecuteCacheMessageSender
    {
        int Flush();

        void Send(IPlacingOrdersWithNoIntentToExecuteRuleBreach ruleBreach);
    }
}