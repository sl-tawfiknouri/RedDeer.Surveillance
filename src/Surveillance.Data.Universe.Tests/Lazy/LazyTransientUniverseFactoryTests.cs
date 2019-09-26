namespace Surveillance.Data.Universe.Tests.Lazy
{
    using System;

    using Domain.Surveillance.Scheduling;

    using FakeItEasy;

    using NUnit.Framework;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Data.Universe.Interfaces;
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
        /// The universe builder.
        /// </summary>
        private IUniverseBuilder universeBuilder;

        /// <summary>
        /// The build returns universe.
        /// </summary>
        [Test]
        public void BuildReturnsUniverse()
        {
            var factory = new LazyTransientUniverseFactory(this.universeBuilder, this.dataManifestInterpreter);
            var execution = new ScheduledExecution();

            var universe = factory.Build(execution, this.operationContext);

            Assert.IsNotNull(universe);
        }

        /// <summary>
        /// The constructor null scheduled executioner throws exception.
        /// </summary>
        [Test]
        public void ConstructorNullScheduledExecutionerThrowsException()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new LazyTransientUniverseFactory(this.universeBuilder, null));
        }

        /// <summary>
        /// The constructor null universe builder throws exception.
        /// </summary>
        [Test]
        public void ConstructorNullUniverseBuilderThrowsException()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new LazyTransientUniverseFactory(null, this.dataManifestInterpreter));
        }

        /// <summary>
        /// The setup.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.universeBuilder = A.Fake<IUniverseBuilder>();
            this.operationContext = A.Fake<ISystemProcessOperationContext>();
            this.dataManifestInterpreter = A.Fake<IDataManifestInterpreter>();
        }
    }
}