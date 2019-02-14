using Surveillance.Auditing.Context.Interfaces;

namespace Surveillance.Auditing.Factories.Interfaces
{
    public interface ISystemProcessOperationContextFactory
    {
        ISystemProcessOperationContext Build(ISystemProcessContext context);
    }
}