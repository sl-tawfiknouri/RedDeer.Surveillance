using System.Collections.Generic;
using System.Threading.Tasks;
using Firefly.Service.Data.TickPriceHistory.Shared.Protos;
using Grpc.Core;

namespace RedDeer.Surveillance.IntegrationTests.Runner
{
    public class FixedIncomeClosePriceMock
    {
        private readonly List<SecurityTimeBarQuerySubResponse> _securityTimeBarQuerySubResponse = new List<SecurityTimeBarQuerySubResponse>();

        public void Add(SecurityTimeBarQuerySubResponse item)
        {
            _securityTimeBarQuerySubResponse.Add(item);
        }

        public AsyncUnaryCall<GetEodPricingResponse> GetEodPricingAsync()
        {
            var response = new GetEodPricingResponse {Success = true};
            return new AsyncUnaryCall<GetEodPricingResponse>(Task.FromResult(response), null, null, null, null);
        }

        public AsyncUnaryCall<SecurityTimeBarQueryResponse> QuerySecurityTimeBarsAsync()
        {
            var response = new SecurityTimeBarQueryResponse
            {
                Success = true
            };
            response.SubResponses.AddRange(_securityTimeBarQuerySubResponse);

            return new AsyncUnaryCall<SecurityTimeBarQueryResponse>(Task.FromResult(response), null, null, null, null);
        }
    }
}