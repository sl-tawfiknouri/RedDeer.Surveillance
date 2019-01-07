using System.Threading.Tasks;

namespace ThirdPartySurveillanceDataSynchroniser.Manager.Factset.Interfaces
{
    public interface IFactsetDataRequestsStorageManager
    {
        Task Store();
    }
}