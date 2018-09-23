using FakeItEasy;
using NUnit.Framework;
using System;
using System.Diagnostics;
using Utilities.Network_IO.Websocket_Connections;
using Utilities.Network_IO.Websocket_Connections.Interfaces;

namespace Utilities.Tests.Network_IO.Websocket_Connections
{
    [TestFixture]
    public class NetworkSwitchTests
    {
        private INetworkTrunk _networkTrunk;
        private INetworkFailOver _networkFailOver;

        [SetUp]
        public void Setup()
        {
            _networkTrunk = A.Fake<INetworkTrunk>();
            _networkFailOver = A.Fake<INetworkFailOver>();
        }

        [Test]
        public void Constructor_ConsidersNullTrunkArgument_ToBeExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new NetworkSwitch(null, _networkFailOver));
        }

        [Test]
        public void Constructor_ConsidersNullFailOverArgument_ToBeExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new NetworkSwitch(_networkTrunk, null));
        }

        [Test]
        public void Add_AddNullValue_DoesNotCallFailOverOrTrunk()
        {
            var networkSwitch = new NetworkSwitch(_networkTrunk, _networkFailOver);

            networkSwitch.Send<string>(null);

            A.CallTo(() => _networkTrunk.Send(A<string>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => _networkFailOver.Store(A<string>.Ignored)).MustNotHaveHappened();
        }

        [Test]
        public void Add_AddValue_DoesCallFailOverOnlyIfNotActive()
        {
            var networkSwitch = new NetworkSwitch(_networkTrunk, _networkFailOver);
            
            networkSwitch.Send("test");

            A.CallTo(() => _networkTrunk.Send(A<string>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => _networkFailOver.Store(A<string>.Ignored)).MustHaveHappened();
        }

        [Test]
        public void Add_AddValue_DoesNotCallFailOverIfActiveAndSent()
        {
            var networkSwitch = new NetworkSwitch(_networkTrunk, _networkFailOver);
            A.CallTo(() => _networkTrunk.Active).Returns(true);
            A.CallTo(() => _networkTrunk.Send(A<string>.Ignored)).Returns(true);

            networkSwitch.Send("test");

            A.CallTo(() => _networkTrunk.Send(A<string>.Ignored)).MustHaveHappened();
            A.CallTo(() => _networkFailOver.Store(A<string>.Ignored)).MustNotHaveHappened();
        }

        [Test]
        public void Add_AddValue_FailOverIfActiveTrunkButFailsToSend()
        {
            var networkSwitch = new NetworkSwitch(_networkTrunk, _networkFailOver);
            A.CallTo(() => _networkTrunk.Active).Returns(true);

            networkSwitch.Send("test");

            A.CallTo(() => _networkTrunk.Send(A<string>.Ignored)).MustHaveHappened();
            A.CallTo(() => _networkFailOver.Store(A<string>.Ignored)).MustHaveHappened();
        }

        [Test]
        [Explicit]
        [Description("Manually verify this; fake framework doesn't work for verifying a complex test")]
        public void Add_WithoutActiveWebsocket_CauseFailOver()
        {
            var failOver = new NetworkFailOverLocalMemory();
            var networkSwitch = new NetworkSwitch(_networkTrunk, failOver, 1);

            networkSwitch.Send("test");

            A.CallTo(() => _networkTrunk.Send(A<string>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => _networkTrunk.Active).Returns(true);
            A.CallTo(() => _networkTrunk.Send(A<string>.Ignored)).Returns(true);

            var timer = new Stopwatch();
            timer.Start();

            while (timer.ElapsedMilliseconds < 5000)
            { }
        }
    }
}