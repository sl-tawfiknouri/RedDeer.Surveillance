using NUnit.Framework;
using Surveillance.Api.Client.Queries;
using Surveillance.Api.DataAccess.Entities;
using System.Threading.Tasks;

namespace Surveillance.Api.Tests.Tests
{
    public class RuleBreachTests : BaseTest
    {
        [Test]
        public async Task CanRequest_RuleBreaches_WithoutOrders()
        {
            // arrange
            _dbContext.DbRuleBreaches.Add(new RuleBreach
            {
                Id = 5,
                RuleId = "abc"
            });
            _dbContext.DbRuleBreaches.Add(new RuleBreach
            {
                Id = 6,
                RuleId = "xyz"
            });
            await _dbContext.SaveChangesAsync();

            var query = new RuleBreachQuery();
            query
                .RuleBreachNode
                    .FieldId()
                    .FieldRuleId();

            // act
            var ruleBreaches = await _apiClient.QueryAsync(query);

            // assert
            Assert.That(ruleBreaches, Has.Count.EqualTo(2));

            var ruleBreach0 = ruleBreaches[0];
            Assert.That(ruleBreach0.Id, Is.EqualTo(5));
            Assert.That(ruleBreach0.RuleId, Is.EqualTo("abc"));
            Assert.That(ruleBreach0.Orders, Is.Null);

            var ruleBreach1 = ruleBreaches[1];
            Assert.That(ruleBreach1.Id, Is.EqualTo(6));
            Assert.That(ruleBreach1.RuleId, Is.EqualTo("xyz"));
            Assert.That(ruleBreach1.Orders, Is.Null);
        }

        [Test]
        public async Task CanRequest_RuleBreachById_WithOrders()
        {
            // arrange
            _dbContext.DbRuleBreaches.Add(new RuleBreach
            {
                Id = 5,
                RuleId = "abc"
            });
            _dbContext.DbOrders.Add(new Order
            {
                Id = 8,
                LimitPrice = 6.78m
            });
            _dbContext.DbOrders.Add(new Order
            {
                Id = 9,
                LimitPrice = 3.45m
            });
            _dbContext.DbRuleBreachOrders.Add(new RuleBreachOrder
            {
                RuleBreachId = 5,
                OrderId = 8
            });
            _dbContext.DbRuleBreachOrders.Add(new RuleBreachOrder
            {
                RuleBreachId = 5,
                OrderId = 9
            });
            await _dbContext.SaveChangesAsync();

            var query = new RuleBreachQuery();
            query
                .RuleBreachNode
                    .ArgumentId(5)
                    .FieldId()
                    .FieldRuleId()
                    .FieldOrders()
                        .FieldId()
                        .FieldLimitPrice();

            // act
            var ruleBreaches = await _apiClient.QueryAsync(query);

            // assert
            Assert.That(ruleBreaches, Has.Count.EqualTo(1));

            var ruleBreach = ruleBreaches[0];
            Assert.That(ruleBreach.Id, Is.EqualTo(5));
            Assert.That(ruleBreach.RuleId, Is.EqualTo("abc"));

            Assert.That(ruleBreach.Orders, Has.Count.EqualTo(2));
            var order0 = ruleBreach.Orders[0];
            Assert.That(order0.Id, Is.EqualTo(8));
            Assert.That(order0.LimitPrice, Is.EqualTo(6.78m));
            var order1 = ruleBreach.Orders[1];
            Assert.That(order1.Id, Is.EqualTo(9));
            Assert.That(order1.LimitPrice, Is.EqualTo(3.45m));
        }
    }
}
