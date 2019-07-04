using System.Threading;
using System.Threading.Tasks;
using RedDeer.Contracts.SurveillanceService.Api.SecurityEnrichment;

namespace Surveillance.Reddeer.ApiClient.Enrichment.Interfaces
{
    public interface IEnrichmentApi
    {
        Task<SecurityEnrichmentMessage> Post(SecurityEnrichmentMessage message);
        Task<bool> HeartBeating(CancellationToken token);
    }
}