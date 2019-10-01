namespace Surveillance.Engine.Rules.Judgements.Interfaces
{
    using System.Threading.Tasks;

    using Domain.Surveillance.Judgement.Equity.Interfaces;

    /// <summary>
    /// The SpoofingJudgementService interface.
    /// </summary>
    public interface ISpoofingJudgementService
    {
        /// <summary>
        /// The judgement.
        /// </summary>
        /// <param name="spoofing">
        /// The spoofing.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task Judgement(ISpoofingJudgement spoofing);
    }
}