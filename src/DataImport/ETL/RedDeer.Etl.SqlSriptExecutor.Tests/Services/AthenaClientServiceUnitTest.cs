using Amazon.Athena;
using Amazon.Athena.Model;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using RedDeer.Etl.SqlSriptExecutor.Services;
using RedDeer.Etl.SqlSriptExecutor.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace RedDeer.Etl.SqlSriptExecutor.Tests.Services
{
    public class AthenaClientServiceUnitTest
    {
        private AthenaClientService _athenaClientService = null;

        private IAmazonAthena _amazonAthena = null;
        private IAmazonAthenaClientFactory _amazonAthenaClientFactory = null;

        [SetUp]
        public void SetUp()
        {
            _amazonAthenaClientFactory = A.Fake<IAmazonAthenaClientFactory>();
            _amazonAthena = A.Fake<IAmazonAthena>();

            A.CallTo(() => _amazonAthenaClientFactory.Create())
                .Returns(_amazonAthena);

            _athenaClientService = new AthenaClientService(_amazonAthenaClientFactory, new NullLogger<AthenaClientService>());
        }

        [Test]
        public async Task StartQueryExecutionAsync_WhenCalled_ShouldExecuteAndReturnQueryExecutionId()
        {
            var database = "test-db-name";
            var queryString = "query-string";
            var outputLocation = "output-location";

            var queryExecutionId = Guid.NewGuid().ToString();

            A.CallTo(() => _amazonAthena.StartQueryExecutionAsync(
                A<StartQueryExecutionRequest>.That.Matches(
                    m => m.QueryString == queryString && m.QueryExecutionContext.Database == database && m.ResultConfiguration.OutputLocation == outputLocation), CancellationToken.None))
                .Returns(Task.FromResult(new StartQueryExecutionResponse { HttpStatusCode = HttpStatusCode.OK, QueryExecutionId = queryExecutionId }));

            var result = await _athenaClientService.StartQueryExecutionAsync(database, queryString, outputLocation);

            result.Should().Be(queryExecutionId);
        }

        [Test]
        public async Task BatchPoolQueryExecutionAsync_WhenSucceeds()
        {
            var queryExecutions = new List<QueryExecution>
            {
                new QueryExecution { QueryExecutionId = "1", Status = new QueryExecutionStatus { State = QueryExecutionState.SUCCEEDED } }
            };

            var queryExecutionId = queryExecutions.Select(s => s.QueryExecutionId).ToList();

            A.CallTo(() => _amazonAthena.BatchGetQueryExecutionAsync(
               A<BatchGetQueryExecutionRequest>.That.Matches(m => m.QueryExecutionIds.SequenceEqual(queryExecutionId)), CancellationToken.None))
               .Returns(Task.FromResult(new BatchGetQueryExecutionResponse { HttpStatusCode = HttpStatusCode.OK, QueryExecutions = queryExecutions }));

            await _athenaClientService.BatchPoolQueryExecutionAsync(queryExecutionId, 1);

            A.CallTo(() => _amazonAthena.BatchGetQueryExecutionAsync(
               A<BatchGetQueryExecutionRequest>.Ignored, CancellationToken.None))
               .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void BatchPoolQueryExecutionAsync_WhenUnsuccessfull_ThrowsException()
        {
            var queryExecutions = new List<QueryExecution>
            {
                new QueryExecution { QueryExecutionId = "1", Status = new QueryExecutionStatus { State = QueryExecutionState.CANCELLED, StateChangeReason = "R1" } , Statistics = new QueryExecutionStatistics { } },
                new QueryExecution { QueryExecutionId = "2", Status = new QueryExecutionStatus { State = QueryExecutionState.FAILED, StateChangeReason = "R2" }, Statistics = new QueryExecutionStatistics { } }
            };

            var queryExecutionId = queryExecutions.Select(s => s.QueryExecutionId).ToList();

            A.CallTo(() => _amazonAthena.BatchGetQueryExecutionAsync(
               A<BatchGetQueryExecutionRequest>.That.Matches(m => m.QueryExecutionIds.SequenceEqual(queryExecutionId)), CancellationToken.None))
               .Returns(Task.FromResult(new BatchGetQueryExecutionResponse { HttpStatusCode = HttpStatusCode.OK, QueryExecutions = queryExecutions }));

            var aggregateException = Assert.ThrowsAsync<AggregateException>(async () => await _athenaClientService.BatchPoolQueryExecutionAsync(queryExecutionId, 1));

            var messages = queryExecutions.Select(queryExecution => $"State '{queryExecution.Status.State}', StateChangeReason: '{queryExecution.Status.StateChangeReason}'.").ToList();

            aggregateException.Message.Should().Be($"BatchPoolQueryExecutionAsync for 'queryExecutionIds': '{string.Join(", ", queryExecutions.Select(s => s.QueryExecutionId))}' failed. {string.Join(" ", messages.Select(s => $"({s})") )}");
            CollectionAssert.AreEqual(messages, aggregateException.InnerExceptions.Select(s => s.Message).ToList());

            A.CallTo(() => _amazonAthena.BatchGetQueryExecutionAsync(
               A<BatchGetQueryExecutionRequest>.Ignored, CancellationToken.None))
               .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task BatchPoolQueryExecutionAsync_WhenInProgress_ChecksStatusAgaing()
        {
            var queryExecutions1 = new List<QueryExecution>
            {
                new QueryExecution { QueryExecutionId = "1", Status = new QueryExecutionStatus { State = QueryExecutionState.RUNNING }},
                new QueryExecution { QueryExecutionId = "2", Status = new QueryExecutionStatus { State = QueryExecutionState.QUEUED }}
            };

            var queryExecutions2 = new List<QueryExecution>
            {
                new QueryExecution { QueryExecutionId = "1", Status = new QueryExecutionStatus { State = QueryExecutionState.SUCCEEDED }},
                new QueryExecution { QueryExecutionId = "2", Status = new QueryExecutionStatus { State = QueryExecutionState.SUCCEEDED }}
            };

            var queryExecutionId = queryExecutions1.Select(s => s.QueryExecutionId).ToList();

            A.CallTo(() => _amazonAthena.BatchGetQueryExecutionAsync(
               A<BatchGetQueryExecutionRequest>.That.Matches(m => m.QueryExecutionIds.SequenceEqual(queryExecutionId)), CancellationToken.None))
               .ReturnsNextFromSequence(
                    Task.FromResult(new BatchGetQueryExecutionResponse { HttpStatusCode = HttpStatusCode.OK, QueryExecutions = queryExecutions1 }),
                    Task.FromResult(new BatchGetQueryExecutionResponse { HttpStatusCode = HttpStatusCode.OK, QueryExecutions = queryExecutions2 })
                );

            await _athenaClientService.BatchPoolQueryExecutionAsync(queryExecutionId, 1);

            A.CallTo(() => _amazonAthena.BatchGetQueryExecutionAsync(
               A<BatchGetQueryExecutionRequest>.That.Matches(m => m.QueryExecutionIds.SequenceEqual(queryExecutionId)), CancellationToken.None))
               .MustHaveHappenedTwiceExactly();
        }
    }
}
