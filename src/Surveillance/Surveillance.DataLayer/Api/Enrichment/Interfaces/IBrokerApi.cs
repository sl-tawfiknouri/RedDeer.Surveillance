using System.Threading;
using System.Threading.Tasks;
using RedDeer.Contracts.SurveillanceService.Api.BrokerEnrichment;

namespace Surveillance.DataLayer.Api.Enrichment
{
    public interface IBrokerApi
    {
        Task<BrokerEnrichmentMessage> Post(BrokerEnrichmentMessage message);
        Task<bool> HeartBeating(CancellationToken token);
    }
}