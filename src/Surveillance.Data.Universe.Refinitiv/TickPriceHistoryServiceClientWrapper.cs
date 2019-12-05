using Firefly.Service.Data.TickPriceHistory.Shared.Protos;
using Grpc.Core;
using Surveillance.Data.Universe.Refinitiv.Interfaces;

namespace Surveillance.Data.Universe.Refinitiv
{
    public class TickPriceHistoryServiceClientWrapper : ITickPriceHistoryServiceClientWrapper
    {
        public TickPriceHistoryService.TickPriceHistoryServiceClient Client { get; }

        public TickPriceHistoryServiceClientWrapper(ChannelBase channel)
        {
            Client = new TickPriceHistoryService.TickPriceHistoryServiceClient(channel);
        }
        
        public AsyncUnaryCall<GetEodPricingResponse> GetEodPricingAsync(GetEodPricingRequest request)
        {
            var response = Client.GetEodPricingAsync(request);
            return response;
        }

        public AsyncUnaryCall<SecurityTimeBarQueryResponse> QuerySecurityTimeBars(SecurityTimeBarQueryRequest request)
        {
            var response = Client.QuerySecurityTimeBarsAsync(request);
            return response;
        }
    }
}