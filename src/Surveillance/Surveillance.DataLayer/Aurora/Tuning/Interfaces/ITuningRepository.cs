namespace Surveillance.DataLayer.Aurora.Tuning.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ITuningRepository
    {
        Task SaveTasks(IReadOnlyCollection<TuningRepository.TuningPair> tuningRuns);
    }
}