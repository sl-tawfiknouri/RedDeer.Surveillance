using System.Threading.Tasks;
using Firefly.Service.Data.TickPriceHistory.Shared.Protos;
using Grpc.Core;

namespace Surveillance.Data.Universe.Refinitiv.Interfaces
{
    public interface ITickPriceHistoryServiceClientWrapper
    {
        TickPriceHistoryService.TickPriceHistoryServiceClient Client { get; }
        AsyncUnaryCall<GetEodPricingResponse> GetEodPricingAsync(GetEodPricingRequest request);
        AsyncUnaryCall<SecurityTimeBarQueryResponse> QuerySecurityTimeBars(SecurityTimeBarQueryRequest request);
    }
}