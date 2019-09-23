namespace DataSynchroniser.Manager.Interfaces
{
    using System.Threading.Tasks;

    using Surveillance.Auditing.Context.Interfaces;

    public interface IDataRequestManager
    {
        Task Handle(
            string systemProcessOperationId,
            ISystemProcessOperationThirdPartyDataRequestContext dataRequestContext);
    }
}