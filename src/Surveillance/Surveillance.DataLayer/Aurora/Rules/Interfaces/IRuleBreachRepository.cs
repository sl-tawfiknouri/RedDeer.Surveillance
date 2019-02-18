using System.Threading.Tasks;
using Domain.Trading;

namespace Surveillance.DataLayer.Aurora.Rules.Interfaces
{
    public interface IRuleBreachRepository
    {
        Task<long?> Create(RuleBreach message);
        Task<RuleBreach> Get(string id);
        Task<bool> HasDuplicate(string ruleId);
    }
}