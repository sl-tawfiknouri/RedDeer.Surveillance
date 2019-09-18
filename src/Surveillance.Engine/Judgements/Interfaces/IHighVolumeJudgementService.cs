namespace Surveillance.Engine.Rules.Judgements.Interfaces
{
    using System.Threading.Tasks;

    using Domain.Surveillance.Judgement.Equity.Interfaces;

    /// <summary>
    /// The HighVolumeJudgementService interface.
    /// </summary>
    public interface IHighVolumeJudgementService
    {
        /// <summary>
        /// The judgement.
        /// </summary>
        /// <param name="highVolume">
        /// The high volume.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task Judgement(IHighVolumeJudgement highVolume);
    }
}