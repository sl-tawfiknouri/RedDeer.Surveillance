namespace Surveillance.Engine.Rules.Rules.Equity.CancelledOrders.Interfaces
{
    using System.Threading.Tasks;

    public interface ICancelledOrderMessageSender
    {
        Task Send(ICancelledOrderRuleBreach ruleBreach);
    }
}