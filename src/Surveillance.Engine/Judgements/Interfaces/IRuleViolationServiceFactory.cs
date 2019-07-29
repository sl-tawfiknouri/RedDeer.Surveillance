namespace Surveillance.Engine.Rules.Judgements.Interfaces
{
    public interface IRuleViolationServiceFactory
    {
        IRuleViolationService Build();
    }
}