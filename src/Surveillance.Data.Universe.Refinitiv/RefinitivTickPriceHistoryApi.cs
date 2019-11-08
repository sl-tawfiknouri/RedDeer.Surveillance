using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firefly.Service.Data.TickPriceHistory.Shared.Protos;
using Google.Protobuf.WellKnownTypes;
using Surveillance.Data.Universe.Refinitiv.Interfaces;

namespace Surveillance.Data.Universe.Refinitiv
{

    public class RefinitivTickPriceHistoryApi 
        : IRefinitivTickPriceHistoryApi
    {
        private readonly TickPriceHistoryService.TickPriceHistoryServiceClient tickPriceHistoryServiceClient;

        public RefinitivTickPriceHistoryApi(
            ITickPriceHistoryServiceClientFactory tickPriceHistoryServiceClientFactory)
        {
            tickPriceHistoryServiceClient = tickPriceHistoryServiceClientFactory.Create();
        }

        public async Task<IList<EndOfDaySecurityTimeBar>> GetInterdayTimeBars(DateTime startDay, DateTime endDay)
        {
            var request = new SecurityTimeBarQueryRequest()
            {
                Subqueries =
                {
                    new SecurityTimeBarSubqueryRequest()
                    {
                        StartUtc = new Timestamp(Timestamp.FromDateTime(startDay)),
                        EndUtc = new Timestamp(Timestamp.FromDateTime(endDay)),
                        ReferenceId = Guid.NewGuid().ToString(),
                        //Identifiers = new SecurityIdentifiers { Ric = "BE179329760=RRPS" },
                        PolicyOptions = TimeBarPolicyOptions.EndOfDay
                    }
                }
            };

            var response = await tickPriceHistoryServiceClient
                .QuerySecurityTimeBarsAsync(request).ResponseAsync;

            var result = new List<EndOfDaySecurityTimeBar>();

            foreach (var subResponse in response.SubResponses)
            {

                foreach (var timebar in subResponse.Timebars)
                {
                    var endOfDaySecurityTimeBar = new EndOfDaySecurityTimeBar
                    {
                        SecurityIdentifiers = new SecurityIdentifier
                        {
                            Cusip = subResponse.Identifiers?.Cusip,
                            ExternalId = subResponse.Identifiers.ExternalIdentifiers,
                            Isin = subResponse.Identifiers.Isin,
                            Ric = subResponse.Identifiers.Ric,
                            Sedol = subResponse.Identifiers.Sedol
                        },
                        
                        TimeBar = MapTimeBar(timebar)
                    };

                    result.Add(endOfDaySecurityTimeBar);
                }
            }

            var success = response.Success;

            return result;
        }

        private Timebar MapTimeBar(TimeBar timeBar)
        {
            return new Timebar
            {
                CloseAsk = Convert.ToDecimal(timeBar.CloseAsk),
                CloseAskYield  = Convert.ToDecimal(timeBar.CloseAskYield),
                CloseBid  = Convert.ToDecimal(timeBar.CloseBid),
                CloseDiscountFactor  = Convert.ToDecimal(timeBar.CloseDiscountFactor),
                CloseYield  = Convert.ToDecimal(timeBar.CloseYield),
                CloseZeroYield  = Convert.ToDecimal(timeBar.CloseZeroYield),
                EpochUtc  = timeBar.EpochUtc.ToDateTime(),
                High  = Convert.ToDecimal(timeBar.High),
                HighAsk  = Convert.ToDecimal(timeBar.HighAsk),
                HighAskYield  = Convert.ToDecimal(timeBar.HighAskYield),
                HighBid  = Convert.ToDecimal(timeBar.HighBid),
                HighBidYield  = Convert.ToDecimal(timeBar.HighBidYield),
                HighDiscountFactor  = Convert.ToDecimal(timeBar.HighDiscountFactor),
                HighYield  = Convert.ToDecimal(timeBar.HighYield),
                HighZeroYield  = Convert.ToDecimal(timeBar.HighZeroYield),
                Last  = Convert.ToDecimal(timeBar.Last),
                Low  = Convert.ToDecimal(timeBar.Low),
                LowAsk  = Convert.ToDecimal(timeBar.LowAsk),
                LowAskYield  = Convert.ToDecimal(timeBar.LowAskYield),
                LowBid  = Convert.ToDecimal(timeBar.LowBid),
                LowBidYield  = Convert.ToDecimal(timeBar.LowBidYield),
                LowDiscountFactor  = Convert.ToDecimal(timeBar.LowDiscountFactor),
                LowYield  = Convert.ToDecimal(timeBar.LowYield),
                LowZeroYield  = Convert.ToDecimal(timeBar.LowZeroYield),
                NoAsks  = Convert.ToDecimal(timeBar.NoAsks),
                NoAskYields  = Convert.ToDecimal(timeBar.NoAskYields),
                NoBids  = Convert.ToDecimal(timeBar.NoBids),
                NoBidYields  = Convert.ToDecimal(timeBar.NoBidYields),
                NoDiscountFactors  = Convert.ToDecimal(timeBar.NoDiscountFactors),
                NoTrades  = Convert.ToDecimal(timeBar.NoTrades),
                NoYields  = Convert.ToDecimal(timeBar.NoYields),
                NoZeroYields  = Convert.ToDecimal(timeBar.NoZeroYields),
                Open  = Convert.ToDecimal(timeBar.Open),
                OpenAsk  = Convert.ToDecimal(timeBar.OpenAsk),
                OpenAskYield  = Convert.ToDecimal(timeBar.OpenAskYield),
                OpenBid  = Convert.ToDecimal(timeBar.OpenBid),
                OpenBidYield  = Convert.ToDecimal(timeBar.OpenBidYield),
                OpenDiscountFactor  = Convert.ToDecimal(timeBar.OpenDiscountFactor),
                OpenYield  = Convert.ToDecimal(timeBar.OpenYield),
                OpenZeroYield  = Convert.ToDecimal(timeBar.OpenZeroYield),
                Volume  = Convert.ToDecimal(timeBar.Volume),
                CurrencyCode  = timeBar.CurrencyCode
            };
        }
    }
}
