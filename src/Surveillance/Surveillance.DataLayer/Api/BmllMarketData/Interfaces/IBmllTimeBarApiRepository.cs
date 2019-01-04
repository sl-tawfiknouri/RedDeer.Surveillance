using System.Threading;
using System.Threading.Tasks;
using Firefly.Service.Data.BMLL.Shared.Requests;

namespace Surveillance.DataLayer.Api.BmllMarketData.Interfaces
{
    public interface IBmllTimeBarApiRepository
    {
        Task<GetMinuteBarsResponse> Get(GetMinuteBarsRequest request);
        Task<bool> HeartBeating(CancellationToken token);
    }
}