using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firefly.Service.Data.TickPriceHistory.Shared.Protos;
using Grpc.Core;

namespace RedDeer.Surveillance.IntegrationTests.Runner
{
    public class FixedIncomeClosePriceMock
    {
        private readonly List<SecurityTimeBarQuerySubResponse> _allData = new List<SecurityTimeBarQuerySubResponse>();
        private readonly List<SecurityTimeBarQuerySubResponse> _currentRunData = new List<SecurityTimeBarQuerySubResponse>();

        public void Add(SecurityTimeBarQuerySubResponse item)
        {
            _allData.Add(item);
        }

        public AsyncUnaryCall<GetEodPricingResponse> GetEodPricingAsync(GetEodPricingRequest request)
        {
            var response = new GetEodPricingResponse {Success = true};

            _currentRunData.AddRange(_allData.Where(w => w.Identifiers.Ric == request.Identifiers.First().Ric
                                                         && w.Timebars.All(a =>
                                                             a.EpochUtc.ToDateTime().Date >= request.StartUtc.ToDateTime().Date &&
                                                             a.EpochUtc.ToDateTime().Date <= request.EndUtc.ToDateTime().Date)));

            return new AsyncUnaryCall<GetEodPricingResponse>(Task.FromResult(response), null, null, null, null);
        }

        public AsyncUnaryCall<SecurityTimeBarQueryResponse> QuerySecurityTimeBarsAsync(SecurityTimeBarQueryRequest request)
        {
            var response = new SecurityTimeBarQueryResponse
            {
                Success = true
            };

            response.SubResponses.AddRange(_currentRunData.Where(w => (request.Subqueries.FirstOrDefault()?.Identifiers == null 
                                                                       || w.Identifiers.Ric == request.Subqueries.FirstOrDefault()?.Identifiers.Ric)
                                                                      && w.Timebars.All(a =>
                                                                          a.EpochUtc.ToDateTime().Date >= request.Subqueries.First().StartUtc.ToDateTime().Date &&
                                                                          a.EpochUtc.ToDateTime().Date <= request.Subqueries.First().EndUtc.ToDateTime().Date)));

            return new AsyncUnaryCall<SecurityTimeBarQueryResponse>(Task.FromResult(response), null, null, null, null);
        }
    }
}