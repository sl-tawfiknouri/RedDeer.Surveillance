namespace Surveillance.Engine.Rules.Judgements.Interfaces
{
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
        void Judgement(IFixedIncomeHighProfitJudgementContext judgementContext);
    }
}
