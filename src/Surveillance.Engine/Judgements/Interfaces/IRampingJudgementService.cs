namespace Surveillance.Engine.Rules.Judgements.Interfaces
{
    using System.Threading.Tasks;

    using Domain.Surveillance.Judgement.Equity.Interfaces;

    /// <summary>
    /// The RampingJudgementService interface.
    /// </summary>
    public interface IRampingJudgementService
    {
        /// <summary>
        /// The judgement.
        /// </summary>
        /// <param name="ramping">
        /// The ramping.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task Judgement(IRampingJudgement ramping);
    }
}