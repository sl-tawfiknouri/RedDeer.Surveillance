using NUnit.Framework;
using RedDeer.Surveillance.Api.Client.Queries;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Surveillance.Api.Tests.Tests
{
    public class RefinitivTickPriceHistoryTests 
        : BaseTest
    {
        [Test]
        public async Task ManualTest()
        {
            var query = new TickPriceHistoryTimeBarQuery();
            query.Filter
               .ArgumentRics(new List<string> { "BE179329760=RRPS" } )
               .ArgumentStartDateTime(new DateTime(2019, 01, 01, 00, 00, 00, DateTimeKind.Utc))
               .ArgumentEndDateTime(new DateTime(2019, 02, 01, 00, 00, 00, DateTimeKind.Utc))
               .Node.FieldClose().FieldCloseAsk().FieldCurrencyCode().FieldEpochUtc().FieldHigh().FieldHighAsk().FieldLow().FieldRic();

            var response = await this._apiClient.QueryAsync(query, CancellationToken.None);
        }
    }
}
