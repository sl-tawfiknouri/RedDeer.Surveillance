namespace Surveillance.Engine.Rules.Judgements.Interfaces
{
    using System.Threading.Tasks;

    using Domain.Surveillance.Judgement.Equity.Interfaces;

    /// <summary>
    /// The LayeringJudgementService interface.
    /// </summary>
    public interface ILayeringJudgementService
    {
        /// <summary>
        /// The judgement.
        /// </summary>
        /// <param name="layering">
        /// The layering.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task Judgement(ILayeringJudgement layering);
    }
}