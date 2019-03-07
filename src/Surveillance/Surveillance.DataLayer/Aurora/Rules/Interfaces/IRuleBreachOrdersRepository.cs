using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Surveillance.Rules;

namespace Surveillance.DataLayer.Aurora.Rules.Interfaces
{
    public interface IRuleBreachOrdersRepository
    {
        Task Create(IReadOnlyCollection<RuleBreachOrder> message);
        Task<IReadOnlyCollection<RuleBreachOrder>> Get(string id);
    }
}