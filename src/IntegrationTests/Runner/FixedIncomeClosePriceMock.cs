using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firefly.Service.Data.TickPriceHistory.Shared.Protos;
using Grpc.Core;

namespace RedDeer.Surveillance.IntegrationTests.Runner
{
    public class FixedIncomeClosePriceMock
    {
        private readonly List<SecurityTimeBarQuerySubResponse> _securityTimeBarQuerySubResponseAllData = new List<SecurityTimeBarQuerySubResponse>();
        private readonly List<SecurityTimeBarQuerySubResponse> _securityTimeBarQuerySubResponseCurrentRunData = new List<SecurityTimeBarQuerySubResponse>();

        public void Add(SecurityTimeBarQuerySubResponse item)
        {
            _securityTimeBarQuerySubResponseAllData.Add(item);
        }

        public AsyncUnaryCall<GetEodPricingResponse> GetEodPricingAsync(GetEodPricingRequest request)
        {
            var response = new GetEodPricingResponse {Success = true};
            _securityTimeBarQuerySubResponseCurrentRunData.AddRange(_securityTimeBarQuerySubResponseAllData.Where(w => w.Identifiers.Ric == request.Identifiers.First().Ric));
            return new AsyncUnaryCall<GetEodPricingResponse>(Task.FromResult(response), null, null, null, null);
        }

        public AsyncUnaryCall<SecurityTimeBarQueryResponse> QuerySecurityTimeBarsAsync(SecurityTimeBarQueryRequest request)
        {
            var response = new SecurityTimeBarQueryResponse
            {
                Success = true
            };

            response.SubResponses.Add(new SecurityTimeBarQuerySubResponse
            {
                Timebars =
                {
                    _securityTimeBarQuerySubResponseCurrentRunData.SelectMany(s => s.Timebars.Where(w => w.EpochUtc.ToDateTime().Date >= request.Subqueries.First().StartUtc.ToDateTime().Date &&
                                                                                                         w.EpochUtc.ToDateTime().Date <= request.Subqueries.First().EndUtc.ToDateTime().Date))
                },
                Identifiers = _securityTimeBarQuerySubResponseCurrentRunData.FirstOrDefault()?.Identifiers
            });

            return new AsyncUnaryCall<SecurityTimeBarQueryResponse>(Task.FromResult(response), null, null, null, null);
        }
    }
}