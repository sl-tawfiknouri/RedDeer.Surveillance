using Surveillance.ElasticSearchDtos;
using System;

namespace Surveillance.Factories.Interfaces
{
    public interface IRuleBreachFactory
    {
        RuleBreachDocument Build(
            RuleBreachCategories category,
            DateTime breachCommencedOn,
            DateTime? breachTerminatedOn);
    }
}