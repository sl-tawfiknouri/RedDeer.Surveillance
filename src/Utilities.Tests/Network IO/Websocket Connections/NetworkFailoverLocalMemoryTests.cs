using NUnit.Framework;
using Utilities.Network_IO.Websocket_Connections;
using System.Linq;

namespace Utilities.Tests.Network_IO.Websocket_Connections
{
    [TestFixture]
    public class NetworkFailoverLocalMemoryTests
    {
        [Test]
        public void Store_StoresValue_Uneventfully()
        {
            var failover = new NetworkFailoverLocalMemory();
            var obj = new NetworkFailoverLocalMemory();

            Assert.DoesNotThrow(() => failover.Store(obj));
        }

        [Test]
        public void Store_StoresMultipleValueOfSameType_Uneventfully()
        {
            var failover = new NetworkFailoverLocalMemory();
            var obj1 = new NetworkFailoverLocalMemory();
            var obj2 = new NetworkFailoverLocalMemory();
            var obj3 = new NetworkFailoverLocalMemory();
            var obj4 = new NetworkFailoverLocalMemory();

            Assert.DoesNotThrow(() => failover.Store(obj1));
            Assert.DoesNotThrow(() => failover.Store(obj2));
            Assert.DoesNotThrow(() => failover.Store(obj3));
            Assert.DoesNotThrow(() => failover.Store(obj4));
        }

        [Test]
        public void Store_StoresMultipleValueOfDifferentType_Uneventfully()
        {
            var failover = new NetworkFailoverLocalMemory();
            var obj0 = new NetworkFailoverLocalMemory();
            var obj1 = new NetworkFailoverLocalMemory();
            var obj2 = "test";
            var obj3 = 1;
            var obj4 = 12m;
            var obj5 = new NetworkFailoverLocalMemoryTests();

            Assert.DoesNotThrow(() => failover.Store(obj0));
            Assert.DoesNotThrow(() => failover.Store(obj1));
            Assert.DoesNotThrow(() => failover.Store(obj2));
            Assert.DoesNotThrow(() => failover.Store(obj3));
            Assert.DoesNotThrow(() => failover.Store(obj4));
            Assert.DoesNotThrow(() => failover.Store(obj5));
        }

        [Test]
        public void Retrieve_RetrievesNoValues_Uneventfully()
        {
            var failover = new NetworkFailoverLocalMemory();

            var result = failover.Retrieve();

            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }

        [Test]
        public void Retrieve_RetrievesSingularValues_Uneventfully()
        {
            var failover = new NetworkFailoverLocalMemory();
            var input = new NetworkFailoverLocalMemory();

            failover.Store(input);
            var result = failover.Retrieve();

            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result);
            Assert.AreEqual(result.ToList().First().Key, typeof(NetworkFailoverLocalMemory));
            Assert.IsTrue(result.ToList().First().Value.Contains(input));
            Assert.AreEqual(result.ToList().First().Value.Count(), 1);
        }

