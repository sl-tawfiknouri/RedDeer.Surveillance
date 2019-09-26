namespace Surveillance.Data.Universe.Lazy.Builder
{
    using System.Collections.Generic;

    using Domain.Surveillance.Scheduling;

    using Surveillance.Data.Universe.Lazy.Builder.Interfaces;

    public class DataManifestBuilder
    {
        public IDataManifest Build(
            ScheduledExecution execution,
            IReadOnlyCollection<object> ruleDataRequirements)
        {
            return null;
        }
    }
}
