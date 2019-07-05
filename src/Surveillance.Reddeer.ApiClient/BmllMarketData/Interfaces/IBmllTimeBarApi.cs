using System.Threading;
using System.Threading.Tasks;
using Firefly.Service.Data.BMLL.Shared.Commands;
using Firefly.Service.Data.BMLL.Shared.Requests;

namespace Surveillance.Reddeer.ApiClient.BmllMarketData.Interfaces
{
    public interface IBmllTimeBarApi
    {
        Task RequestMinuteBars(CreateMinuteBarRequestCommand createCommand);
        Task<BmllStatusMinuteBarResult> StatusMinuteBars(GetMinuteBarRequestStatusesRequest statusCommand);
        Task<GetMinuteBarsResponse> GetMinuteBars(GetMinuteBarsRequest request);
        Task<bool> HeartBeating(CancellationToken token);
    }
}