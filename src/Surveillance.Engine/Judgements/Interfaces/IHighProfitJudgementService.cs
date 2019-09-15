namespace Surveillance.Engine.Rules.Judgements.Interfaces
{
    using Surveillance.Engine.Rules.Judgements.Equities.Interfaces;

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
    }
}