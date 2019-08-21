namespace Surveillance.Engine.Rules.Judgements.Interfaces
{
    public interface IJudgementService : IHighProfitJudgementService,
                                         ICancelledOrderJudgementService,
                                         IHighVolumeJudgementService,
                                         ILayeringJudgementService,
                                         IMarkingTheCloseJudgementService,
                                         IPlacingOrdersJudgementService,
                                         IRampingJudgementService,
                                         ISpoofingJudgementService
    {
        void PassJudgement();
    }
}