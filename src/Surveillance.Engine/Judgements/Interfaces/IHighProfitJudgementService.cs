namespace Surveillance.Engine.Rules.Judgements.Interfaces
{
    using Surveillance.Engine.Rules.Judgements.Equities.Interfaces;
    using Surveillance.Engine.Rules.Judgements.FixedIncome.Interfaces;

    /// <summary>
    /// The HighProfitJudgementService interface.
    /// </summary>
    public interface IHighProfitJudgementService
    {
        /// <summary>
        /// The judgement.
        /// </summary>
        /// <param name="judgementContext">
        /// The judgement context.
        /// </param>
        void Judgement(IHighProfitJudgementContext judgementContext);

        /// <summary>
        /// The judgement.
        /// </summary>
        /// <param name="judgementContext">
        /// The judgement context.
        /// </param>
        void Judgement(IFixedIncomeHighProfitJudgementContext judgementContext);
    }
}