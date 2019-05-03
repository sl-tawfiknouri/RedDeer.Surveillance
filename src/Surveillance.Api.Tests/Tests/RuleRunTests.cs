using NUnit.Framework;
using Surveillance.Api.Client.Queries;
using Surveillance.Api.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Surveillance.Api.Tests.Tests
{
    public class RuleRunTests : BaseTest
    {
        [Test]
        public async Task CanRequest_RuleRuns_ByCorrelationIds()
        {
            // arrange
            _dbContext.DbRuleRuns.Add(new SystemProcessOperationRuleRun
            {
                Id = 3,
                CorrelationId = "abc"
            });
            _dbContext.DbRuleRuns.Add(new SystemProcessOperationRuleRun
            {
                Id = 4,
                CorrelationId = "abc"
            });
            _dbContext.DbRuleRuns.Add(new SystemProcessOperationRuleRun // a rule run which shouldn't be found
            {
                Id = 5,
                CorrelationId = "xyz"
            });
            await _dbContext.SaveChangesAsync();

            var query = new RuleRunQuery();
            query
                .RuleRunNode
                    .ArgumentCorrelationIds(new string[] { "abc" })
                    .FieldId()
                    .FieldCorrelationId();

            // act
            var ruleRuns = await _apiClient.QueryAsync(query);

            // assert
            Assert.That(ruleRuns, Has.Count.EqualTo(2));
        }
    }
}
