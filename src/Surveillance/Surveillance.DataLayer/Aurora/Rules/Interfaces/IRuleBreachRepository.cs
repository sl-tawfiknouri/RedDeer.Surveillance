using System.Threading.Tasks;
using DomainV2.Trading;

namespace Surveillance.DataLayer.Aurora.Rules.Interfaces
{
    public interface IRuleBreachRepository
    {
        Task<long?> Create(RuleBreach message);
        Task<RuleBreach> Get(string id);
    }
}