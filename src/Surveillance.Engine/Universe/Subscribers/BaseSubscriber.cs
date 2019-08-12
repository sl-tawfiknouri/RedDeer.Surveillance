using SharedKernel.Contracts.Markets;
using Surveillance.Engine.Rules.RuleParameters;

namespace Surveillance.Engine.Rules.Universe.Subscribers
{
    public abstract class BaseSubscriber
    {
        /// <summary>
        /// Used to figure out if we can use factset or not
        /// </summary>
        protected DataSource DataSourceForWindow(TimeWindows windows)
        {
            if (windows == null)
            {
                return DataSource.AllInterday;
            }

            return ((windows.BackwardWindowSize.Hours > 0
                    || windows.BackwardWindowSize.Minutes > 0)
                   || windows.BackwardWindowSize.Days < 1)
                ? DataSource.AllIntraday
                : DataSource.AllInterday;
        }
    }
}
