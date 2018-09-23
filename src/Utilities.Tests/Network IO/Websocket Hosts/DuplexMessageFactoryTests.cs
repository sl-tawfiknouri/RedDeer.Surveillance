using NUnit.Framework;
using Utilities.Network_IO.Websocket_Hosts;

namespace Utilities.Tests.Network_IO.Websocket_Hosts
{
    [TestFixture]
    public class DuplexMessageFactoryTests
    {
        [Test]
        public void Create_NullValue_ReturnsEmptyMessage()
        {
            var factory = new DuplexMessageFactory();

            var msg = factory.Create<string>(MessageType.ReddeerTradeFormat, null);
            
            Assert.AreEqual(msg.Message, string.Empty);
        }

        [Test]
        public void Create_DuplexedMessage_ReturnsMessageWithSameType()
        {
            const string expectedResult = @"""test""";

            var factory = new DuplexMessageFactory();

            var msg = factory.Create(MessageType.ReddeerTradeFormat, "test");

            Assert.AreEqual(msg.Message, expectedResult);
            Assert.AreEqual(msg.Type, MessageType.ReddeerTradeFormat);
        }
    }
}