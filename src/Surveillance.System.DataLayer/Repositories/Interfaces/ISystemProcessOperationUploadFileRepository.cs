namespace Surveillance.Auditing.DataLayer.Repositories.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Surveillance.Auditing.DataLayer.Processes.Interfaces;

    public interface ISystemProcessOperationUploadFileRepository
    {
        Task Create(ISystemProcessOperationUploadFile entity);

        Task<IReadOnlyCollection<ISystemProcessOperationUploadFile>> GetDashboard();

        Task<IReadOnlyCollection<ISystemProcessOperationUploadFile>> GetOnDate(DateTime date);
    }
}