using NUnit.Framework;
using Surveillance.Api.DataAccess.Entities;
using Surveillance.Api.Tests.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Surveillance.Api.Tests.Tests
{
    public class RuleBreachTests
    {
        [Test]
        public async Task CanRequestRuleBreaches()
        {
            // arrange
            Dependencies.DbContext.RuleBreaches.Add(new RuleBreach());
            await Dependencies.DbContext.SaveChangesAsync();

            // act
            var count = await Dependencies.ApiClient.RuleBreachesCountAsync();

            // assert
            Assert.That(count, Is.EqualTo(1));
        }
    }
}
