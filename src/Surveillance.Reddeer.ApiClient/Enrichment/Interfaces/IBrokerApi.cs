using System.Threading;
using System.Threading.Tasks;
using RedDeer.Contracts.SurveillanceService.Api.BrokerEnrichment;

namespace Surveillance.Reddeer.ApiClient.Enrichment.Interfaces
{
    public interface IBrokerApi
    {
        Task<BrokerEnrichmentMessage> Post(BrokerEnrichmentMessage message);
        Task<bool> HeartBeating(CancellationToken token);
    }
}