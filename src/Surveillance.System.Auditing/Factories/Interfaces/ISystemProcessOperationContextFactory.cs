using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.System.Auditing.Factories.Interfaces
{
    public interface ISystemProcessOperationContextFactory
    {
        ISystemProcessOperationContext Build(ISystemProcessContext context);
    }
}