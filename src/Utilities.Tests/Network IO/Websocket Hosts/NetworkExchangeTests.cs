using System;
using FakeItEasy;
using Fleck;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Utilities.Network_IO.Websocket_Hosts;
using Utilities.Network_IO.Websocket_Hosts.Interfaces;

namespace Utilities.Tests.Network_IO.Websocket_Hosts
{
    [TestFixture]
    public class NetworkExchangeTests
    {
        private IWebsocketHostFactory _hostFactory;
        private IWebsocketHost _websocketHost;
        private INetworkDuplexer _networkDuplexer;
        private ILogger<NetworkExchange> _logger;

        [SetUp]
        public void Setup()
        {
            _hostFactory = A.Fake<IWebsocketHostFactory>();
            _networkDuplexer = A.Fake<INetworkDuplexer>();
            _logger = A.Fake<ILogger<NetworkExchange>>();
            _websocketHost = A.Fake<IWebsocketHost>();
        }

        [Test]
        public void Constructor_NullWebsocketHostFactory_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => new NetworkExchange(null, _networkDuplexer, _logger));
        }

        [Test]
        public void Constructor_NullNetworkDuplexer_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => new NetworkExchange(_hostFactory, null, _logger));
        }

        [Test]
        public void Constructor_NullLogger_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => new NetworkExchange(_hostFactory, _networkDuplexer, null));
        }

        [Test]
        public void TerminateConnection_DoesNotThrow_IfCalledBeforeInitialise()
        {
            var networkExchange = new NetworkExchange(_hostFactory, _networkDuplexer, _logger);

            Assert.DoesNotThrow(() => networkExchange.TerminateConnections());
        }

        [Test]
        public void Initialise_NullHostUrl_LogsError()
        {
            var networkExchange = new NetworkExchange(_hostFactory, _networkDuplexer, _logger);

            networkExchange.Initialise(null);

            A.CallTo(() => _hostFactory.Build(A<string>.Ignored)).MustNotHaveHappened();
        }

        [Test]
        public void Initialise_OpensAndCloses_IfTerminated()
        {
            var exchange = new NetworkExchange(_hostFactory, _networkDuplexer, _logger);
            A.CallTo(() => _hostFactory.Build(A<string>.Ignored)).Returns(_websocketHost);
            
            exchange.Initialise("ws://0.0.0.0:0001");

            A.CallTo(() => _hostFactory.Build("ws://0.0.0.0:0001")).MustHaveHappenedOnceExactly();
            A.CallTo(() => _websocketHost.Start(A<Action<IWebSocketConnection>>.Ignored)).MustHaveHappenedOnceExactly();

            exchange.TerminateConnections();

            A.CallTo(() => _websocketHost.Dispose()).MustHaveHappenedOnceExactly();
        }
    }
}
