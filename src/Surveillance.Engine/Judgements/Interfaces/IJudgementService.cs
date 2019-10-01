namespace Surveillance.Engine.Rules.Judgements.Interfaces
{
    /// <summary>
    /// The JudgementService interface.
    /// </summary>
    public interface IJudgementService : IHighProfitJudgementService,
                                         IFixedIncomeHighProfitJudgementService,
                                         ICancelledOrderJudgementService,
                                         IHighVolumeJudgementService,
                                         ILayeringJudgementService,
                                         IMarkingTheCloseJudgementService,
                                         IPlacingOrdersJudgementService,
                                         IRampingJudgementService,
                                         ISpoofingJudgementService,
                                         IFixedIncomeHighVolumeJudgementService
    {
        /// <summary>
        /// The pass judgement method which will process the underlying judgement cache.
        /// </summary>
        void PassJudgement();
    }
}