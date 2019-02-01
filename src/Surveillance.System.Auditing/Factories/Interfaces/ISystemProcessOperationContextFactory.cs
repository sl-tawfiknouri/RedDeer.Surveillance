using Surveillance.Systems.Auditing.Context.Interfaces;

namespace Surveillance.Systems.Auditing.Factories.Interfaces
{
    public interface ISystemProcessOperationContextFactory
    {
        ISystemProcessOperationContext Build(ISystemProcessContext context);
    }
}