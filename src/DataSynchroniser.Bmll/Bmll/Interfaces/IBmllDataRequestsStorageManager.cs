namespace DataSynchroniser.Api.Bmll.Bmll.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IBmllDataRequestsStorageManager
    {
        Task Store(IReadOnlyCollection<IGetTimeBarPair> timeBarPairs);
    }
}