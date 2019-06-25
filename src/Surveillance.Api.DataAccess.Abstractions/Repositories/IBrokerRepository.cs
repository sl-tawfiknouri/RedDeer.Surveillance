using System.Threading.Tasks;
using Surveillance.Api.DataAccess.Abstractions.Entities;

namespace Surveillance.Api.DataAccess.Abstractions.Repositories
{
    public interface IBrokerRepository
    {
        Task<IBroker> GetById(int? id);
    }
}