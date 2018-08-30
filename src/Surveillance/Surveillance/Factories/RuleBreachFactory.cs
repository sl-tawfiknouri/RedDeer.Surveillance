using Surveillance.ElasticSearchDtos;
using Surveillance.Factories.Interfaces;
using System;

namespace Surveillance.Factories
{
    public class RuleBreachFactory : IRuleBreachFactory
    {
        public RuleBreachDocument Build(
            RuleBreachCategories category,
            DateTime breachCommencedOn,
            DateTime? breachTerminatedOn)
        {
            var id = GenerateDate();

            return new RuleBreachDocument
            {
                Id = id,
                CategoryId = (int)category,
                CategoryDescription = category.ToString(),
                BreachRaisedOn = DateTime.UtcNow,
                BreachCommencedOn = breachCommencedOn,
                BreachTerminatedOn = breachTerminatedOn
            };
        }

        private string GenerateDate()
        {
            var id = Guid.NewGuid().ToString();
            id += "." + DateTime.UtcNow.ToString();

            return id;
        }
    }
}
