namespace Surveillance.Configuration.Interfaces
{
    public interface IRuleConfiguration
    {
        /// <summary>
        /// How long to wait on de duplicating pure subset alerts for cancelled orders
        /// </summary>
        int? CancelledOrderDeduplicationDelaySeconds { get; set; }
    }
}
