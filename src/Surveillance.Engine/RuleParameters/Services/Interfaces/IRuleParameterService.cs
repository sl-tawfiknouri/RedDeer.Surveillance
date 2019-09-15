namespace Surveillance.Engine.Rules.RuleParameters.Services.Interfaces
{
    using System.Threading.Tasks;

    using Domain.Surveillance.Scheduling;

    using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;

    /// <summary>
    /// The RuleParameterService interface.
    /// </summary>
    public interface IRuleParameterService
    {
        /// <summary>
        /// The rule parameters transformation from a scheduled execution.
        /// </summary>
        /// <param name="execution">
        /// The execution.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<RuleParameterDto> RuleParameters(ScheduledExecution execution);
    }
}