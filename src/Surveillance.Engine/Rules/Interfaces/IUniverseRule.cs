namespace Surveillance.Engine.Rules.Rules.Interfaces
{
    using System;

    using Domain.Surveillance.Rules.Interfaces;
    using Domain.Surveillance.Scheduling;

    using Surveillance.Data.Universe.Interfaces;

    /// <summary>
    /// The UniverseRule interface.
    /// </summary>
    public interface IUniverseRule : IObserver<IUniverseEvent>
    {
        /// <summary>
        /// Gets the rule.
        /// </summary>
        Rules Rule { get; }

        /// <summary>
        /// Gets the version.
        /// </summary>
        string Version { get; }

        /// <summary>
        /// The data constraints.
        /// </summary>
        /// <returns>
        /// The <see cref="IRuleDataConstraint"/>.
        /// </returns>
        IRuleDataConstraint DataConstraints();
    }
}