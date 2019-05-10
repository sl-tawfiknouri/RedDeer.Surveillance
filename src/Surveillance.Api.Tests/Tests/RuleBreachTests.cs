using NUnit.Framework;
using Surveillance.Api.Client.Queries;
using Surveillance.Api.DataAccess.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Surveillance.Api.Tests.Tests
{
    public class RuleBreachTests : BaseTest
    {
        [Test]
        public async Task CanRequest_RuleBreach_AllFields()
        {
            // arrange
            _dbContext.DbRuleBreaches.Add(new RuleBreach
            {
                Id = 5,
                StartOfPeriodUnderInvestigation = new DateTime(2020, 01, 01, 01, 01, 01, DateTimeKind.Utc),
                // end date in british summer time, to check that using GB formatting doesn't cause any timezone changes
                EndOfPeriodUnderInvestigation = new DateTime(2020, 06, 03, 05, 06, 07, DateTimeKind.Utc),
                ReddeerEnrichmentId = "VOD LN",
                Title = "Test Rule Breach",
                Description = "This is a description",
                Venue = "London",
                AssetCfi = "Equity",
                CreatedOn = new DateTime(2020, 03, 01, 12, 12, 12),
                RuleId = "abc",
                CorrelationId = "xyz123",
                IsBackTest = true,
                SystemOperationId = 789
            });
            await _dbContext.SaveChangesAsync();

            var query = new RuleBreachQuery();
            query
                .Filter
                    .Node
                        .FieldId()
                        .FieldStartOfPeriodUnderInvestigation()
                        .FieldEndOfPeriodUnderInvestigation()
                        .FieldReddeerEnrichmentId()
                        .FieldTitle()
                        .FieldDescription()
                        .FieldVenue()
                        .FieldAssetCfi()
                        .FieldCreatedOn()
                        .FieldRuleId()
                        .FieldCorrelationId()
                        .FieldIsBackTest()
                        .FieldSystemOperationId();

            // act
            var ruleBreaches = await _apiClient.QueryAsync(query, CancellationToken.None);

            // assert
            Assert.That(ruleBreaches, Has.Count.EqualTo(1));

            var ruleBreach = ruleBreaches[0];
            Assert.That(ruleBreach.Id, Is.EqualTo(5));
            Assert.That(ruleBreach.StartOfPeriodUnderInvestigation, Is.EqualTo(new DateTime(2020, 01, 01, 01, 01, 01, DateTimeKind.Utc)));
            Assert.That(ruleBreach.EndOfPeriodUnderInvestigation, Is.EqualTo(new DateTime(2020, 06, 03, 05, 06, 07, DateTimeKind.Utc)));
            Assert.That(ruleBreach.ReddeerEnrichmentId, Is.EqualTo("VOD LN"));
            Assert.That(ruleBreach.Title, Is.EqualTo("Test Rule Breach"));
            Assert.That(ruleBreach.Description, Is.EqualTo("This is a description"));
            Assert.That(ruleBreach.Venue, Is.EqualTo("London"));
            Assert.That(ruleBreach.AssetCfi, Is.EqualTo("Equity"));
            Assert.That(ruleBreach.CreatedOn, Is.EqualTo(new DateTime(2020, 03, 01, 12, 12, 12)));
            Assert.That(ruleBreach.RuleId, Is.EqualTo("abc"));
            Assert.That(ruleBreach.CorrelationId, Is.EqualTo("xyz123"));
            Assert.That(ruleBreach.IsBackTest, Is.EqualTo(true));
            Assert.That(ruleBreach.SystemOperationId, Is.EqualTo(789));
            Assert.That(ruleBreach.Orders, Is.Null);
        }

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
                .Filter
                    .Node
                        .FieldId()
                        .FieldRuleId();

            // act
            var ruleBreaches = await _apiClient.QueryAsync(query, CancellationToken.None);

            // assert
            Assert.That(ruleBreaches, Has.Count.EqualTo(2));

            var ruleBreach0 = ruleBreaches.Find(x => x.Id == 5);
            Assert.That(ruleBreach0.RuleId, Is.EqualTo("abc"));
            Assert.That(ruleBreach0.Orders, Is.Null);

            var ruleBreach1 = ruleBreaches.Find(x => x.Id == 6);
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
            _dbContext.DbRuleBreaches.Add(new RuleBreach // a rule breach which shouldn't be found
            {
                Id = 6,
                RuleId = "xyz"
            });
            _dbContext.DbOrders.Add(new Order // an order which shouldn't be found
            {
                Id = 7,
                LimitPrice = 1.78m
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
            _dbContext.DbRuleBreachOrders.Add(new RuleBreachOrder // a rule breach order which shouldn't be found
            {
                RuleBreachId = 4,
                OrderId = 7
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
                .Filter
                    .ArgumentId(5)
                    .Node
                        .FieldId()
                        .FieldRuleId()
                        .FieldOrders()
                            .FieldId()
                            .FieldLimitPrice();

            // act
            var ruleBreaches = await _apiClient.QueryAsync(query, CancellationToken.None);

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
