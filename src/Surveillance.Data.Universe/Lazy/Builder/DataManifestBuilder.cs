namespace Surveillance.Data.Universe.Lazy.Builder
{
    using System.Collections.Generic;

    using Domain.Surveillance.Rules.Interfaces;
    using Domain.Surveillance.Scheduling;

    using Surveillance.Data.Universe.Lazy.Builder.Interfaces;

    /// <summary>
    /// The data manifest builder.
    /// </summary>
    public class DataManifestBuilder : IDataManifestBuilder
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
        /// The <see cref="IDataManifestInterpreter"/>.
        /// </returns>
        public IDataManifestInterpreter Build(
            ScheduledExecution execution,
            IReadOnlyCollection<IRuleDataConstraint> ruleDataConstraints)
        {
            return null;
        }
    }
}
