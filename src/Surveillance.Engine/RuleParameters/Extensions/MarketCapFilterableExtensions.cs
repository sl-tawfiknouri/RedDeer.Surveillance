namespace Surveillance.Engine.Rules.RuleParameters.Extensions
{
    using Surveillance.Engine.Rules.RuleParameters.Filter;
    using Surveillance.Engine.Rules.RuleParameters.Interfaces;

    /// <summary>
    /// The market cap filterable extensions.
    /// </summary>
    public static class MarketCapFilterableExtensions
    {
        /// <summary>
        /// The has market cap filters.
        /// </summary>
        /// <param name="marketCapFilterable">
        /// The market cap filterable.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool HasMarketCapFilters(this IMarketCapFilterable marketCapFilterable)
        {
            return marketCapFilterable.MarketCapFilter.Type == RuleFilterType.Include;
        }
    }
}