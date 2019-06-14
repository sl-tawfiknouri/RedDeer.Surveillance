using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Surveillance.Scheduling;

namespace Surveillance.DataLayer.Aurora.Scheduler.Interfaces
{
    public interface ITaskSchedulerRepository
    {
        Task MarkTasksProcessed(IReadOnlyCollection<AdHocScheduleRequest> requests);
        Task<IReadOnlyCollection<AdHocScheduleRequest>> ReadUnprocessedTask(DateTime dueBy);
        Task SaveTask(AdHocScheduleRequest request);
    }
}