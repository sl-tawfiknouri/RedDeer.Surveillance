namespace Surveillance.Auditing.Factories.Interfaces
{
    using Surveillance.Auditing.Context.Interfaces;

    public interface ISystemProcessOperationRunRuleContextFactory
    {
        ISystemProcessOperationRunRuleContext Build(ISystemProcessOperationContext context);
    }
}