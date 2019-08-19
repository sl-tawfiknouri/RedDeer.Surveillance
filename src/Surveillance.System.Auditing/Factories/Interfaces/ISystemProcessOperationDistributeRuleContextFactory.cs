namespace Surveillance.Auditing.Factories.Interfaces
{
    using Surveillance.Auditing.Context.Interfaces;

    public interface ISystemProcessOperationDistributeRuleContextFactory
    {
        ISystemProcessOperationDistributeRuleContext Build(ISystemProcessOperationContext operationContext);
    }
}