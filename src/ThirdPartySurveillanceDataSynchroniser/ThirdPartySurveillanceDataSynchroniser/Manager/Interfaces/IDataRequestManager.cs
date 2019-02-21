using System.Threading.Tasks;
using Surveillance.Auditing.Context.Interfaces;

namespace DataSynchroniser.Manager.Interfaces
{
    public interface IDataRequestManager
    {
        Task Handle(string systemProcessOperationId, ISystemProcessOperationThirdPartyDataRequestContext dataRequestContext);
    }
}