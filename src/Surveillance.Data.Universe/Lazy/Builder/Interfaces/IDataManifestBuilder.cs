namespace Surveillance.Data.Universe.Lazy.Builder.Interfaces
{
    using System.Collections.Generic;

    using Domain.Surveillance.Rules.Interfaces;
    using Domain.Surveillance.Scheduling;

    /// <summary>
    /// The DataManifestBuilder interface.
    /// </summary>
    public interface IDataManifestBuilder
    {
        /// <summary>
        /// The build.
        /// </summary>
        /// <param name="execution">
        /// The execution.
        /// </param>
        /// <param name="ruleDataConstraints">
        /// The rule data constraints.
        /// </param>
        /// <returns>
        /// The <see cref="IDataManifest"/>.
        /// </returns>
        IDataManifest Build(ScheduledExecution execution, IReadOnlyCollection<IRuleDataConstraint> ruleDataConstraints);
    }
}
