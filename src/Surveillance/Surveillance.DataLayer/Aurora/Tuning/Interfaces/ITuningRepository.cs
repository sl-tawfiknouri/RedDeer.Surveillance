using System.Collections.Generic;
using System.Threading.Tasks;

namespace Surveillance.DataLayer.Aurora.Tuning.Interfaces
{
    public interface ITuningRepository
    {
        Task SaveTasks(IReadOnlyCollection<TuningRepository.TuningPair> tuningRuns);
    }
}