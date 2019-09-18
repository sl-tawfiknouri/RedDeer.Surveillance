namespace Surveillance.Engine.Rules.Judgements.Interfaces
{
    using System.Threading.Tasks;

    using Domain.Surveillance.Judgement.Equity.Interfaces;

    /// <summary>
    /// The MarkingTheCloseJudgementService interface.
    /// </summary>
    public interface IMarkingTheCloseJudgementService
    {
        /// <summary>
        /// The judgement.
        /// </summary>
        /// <param name="markingTheClose">
        /// The marking the close.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task Judgement(IMarkingTheCloseJudgement markingTheClose);
    }
}