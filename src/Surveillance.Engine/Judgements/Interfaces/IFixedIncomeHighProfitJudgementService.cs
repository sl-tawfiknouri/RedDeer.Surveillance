namespace Surveillance.Engine.Rules.Judgements.Interfaces
{
    using System.Threading.Tasks;

    using Surveillance.Engine.Rules.Judgements.FixedIncome.Interfaces;

    /// <summary>
    /// The FixedIncomeHighProfitJudgementService interface (interface segregation principle).
    /// </summary>
    public interface IFixedIncomeHighProfitJudgementService
    {
        /// <summary>
        /// The judgement.
        /// </summary>
        /// <param name="judgementContext">
        /// The judgement context.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task Judgement(IFixedIncomeHighProfitJudgementContext judgementContext);
    }
}
