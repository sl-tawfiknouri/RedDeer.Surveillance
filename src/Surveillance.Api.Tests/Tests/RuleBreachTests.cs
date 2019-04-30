using NUnit.Framework;
using Surveillance.Api.Client.Queries;
using Surveillance.Api.DataAccess.Entities;
using System.Threading.Tasks;

namespace Surveillance.Api.Tests.Tests
{
    public class RuleBreachTests
    {
        [Test]
        public async Task CanRequestRuleBreachWithId()
        {
            // arrange
            Dependencies.DbContext.RuleBreaches.Add(new RuleBreach
            {
                Id = 5,
                RuleId = "abc"
            });
            await Dependencies.DbContext.SaveChangesAsync();

            // act
            var ruleBreaches = await Dependencies.ApiClient.QueryAsync(new RuleBreachQuery().ArgumentId(5));

            // assert
            Assert.That(ruleBreaches, Has.Count.EqualTo(1));
            Assert.That(ruleBreaches[0].Id, Is.EqualTo(5));
            Assert.That(ruleBreaches[0].RuleId, Is.EqualTo("abc"));
        }
    }
}
