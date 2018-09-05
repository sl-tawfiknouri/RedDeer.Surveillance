using System;
using NUnit.Framework;
using Utilities.Network_IO.Websocket_Connections;

namespace Utilities.Tests.Network_IO.Websocket_Connections
{
    [TestFixture]
    public class WebsocketConnectionFactoryTests
    {
        [Test]
        public void Build_WithNullConnection_ThrowsException()
        {
            var factory = new WebsocketConnectionFactory();

            Assert.Throws<ArgumentNullException>(() => factory.Build(null));
        }

        [Test]
        public void Build_WithEmptyConnection_ThrowsException()
        {
            var factory = new WebsocketConnectionFactory();

            Assert.Throws<ArgumentNullException>(() => factory.Build(string.Empty));
        }

        [Test]
        public void Build_WithInvalidConnectionString_ThrowsException()
        {
            var factory = new WebsocketConnectionFactory();

            Assert.Throws<ArgumentException>(() => factory.Build("a-connection"));
        }

        [Test]
        public void Build_WithValidConnectionString_ReturnsItem()
        {
            var factory = new WebsocketConnectionFactory();

            var result = factory.Build("ws://localhost.com:8008");

            Assert.IsNotNull(result);
        }
    }
}