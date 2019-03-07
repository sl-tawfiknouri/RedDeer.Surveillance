using System.Threading.Tasks;
using Domain.Surveillance.Rules;

namespace Surveillance.DataLayer.Aurora.Rules.Interfaces
{
    public interface IRuleBreachRepository
    {
        Task<long?> Create(RuleBreach message);
        Task<RuleBreach> Get(string id);
        Task<bool> HasDuplicate(string ruleId);
    }
}