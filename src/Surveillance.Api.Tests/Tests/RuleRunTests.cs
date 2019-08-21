namespace Surveillance.Api.Tests.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using NUnit.Framework;

    using RedDeer.Surveillance.Api.Client.Queries;

    using Surveillance.Api.DataAccess.Entities;

    public class RuleRunTests : BaseTest
    {
        [Test]
        public async Task CanRequest_RuleRun_ProcessOperation_DateTimeCanBeNull()
        {
            // arrange
            this._dbContext.DbRuleRuns.Add(new SystemProcessOperationRuleRun { Id = 3, SystemProcessOperationId = 7 });
            this._dbContext.DbProcessOperations.Add(new SystemProcessOperation { Id = 7 });
            await this._dbContext.SaveChangesAsync();

            var query = new RuleRunQuery();
            query.Filter.Node.FieldProcessOperation().FieldOperationEnd();

            // act
            var ruleRuns = await this._apiClient.QueryAsync(query, CancellationToken.None);

            // assert
            Assert.That(ruleRuns, Has.Count.EqualTo(1));
            var ruleRun = ruleRuns[0];
            Assert.That(ruleRun.ProcessOperation.OperationEnd, Is.EqualTo(null));
        }

        [Test]
        public async Task CanRequest_RuleRun_WithProcessOperation_AllFields()
        {
            // arrange
            this._dbContext.DbRuleRuns.Add(
                new SystemProcessOperationRuleRun
                    {
                        Id = 3,
                        CorrelationId = "abc",
                        RuleDescription = "this is a description",
                        RuleVersion = "1.2.3",
                        ScheduleRuleStart = new DateTime(2020, 04, 05, 03, 45, 22, DateTimeKind.Utc),
                        ScheduleRuleEnd = new DateTime(2020, 04, 06, 04, 46, 23, DateTimeKind.Utc),
                        SystemProcessOperationId = 7
                    });
            this._dbContext.DbProcessOperations.Add(
                new SystemProcessOperation
                    {
                        Id = 7,
                        OperationStart = new DateTime(2020, 07, 05, 03, 45, 22, DateTimeKind.Utc),
                        OperationEnd = new DateTime(2020, 07, 06, 03, 45, 22, DateTimeKind.Utc),
                        OperationState = 99
                    });
            this._dbContext.DbProcessOperations.Add(
                new SystemProcessOperation
                    {
                        // system process operation not to be found
                        Id = 8
                    });
            await this._dbContext.SaveChangesAsync();

            var query = new RuleRunQuery();
            query.Filter.Node.FieldId().FieldCorrelationId().FieldRuleDescription().FieldRuleVersion()
                .FieldScheduleRuleStart().FieldScheduleRuleEnd().FieldProcessOperation().FieldId().FieldOperationStart()
                .FieldOperationEnd().FieldOperationState();

            // act
            var ruleRuns = await this._apiClient.QueryAsync(query, CancellationToken.None);

            // assert
            Assert.That(ruleRuns, Has.Count.EqualTo(1));
            var ruleRun = ruleRuns[0];
            Assert.That(ruleRun.Id, Is.EqualTo(3));
            Assert.That(ruleRun.CorrelationId, Is.EqualTo("abc"));
            Assert.That(ruleRun.RuleDescription, Is.EqualTo("this is a description"));
            Assert.That(ruleRun.RuleVersion, Is.EqualTo("1.2.3"));
            Assert.That(
                ruleRun.ScheduleRuleStart,
                Is.EqualTo(new DateTime(2020, 04, 05, 03, 45, 22, DateTimeKind.Utc)));
            Assert.That(ruleRun.ScheduleRuleEnd, Is.EqualTo(new DateTime(2020, 04, 06, 04, 46, 23, DateTimeKind.Utc)));
            Assert.That(ruleRun.ProcessOperation.Id, Is.EqualTo(7));
            Assert.That(
                ruleRun.ProcessOperation.OperationStart,
                Is.EqualTo(new DateTime(2020, 07, 05, 03, 45, 22, DateTimeKind.Utc)));
            Assert.That(
                ruleRun.ProcessOperation.OperationEnd,
                Is.EqualTo(new DateTime(2020, 07, 06, 03, 45, 22, DateTimeKind.Utc)));
            Assert.That(ruleRun.ProcessOperation.OperationState, Is.EqualTo(99));
        }

        [Test]
        public async Task CanRequest_RuleRuns_ByCorrelationIds()
        {
            // arrange
            this._dbContext.DbRuleRuns.Add(new SystemProcessOperationRuleRun { Id = 3, CorrelationId = "abc" });
            this._dbContext.DbRuleRuns.Add(new SystemProcessOperationRuleRun { Id = 4, CorrelationId = "abc" });
            this._dbContext.DbRuleRuns.Add(new SystemProcessOperationRuleRun { Id = 5, CorrelationId = "def" });
            this._dbContext.DbRuleRuns.Add(
                new SystemProcessOperationRuleRun
                    {
                        // a rule run which shouldn't be found
                        Id = 6, CorrelationId = "xyz"
                    });
            await this._dbContext.SaveChangesAsync();

            var query = new RuleRunQuery();
            query.Filter.ArgumentCorrelationIds(new List<string> { "abc", "def" }).Node.FieldId().FieldCorrelationId();

            // act
            var ruleRuns = await this._apiClient.QueryAsync(query, CancellationToken.None);

            // assert
            Assert.That(ruleRuns, Has.Count.EqualTo(3));
        }
    }
}