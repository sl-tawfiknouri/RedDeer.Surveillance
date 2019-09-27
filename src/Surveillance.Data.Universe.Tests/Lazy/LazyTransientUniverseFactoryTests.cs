namespace Surveillance.Data.Universe.Tests.Lazy
{
    using System;

    using Domain.Surveillance.Scheduling;

    using FakeItEasy;

    using NUnit.Framework;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Data.Universe.Lazy;
    using Surveillance.Data.Universe.Lazy.Builder.Interfaces;

    /// <summary>
    /// The lazy transient universe factory tests.
    /// </summary>
    [TestFixture]
    public class LazyTransientUniverseFactoryTests
    {
        /// <summary>
        /// The operation context.
        /// </summary>
        private ISystemProcessOperationContext operationContext;

        /// <summary>
        /// The data manifest interpreter.
        /// </summary>
        private IDataManifestInterpreter dataManifestInterpreter;

        /// <summary>
        /// The build returns universe.
        /// </summary>
        [Test]
        public void BuildReturnsUniverse()
        {
            var factory = new LazyTransientUniverseFactory(this.dataManifestInterpreter);
            var execution = new ScheduledExecution();

            var universe = factory.Build(execution, this.operationContext);

            Assert.IsNotNull(universe);
        }

        /// <summary>
        /// The constructor null manifest interpreter throws exception.
        /// </summary>
        [Test]
        public void ConstructorNullManifestInterpreterThrowsException()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new LazyTransientUniverseFactory(null));
        }

        /// <summary>
        /// The setup.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.operationContext = A.Fake<ISystemProcessOperationContext>();
            this.dataManifestInterpreter = A.Fake<IDataManifestInterpreter>();
        }
    }
}