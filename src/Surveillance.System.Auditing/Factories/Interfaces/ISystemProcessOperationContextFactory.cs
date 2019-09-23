namespace Surveillance.Auditing.Factories.Interfaces
{
    using Surveillance.Auditing.Context.Interfaces;

    public interface ISystemProcessOperationContextFactory
    {
        ISystemProcessOperationContext Build(ISystemProcessContext context);
    }
}