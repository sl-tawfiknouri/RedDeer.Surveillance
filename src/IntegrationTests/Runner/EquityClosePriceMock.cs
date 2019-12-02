using RedDeer.Contracts.SurveillanceService.Api.FactsetSecurityDaily;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedDeer.Surveillance.IntegrationTests.Runner
{
    public class EquityClosePriceMock
    {
        private Dictionary<string, List<FactsetSecurityDailyResponseItem>> _data = new Dictionary<string, List<FactsetSecurityDailyResponseItem>>();

        public void Add(FactsetSecurityDailyResponseItem item)
        {
            if (!_data.ContainsKey(item.Figi))
            {
                _data[item.Figi] = new List<FactsetSecurityDailyResponseItem>();
            }
            _data[item.Figi].Add(item);
        }

        public FactsetSecurityResponseDto GetPrices(FactsetSecurityDailyRequest requests)
        {
            var results = new List<FactsetSecurityDailyResponseItem>();

            foreach (var request in requests.Requests)
            {
                if (_data.ContainsKey(request.Figi))
                {
                    var instrumentItems = _data[request.Figi];

                    var items = instrumentItems
                        .Where(x => x.Epoch >= request.From)
                        .Where(x => x.Epoch <= request.To);

                    results.AddRange(items);
                }
            }

            return new FactsetSecurityResponseDto
            {
                Responses = results
            };
        }
    }
}
