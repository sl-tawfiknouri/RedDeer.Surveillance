using System;
using System.Threading;
using FakeItEasy;
using NUnit.Framework;
using Utilities.Network_IO.Interfaces;
using Utilities.Network_IO.Websocket_Connections;
using Utilities.Network_IO.Websocket_Connections.Interfaces;

namespace Utilities.Tests.Network_IO.Websocket_Connections
{
    [TestFixture]
    public class NetworkTrunkTests
    {
        private IWebsocketConnectionFactory _connectionFactory;
        private IMessageWriter _messageWriter;

        [SetUp]
        public void Setup()
        {
            _connectionFactory = A.Fake<IWebsocketConnectionFactory>();
            _messageWriter = A.Fake<IMessageWriter>();
        }

        [Test]
        public void Constructor_ThrowsForNull_WebSocketFactory()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new NetworkTrunk(null, _messageWriter));
        }

        [Test]
        public void Constructor_ThrowsForNull_MessageWriter()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new NetworkTrunk(_connectionFactory, null));
        }

        [Test]
        public void Initiate_NullDomain_ThrowsException()
        {
            var networkTrunk = new NetworkTrunk(_connectionFactory, _messageWriter);

            Assert.Throws<ArgumentNullException>(() => networkTrunk.Initiate(null, "port", new CancellationToken()));
        }

        [Test]
        public void Initiate_NullPort_ThrowsException()
        {
            var networkTrunk = new NetworkTrunk(_connectionFactory, _messageWriter);

            Assert.Throws<ArgumentNullException>(() => networkTrunk.Initiate("domain", null, new CancellationToken()));
        }

        [Test]
        public void Send_WithNullArgument_ReturnsFalse()
        {
            var networkTrunk = new NetworkTrunk(_connectionFactory, _messageWriter);

            Assert.IsFalse(networkTrunk.Send((string)null));
        }
    }
}
