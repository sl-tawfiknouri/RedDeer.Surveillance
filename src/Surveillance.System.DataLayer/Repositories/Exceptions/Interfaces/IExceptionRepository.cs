namespace Surveillance.Auditing.DataLayer.Repositories.Exceptions.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IExceptionRepository
    {
        Task<IReadOnlyCollection<ExceptionDto>> GetForDashboard();

        void Save(ExceptionDto dto);
    }
}