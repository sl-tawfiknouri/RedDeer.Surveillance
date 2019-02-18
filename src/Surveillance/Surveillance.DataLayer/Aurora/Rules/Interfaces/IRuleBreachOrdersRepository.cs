using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Trading;

namespace Surveillance.DataLayer.Aurora.Rules.Interfaces
{
    public interface IRuleBreachOrdersRepository
    {
        Task Create(IReadOnlyCollection<RuleBreachOrder> message);
        Task<IReadOnlyCollection<RuleBreachOrder>> Get(string id);
    }
}