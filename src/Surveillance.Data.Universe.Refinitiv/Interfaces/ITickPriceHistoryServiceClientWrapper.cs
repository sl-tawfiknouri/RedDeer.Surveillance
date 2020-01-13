using Firefly.Service.Data.TickPriceHistory.Shared.Protos;

namespace Surveillance.Data.Universe.Refinitiv.Interfaces
{
    public interface ITickPriceHistoryServiceClientWrapper
    {
        TickPriceHistoryService.TickPriceHistoryServiceClient Client { get; }
    }
}