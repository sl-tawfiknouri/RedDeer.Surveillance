namespace Surveillance.Engine.Rules.Rules.Equity.Spoofing.Interfaces
{
    using System.Threading.Tasks;

    public interface ISpoofingRuleMessageSender
    {
        Task Send(ISpoofingRuleBreach ruleBreach);
    }
}