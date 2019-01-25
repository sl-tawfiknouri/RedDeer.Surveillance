using System.Collections.Generic;
using System.Threading.Tasks;
using DomainV2.Trading;

namespace Surveillance.DataLayer.Aurora.Rules.Interfaces
{
    public interface IRuleBreachOrdersRepository
    {
        Task Create(IReadOnlyCollection<RuleBreachOrder> message);
        Task<RuleBreachOrder> Get(string id);
    }
}