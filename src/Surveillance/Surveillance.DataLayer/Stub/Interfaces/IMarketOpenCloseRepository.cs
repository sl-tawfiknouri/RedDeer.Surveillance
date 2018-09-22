using System.Collections.Generic;

namespace Surveillance.DataLayer.Stub.Interfaces
{
    public interface IMarketOpenCloseRepository
    {
        IReadOnlyCollection<MarketOpenClose> GetAll();
        IReadOnlyCollection<MarketOpenClose> Get(IReadOnlyCollection<string> marketIds);
    }
}