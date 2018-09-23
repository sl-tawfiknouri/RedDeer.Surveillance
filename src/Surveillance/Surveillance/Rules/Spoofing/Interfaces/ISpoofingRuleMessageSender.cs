namespace Surveillance.Rules.Spoofing.Interfaces
{
    public interface ISpoofingRuleMessageSender
    {
        void Send(ISpoofingRuleBreach ruleBreach);
    }
}