        [Test]
        public void Retrieve_RetrievesMultipleValuesOfSingularType_Uneventfully()
        {
            var failover = new NetworkFailoverLocalMemory();
            var input1 = new NetworkFailoverLocalMemory();
            var input2 = new NetworkFailoverLocalMemory();
            var input3 = new NetworkFailoverLocalMemory();
            var input4 = new NetworkFailoverLocalMemory();

            failover.Store(input1);
            failover.Store(input2);
            failover.Store(input3);
            failover.Store(input4);
            var result = failover.Retrieve();

            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result);
            Assert.AreEqual(result.ToList().Count, 1);
            Assert.AreEqual(result.ToList().First().Key, typeof(NetworkFailoverLocalMemory));
            Assert.IsTrue(result.ToList().First().Value.Contains(input1));
            Assert.IsTrue(result.ToList().First().Value.Contains(input2));
            Assert.IsTrue(result.ToList().First().Value.Contains(input3));
            Assert.IsTrue(result.ToList().First().Value.Contains(input4));
            Assert.AreEqual(result.ToList().First().Value.Count(), 4);
        }

        [Test]
        public void Retrieve_RetrievesMultipleValuesOfMultipleTypes_Uneventfully()
        {
            var failover = new NetworkFailoverLocalMemory();

            var input1 = new NetworkFailoverLocalMemory();
            var input2 = new NetworkFailoverLocalMemory();
            var input3 = "test-1";
            var input4 = "test-2";
            var input5 = new NetworkFailoverLocalMemoryTests();
            var input6 = new NetworkFailoverLocalMemoryTests();

            failover.Store(input1);
            failover.Store(input2);
            failover.Store(input3);
            failover.Store(input4);
            failover.Store(input5);
            failover.Store(input6);
            var result = failover.Retrieve();

            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result);
            Assert.AreEqual(result.ToList().Count, 3);
            Assert.AreEqual(result.ToList().First().Key, typeof(NetworkFailoverLocalMemory));
            Assert.AreEqual(result.ToList().Skip(1).First().Key, typeof(string));
            Assert.AreEqual(result.ToList().Skip(2).First().Key, typeof(NetworkFailoverLocalMemoryTests));
            Assert.IsTrue(result.ToList().First().Value.Contains(input1));
            Assert.IsTrue(result.ToList().First().Value.Contains(input2));
            Assert.IsTrue(result.ToList().Skip(1).First().Value.Contains(input3));
            Assert.IsTrue(result.ToList().Skip(1).First().Value.Contains(input4));
            Assert.IsTrue(result.ToList().Skip(2).First().Value.Contains(input5));
            Assert.IsTrue(result.ToList().Skip(2).First().Value.Contains(input6));

            Assert.AreEqual(result.ToList().First().Value.Count(), 2);
            Assert.AreEqual(result.ToList().Skip(1).First().Value.Count(), 2);
            Assert.AreEqual(result.ToList().Skip(2).First().Value.Count(), 2);
        }

        [Test]
        public void RetrieveAndRemove_RetrievesMultipleValuesOfMultipleTypes_Uneventfully()
        {
            var failover = new NetworkFailoverLocalMemory();

            var input1 = new NetworkFailoverLocalMemory();
            var input2 = new NetworkFailoverLocalMemory();
            var input3 = "test-1";
            var input4 = "test-2";
            var input5 = new NetworkFailoverLocalMemoryTests();
            var input6 = new NetworkFailoverLocalMemoryTests();

            failover.Store(input1);
            failover.Store(input2);
            failover.Store(input3);
            failover.Store(input4);
            failover.Store(input5);
            failover.Store(input6);
            var result = failover.RetrieveAndRemove();

            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result);
            Assert.AreEqual(result.ToList().Count, 3);
            Assert.AreEqual(result.ToList().First().Key, typeof(NetworkFailoverLocalMemory));
            Assert.AreEqual(result.ToList().Skip(1).First().Key, typeof(string));
            Assert.AreEqual(result.ToList().Skip(2).First().Key, typeof(NetworkFailoverLocalMemoryTests));
            Assert.IsTrue(result.ToList().First().Value.Contains(input1));
            Assert.IsTrue(result.ToList().First().Value.Contains(input2));
            Assert.IsTrue(result.ToList().Skip(1).First().Value.Contains(input3));
            Assert.IsTrue(result.ToList().Skip(1).First().Value.Contains(input4));
            Assert.IsTrue(result.ToList().Skip(2).First().Value.Contains(input5));
            Assert.IsTrue(result.ToList().Skip(2).First().Value.Contains(input6));

            Assert.AreEqual(result.ToList().First().Value.Count(), 2);
            Assert.AreEqual(result.ToList().Skip(1).First().Value.Count(), 2);
            Assert.AreEqual(result.ToList().Skip(2).First().Value.Count(), 2);

            var result2 = failover.Retrieve();

            Assert.IsNotNull(result2);
            Assert.IsEmpty(result2);

            var input7 = new NetworkFailoverLocalMemory();

            Assert.DoesNotThrow(() => failover.Store(input7));
        }

        [Test]
        public void HasData_ReturnsFalse_WhenNoData()
        {
            var failOver = new NetworkFailoverLocalMemory();

            Assert.IsFalse(failOver.HasData());
        }

        [Test]
        public void HasData_ReturnsTrue_WhenData()
        {
            var failOver = new NetworkFailoverLocalMemory();

            failOver.Store("hello world");

            Assert.IsTrue(failOver.HasData());
        }

        [Test]
        public void HasData_ReturnsFalseThenTrueThenFalse_WhenAddingThenRemovingItem()
        {
            var failOver = new NetworkFailoverLocalMemory();

            Assert.IsFalse(failOver.HasData());

            failOver.Store("hello world");

            Assert.IsTrue(failOver.HasData());

            failOver.RemoveItem(typeof(string), "hello world");

            Assert.IsFalse(failOver.HasData());
        }
    }
}
