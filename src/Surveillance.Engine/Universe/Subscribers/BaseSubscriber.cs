namespace Surveillance.Engine.Rules.Universe.Subscribers
{
    using SharedKernel.Contracts.Markets;

    using Surveillance.Engine.Rules.RuleParameters;

    /// <summary>
    /// The base subscriber.
    /// </summary>
    public abstract class BaseSubscriber
    {
        /// <summary>
        /// Used to figure out if we can use fact set or not
        /// </summary>
        /// <param name="windows">
        /// The windows.
        /// </param>
        /// <returns>
        /// The <see cref="DataSource"/>.
        /// </returns>
        protected DataSource DataSourceForWindow(TimeWindows windows)
        {
            if (windows == null)
            {
                return DataSource.AllInterday;
            }

            return windows.BackwardWindowSize.Hours > 0 || windows.BackwardWindowSize.Minutes > 0
                                                        || windows.BackwardWindowSize.Days < 1
                       ? DataSource.AllIntraday
                       : DataSource.AllInterday;
        }
    }
}