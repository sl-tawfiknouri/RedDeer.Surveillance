using FakeItEasy;
using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Threading;
using Utilities.Network_IO.Websocket_Connections;
using Utilities.Network_IO.Websocket_Connections.Interfaces;

namespace Utilities.Tests.Network_IO.Websocket_Connections
{
    [TestFixture]
    public class NetworkSwitchTests
    {
        private INetworkTrunk _networkTrunk;
        private INetworkFailover _networkFailover;

        [SetUp]
        public void Setup()
        {
            _networkTrunk = A.Fake<INetworkTrunk>();
            _networkFailover = A.Fake<INetworkFailover>();
        }

        [Test]
        public void Constructor_ConsidersNullTrunkArgument_ToBeExceptional()
        {
            Assert.Throws<ArgumentNullException>(() => new NetworkSwitch(null, _networkFailover));
        }

        [Test]
        public void Constructor_ConsidersNullFailOverArgument_ToBeExceptional()
        {
            Assert.Throws<ArgumentNullException>(() => new NetworkSwitch(_networkTrunk, null));
        }

        [Test]
        public void Add_AddNullValue_DoesNotCallFailoverOrTrunk()
        {
            var networkSwitch = new NetworkSwitch(_networkTrunk, _networkFailover);

            networkSwitch.Add<string>(null);

            A.CallTo(() => _networkTrunk.Send(A<string>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => _networkFailover.Store(A<string>.Ignored)).MustNotHaveHappened();
        }

        [Test]
        public void Add_AddValue_DoesCallFailoverOnlyIfNotActive()
        {
            var networkSwitch = new NetworkSwitch(_networkTrunk, _networkFailover);
            
            networkSwitch.Add("test");

            A.CallTo(() => _networkTrunk.Send(A<string>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => _networkFailover.Store(A<string>.Ignored)).MustHaveHappened();
        }

        [Test]
        public void Add_AddValue_DoesNotCallFailoverIfActiveAndSent()
        {
            var networkSwitch = new NetworkSwitch(_networkTrunk, _networkFailover);
            A.CallTo(() => _networkTrunk.Active).Returns(true);
            A.CallTo(() => _networkTrunk.Send(A<string>.Ignored)).Returns(true);

            networkSwitch.Add("test");

            A.CallTo(() => _networkTrunk.Send(A<string>.Ignored)).MustHaveHappened();
            A.CallTo(() => _networkFailover.Store(A<string>.Ignored)).MustNotHaveHappened();
        }

        [Test]
        public void Add_AddValue_FailoverIfActiveTrunkButFailsToSend()
        {
            var networkSwitch = new NetworkSwitch(_networkTrunk, _networkFailover);
            A.CallTo(() => _networkTrunk.Active).Returns(true);

            networkSwitch.Add("test");

            A.CallTo(() => _networkTrunk.Send(A<string>.Ignored)).MustHaveHappened();
            A.CallTo(() => _networkFailover.Store(A<string>.Ignored)).MustHaveHappened();
        }

        [Test]
        [Explicit]
        [Description("Manually verify this; fake framework doesn't work for verifying a complex test")]
        public void Add_WithoutActiveWebsocket_CauseFailover()
        {
            var failover = new NetworkFailoverLocalMemory();
            var networkSwitch = new NetworkSwitch(_networkTrunk, failover, 1);

            networkSwitch.Add("test");

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