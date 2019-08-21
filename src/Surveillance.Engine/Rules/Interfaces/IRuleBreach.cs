namespace Surveillance.Engine.Rules.Rules.Interfaces
{
    public interface IRuleBreach : IRuleBreachContext
    {
        string CaseTitle { get; set; }

        string Description { get; set; }
    }
}