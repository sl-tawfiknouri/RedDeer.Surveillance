using System;
using NUnit.Framework;
using Utilities.Network_IO.Websocket_Hosts;

namespace Utilities.Tests.Network_IO.Websocket_Hosts
{
    [TestFixture]
    public class WebsocketHostFactoryTests
    {
        [Test]
        public void Build_HandlesNullString_WithEmpty()
        {
            var factory = new WebsocketHostFactory();

            Assert.Throws<ArgumentException>(() => factory.Build(null));
        }
    }
}
