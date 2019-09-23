namespace Surveillance.Engine.Rules.Judgements.Interfaces
{
    using Domain.Surveillance.Judgement.Equity.Interfaces;

    public interface IHighVolumeJudgementService
    {
        void Judgement(IHighVolumeJudgement highVolume);
    }
}