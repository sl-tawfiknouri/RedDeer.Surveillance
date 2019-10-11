namespace Surveillance.Reddeer.TickPriceHistoryClientTests
{
    using System;

    using Firefly.Service.Data.TickPriceHistory.Shared.Protos;

    using Google.Protobuf.WellKnownTypes;

    using Grpc.Core;

    using NUnit.Framework;

    [TestFixture]
    [Explicit("integration tests")]
    public class TickPriceHistoryClientTests
    {
        private TickPriceHistoryService.TickPriceHistoryServiceClient client;

        [SetUp]
        public void Setup()
        {
            var channel = new Channel("localhost", 8085, ChannelCredentials.Insecure);
            this.client = new TickPriceHistoryService.TickPriceHistoryServiceClient(channel);
        }

        [Test]
        public void SecurityTimeBarQuerySentOverGrpcIsOk()
        {
            var req = new SecurityTimeBarQueryRequest()
              {
                  Subqueries =
                  {
                      new SecurityTimeBarSubqueryRequest
                      {
                          StartUtc = new DateTime(2019, 10, 01, 18, 00, 00).ToUniversalTime().ToTimestamp(),
                          EndUtc = new DateTime(2019, 10, 01, 23, 00, 00).ToUniversalTime().ToTimestamp(),
                          PolicyOptions = TimeBarPolicyOptions.OneHour,
                          ReferenceId = "test",
                          Identifiers = new SecurityIdentifiers()
                            {
                                Isin = "USP80557BV53",
                                Ric = "UYE28EY002=UE"
                            }
                     }
                  }
              };

            var queryResult = this.client.QuerySecurityTimeBars(req);
            var inspectResult = queryResult;
        }

        [Test]
        public void DeleteSecuritySynchronisationCommandSentOverGrpcIsOk()
        {
            var deleteSynchronisationCommand = new DeleteDataSynchronisationRequest
               {
                    RequestOriginApplication = "Surveillance-Engine",
                    RequestOriginClientName = "LionTrust",
                    RequestOriginMachineId = Environment.MachineName,
                    Identifiers = new SecurityIdentifiers
                      {
                          Isin = "USP80557BV53",
                          Ric = "UYE28EY002=UE"
                      },
                    PolicyOptions = { new TimeBarPolicyOptions[] { TimeBarPolicyOptions.OneHour } }
            };

            var commandResult = this.client.DeleteDataSynchronisation(deleteSynchronisationCommand);
            var inspectResult = commandResult;
        }

        [Test]
        public void PutDataSynchronisationCommandSentOverGrpcIsOk()
        {
            var putDataSynchronisationRequest = new DataSynchronisationRequest
            {
                StartUtc = new DateTime(2019, 10, 01, 18, 00, 00).ToUniversalTime().ToTimestamp(),
                EndUtc = new DateTime(2019, 10, 01, 23, 00, 00).ToUniversalTime().ToTimestamp(),
                RequestOriginApplication = "Surveillance-Engine",
                RequestOriginClientName = "LionTrust",
                RequestOriginMachineId = Environment.MachineName,
                Identifiers = new SecurityIdentifiers
                  {
                      Isin = "USP80557BV53",
                      Ric = "UYE28EY002=UE"
                  }
            };

            var commandResult = this.client.PutDataSynchronisation(putDataSynchronisationRequest);
            var inspectResult = commandResult;
        }

        [Test]
        public void PutSecuritySynchronisationCommandSentOverGrpcIsOk()
        {
            var putSecuritySynchronisationRequest = new SecuritySynchronisationRequest
                {
                    RequestOriginApplication = "Surveillance-Engine",
                    RequestOriginClientName = "LionTrust",
                    RequestOriginMachineId = Environment.MachineName,
                    Identifiers =
                        new SecurityIdentifiers
                            {
                                Isin = "USP80557BV53", Ric = "UYE28EY002=UE"
                            },
                    PolicyOptions =
                        {
                            new[]
                                {
                                    TimeBarPolicyOptions.OneHour
                                }
                        }
                };

            var commandResult = this.client.PutSecuritySynchronisation(putSecuritySynchronisationRequest);
            var inspectResult = commandResult;
        }
    }
}