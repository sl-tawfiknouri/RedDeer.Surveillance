namespace Surveillance.Data.Universe.Lazy.Builder.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Domain.Surveillance.Rules.Interfaces;
    using Domain.Surveillance.Scheduling;

    using Surveillance.Auditing.Context.Interfaces;

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
        /// <param name="systemProcessOperationContext">
        /// The system process operation context
        /// </param>
        /// <returns>
        /// The <see cref="IDataManifestInterpreter"/>.
        /// </returns>
        Task<IDataManifestInterpreter> Build(
            ScheduledExecution execution,
            IReadOnlyCollection<IRuleDataConstraint> ruleDataConstraints,
            ISystemProcessOperationContext systemProcessOperationContext);
    }
}
