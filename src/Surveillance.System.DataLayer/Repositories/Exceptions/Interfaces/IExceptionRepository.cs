using System.Collections.Generic;
using System.Threading.Tasks;

namespace Surveillance.System.DataLayer.Repositories.Exceptions.Interfaces
{
    public interface IExceptionRepository
    {
        void Save(ExceptionDto dto);
        Task<IReadOnlyCollection<ExceptionDto>> GetForDashboard();
    }
}