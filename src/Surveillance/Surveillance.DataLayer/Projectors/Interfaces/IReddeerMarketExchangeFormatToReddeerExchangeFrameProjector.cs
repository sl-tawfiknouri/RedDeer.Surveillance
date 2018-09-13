using Domain.Equity.Frames;
using Surveillance.ElasticSearchDtos.Market;
using System.Collections.Generic;

namespace Surveillance.DataLayer.Projectors.Interfaces
{
    public interface IReddeerMarketExchangeFormatToReddeerExchangeFrameProjector
    {
        IReadOnlyCollection<ExchangeFrame> Project(IReadOnlyCollection<ReddeerMarketDocument> documents);
        ExchangeFrame Project(ReddeerMarketDocument document);
    }
}