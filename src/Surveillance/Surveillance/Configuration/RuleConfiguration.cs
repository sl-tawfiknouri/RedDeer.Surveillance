using Surveillance.Configuration.Interfaces;

namespace Surveillance.Configuration
{
    public class RuleConfiguration : IRuleConfiguration
    {
        /// <summary>
        /// How long to wait on de duplicating pure subset alerts for cancelled orders
        /// </summary>
        public int? CancelledOrderDeduplicationDelaySeconds { get; set; }
    }
}
