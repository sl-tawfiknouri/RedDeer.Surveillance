namespace Surveillance.Engine.Rules.RuleParameters.Interfaces
{
    public interface IRuleParameter : IValidatable, ITuneableRule
    {
        string Id { get; }
    }
}