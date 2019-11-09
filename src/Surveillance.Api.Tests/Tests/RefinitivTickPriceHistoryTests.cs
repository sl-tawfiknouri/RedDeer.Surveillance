using NUnit.Framework;
using RedDeer.Surveillance.Api.Client.Queries;
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
            query.Filter.Node.FieldClose().FieldCloseAsk().FieldCurrencyCode().FieldEpochUtc().FieldHigh().FieldHighAsk().FieldLow().FieldRic();

            var response = await this._apiClient.QueryAsync(query, CancellationToken.None);
        }
    }
}
