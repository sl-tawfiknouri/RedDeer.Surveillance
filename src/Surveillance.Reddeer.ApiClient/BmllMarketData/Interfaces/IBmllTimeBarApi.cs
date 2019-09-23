namespace Surveillance.Reddeer.ApiClient.BmllMarketData.Interfaces
{
    using System.Threading;
    using System.Threading.Tasks;

    using Firefly.Service.Data.BMLL.Shared.Commands;
    using Firefly.Service.Data.BMLL.Shared.Requests;

    public interface IBmllTimeBarApi
    {
        Task<GetMinuteBarsResponse> GetMinuteBars(GetMinuteBarsRequest request);

        Task<bool> HeartBeating(CancellationToken token);

        Task RequestMinuteBars(CreateMinuteBarRequestCommand createCommand);

        Task<BmllStatusMinuteBarResult> StatusMinuteBars(GetMinuteBarRequestStatusesRequest statusCommand);
    }
}