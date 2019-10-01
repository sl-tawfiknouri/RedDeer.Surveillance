namespace Surveillance.Engine.Rules.Judgements.Interfaces
{
    using System.Threading.Tasks;

    using Surveillance.Engine.Rules.Judgements.FixedIncome.Interfaces;

    /// <summary>
    /// The FixedIncomeHighVolumeJudgementService interface.
    /// </summary>
    public interface IFixedIncomeHighVolumeJudgementService
    {
        /// <summary>
        /// The judgement.
        /// </summary>
        /// <param name="highVolumeJudgementContext">
        /// The high volume judgement context.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task Judgement(IFixedIncomeHighVolumeJudgementContext highVolumeJudgementContext);
    }
}