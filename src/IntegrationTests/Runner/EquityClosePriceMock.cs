using RedDeer.Contracts.SurveillanceService.Api.FactsetSecurityDaily;
using System;
using System.Collections.Generic;
using System.Text;

namespace RedDeer.Surveillance.IntegrationTests.Runner
{
    public class EquityClosePriceMock
    {
        private Dictionary<string, Dictionary<DateTime, FactsetSecurityDailyResponseItem>> _data = new Dictionary<string, Dictionary<DateTime, FactsetSecurityDailyResponseItem>>();

        public void Add(FactsetSecurityDailyResponseItem item)
        {
            if (!_data.ContainsKey(item.Figi))
            {
                _data[item.Figi] = new Dictionary<DateTime, FactsetSecurityDailyResponseItem>();
            }
            _data[item.Figi][item.Epoch] = item;
        }

        public FactsetSecurityResponseDto GetPrices(FactsetSecurityDailyRequest requests)
        {
            var results = new List<FactsetSecurityDailyResponseItem>();

            foreach (var request in requests.Requests)
            {
                if (_data.ContainsKey(request.Figi))
                {
                    var instrumentDates = _data[request.Figi];

                    var min = request.From.Date;
                    var max = request.To.Date;

                    var iter = min;
                    while (iter <= max)
                    {
                        if (instrumentDates.ContainsKey(iter))
                        {
                            results.Add(instrumentDates[iter]);
                        }

                        iter = iter.AddDays(1);
                    }
                }
            }

            return new FactsetSecurityResponseDto
            {
                Responses = results
            };
        }
    }
}
