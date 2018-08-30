using Domain.Equity.Trading.Frames;
using Surveillance.ElasticSearchDtos.Market;

namespace Surveillance.Recorders.Projectors.Interfaces
{
    public interface IReddeerMarketExchangeFormatProjector
    {
        ReddeerMarketDocument Project(ExchangeFrame frame);
    }
}