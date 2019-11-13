using System;
using System.Collections.Generic;
using System.Linq;
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



        public async Task<IList<EndOfDaySecurityTimeBar>> GetInterdayTimeBars(DateTime? startDay, DateTime? endDay, IList<string> rics = null)
        {
            var allDayPeriods = SplitDateRangeByDay(startDay.Value, endDay.Value);
            var result = new List<EndOfDaySecurityTimeBar>();

            foreach (var period in allDayPeriods)
            {
                var request = new SecurityTimeBarQueryRequest() { };
                var requestByRics = rics?.Where(w => !string.IsNullOrEmpty(w)).ToList() ?? new List<string>();
                if (!requestByRics.Any())
                {
                    requestByRics.Add(null);
                }

                var referenceId = Guid.NewGuid().ToString();
                Timestamp startUtc = new Timestamp(Timestamp.FromDateTime(DateTime.SpecifyKind(period.Item1, DateTimeKind.Utc)));
                Timestamp endUtc = new Timestamp(Timestamp.FromDateTime(DateTime.SpecifyKind(period.Item2, DateTimeKind.Utc)));

                foreach (var item in requestByRics)
                {
                    var subqueryRequest = new SecurityTimeBarSubqueryRequest()
                    {
                        StartUtc = startUtc,
                        EndUtc = endUtc,
                        ReferenceId = referenceId,
                        PolicyOptions = TimeBarPolicyOptions.EndOfDay
                    };

                    if (item != null)
                    {
                        subqueryRequest.Identifiers = new SecurityIdentifiers
                        {
                            Ric = item
                        };
                    }

                    request.Subqueries.Add(subqueryRequest);
                }

                var response = await tickPriceHistoryServiceClient
                    .QuerySecurityTimeBarsAsync(request).ResponseAsync;

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
            }

            return result;
        }

        //  https://github.com/red-deer/RedDeer/commit/2baed618
        private static List<(DateTime, DateTime)> SplitDateRangeByDay(DateTime from, DateTime to)
        {
            if (to < from)
            {
                throw new ArgumentException($"To time is lower than from. {to} < {from}");
            }

            var splits = new List<(DateTime, DateTime)>();

            TimeSpan difference = to - from;
            var currentFrom = from;

            for (var i = 0; i <= difference.Days; i++)
            {
                var currentTo = currentFrom.AddDays(1);
                if (currentTo > to)
                {
                    currentTo = to;
                }
                else
                {
                    currentTo = currentTo.AddMilliseconds(-1);
                }

                if (currentTo > currentFrom || (splits.Count == 0 && currentTo == currentFrom))
                {
                    splits.Add((currentFrom, currentTo));
                }

                currentFrom = currentFrom.AddDays(1);
            }

            return splits;
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
