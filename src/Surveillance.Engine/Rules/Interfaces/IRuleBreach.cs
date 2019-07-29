namespace Surveillance.Engine.Rules.Rules.Interfaces
{
    public interface IRuleBreach : IRuleBreachContext
    {
        string Description { get; set; }
        string CaseTitle { get; set; }
    }
}
