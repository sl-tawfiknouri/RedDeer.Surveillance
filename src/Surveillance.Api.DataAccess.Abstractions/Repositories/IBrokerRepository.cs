namespace Surveillance.Api.DataAccess.Abstractions.Repositories
{
    using System.Threading.Tasks;

    using Surveillance.Api.DataAccess.Abstractions.Entities;

    public interface IBrokerRepository
    {
        Task<IBroker> GetById(int? id);
    }
}