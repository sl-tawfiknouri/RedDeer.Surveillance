namespace Surveillance.Engine.Rules.Rules.FixedIncome.HighVolume.Interfaces
{
    using Surveillance.Engine.Rules.Judgements.FixedIncome.Interfaces;
    using Surveillance.Engine.Rules.Rules.Interfaces;

    /// <summary>
    /// The FixedIncomeHighVolumeJudgementMapper interface.
    /// </summary>
    public interface IFixedIncomeHighVolumeJudgementMapper
    {
        /// <summary>
        /// The map function to a rule breach.
        /// </summary>
        /// <param name="judgementContext">
        /// The judgement context.
        /// </param>
        /// <returns>
        /// The <see cref="IRuleBreach"/>.
        /// </returns>
        IRuleBreach Map(IFixedIncomeHighVolumeJudgementContext judgementContext);
    }
}