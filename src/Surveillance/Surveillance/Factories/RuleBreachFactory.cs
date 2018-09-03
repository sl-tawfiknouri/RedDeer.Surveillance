using Surveillance.ElasticSearchDtos.Rules;
using Surveillance.Factories.Interfaces;
using System;

namespace Surveillance.Factories
{
    public class RuleBreachFactory : IRuleBreachFactory
    {
        private readonly IOriginFactory _originFactory;

        public RuleBreachFactory(IOriginFactory originFactory)
        {
            _originFactory = originFactory ?? throw new ArgumentNullException(nameof(originFactory));
        }

        public RuleBreachDocument Build(
            RuleBreachCategories category,
            DateTime breachCommencedOn,
            DateTime? breachTerminatedOn,
            string ruleBreachDescription)
        {
            var id = GenerateDate();

            return new RuleBreachDocument
            {
                Id = id,
                CategoryId = (int)category,
                CategoryDescription = category.ToString(),
                RuleBreachDescription = ruleBreachDescription,
                BreachRaisedOn = DateTime.UtcNow,
                BreachCommencedOn = breachCommencedOn,
                BreachTerminatedOn = breachTerminatedOn,
                Origin = _originFactory.Origin(),
            };
        }

        private string GenerateDate()
        {
            var id = Guid.NewGuid().ToString();
            id += "." + DateTime.UtcNow;

            return id;
        }
    }
}
