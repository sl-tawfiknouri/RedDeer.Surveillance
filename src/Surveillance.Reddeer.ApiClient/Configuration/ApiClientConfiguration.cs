namespace Surveillance.Reddeer.ApiClient.Configuration
{
    using Surveillance.Reddeer.ApiClient.Configuration.Interfaces;

    /// <summary>
    /// The client configuration.
    /// </summary>
    public class ApiClientConfiguration : IApiClientConfiguration
    {
        /// <summary>
        /// Gets or sets the aurora connection string.
        /// </summary>
        public string AuroraConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the service url.
        /// </summary>
        public string BmllServiceUrl { get; set; }

        /// <summary>
        /// Gets or sets the case message queue name.
        /// </summary>
        public string CaseMessageQueueName { get; set; }

        /// <summary>
        /// Gets or sets the client service url.
        /// </summary>
        public string ClientServiceUrl { get; set; }

        /// <summary>
        /// Gets or sets the data synchronizer request queue name.
        /// </summary>
        public string DataSynchroniserRequestQueueName { get; set; }

        /// <summary>
        /// Gets or sets the email service send email queue name.
        /// </summary>
        public string EmailServiceSendEmailQueueName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is elastic compute 2 instance.
        /// </summary>
        public bool IsEc2Instance { get; set; }

        /// <summary>
        /// Gets or sets the schedule delayed rule run queue name.
        /// </summary>
        public string ScheduleDelayedRuleRunQueueName { get; set; }

        /// <summary>
        /// Gets or sets the scheduled rule queue name.
        /// </summary>
        public string ScheduledRuleQueueName { get; set; }

        /// <summary>
        /// Gets or sets the schedule rule cancellation queue name.
        /// </summary>
        public string ScheduleRuleCancellationQueueName { get; set; }

        /// <summary>
        /// Gets or sets the schedule rule distributed work queue name.
        /// </summary>
        public string ScheduleRuleDistributedWorkQueueName { get; set; }

        /// <summary>
        /// Gets or sets the surveillance user access token.
        /// </summary>
        public string SurveillanceUserApiAccessToken { get; set; }

        /// <summary>
        /// Gets or sets the test rule run update queue name.
        /// </summary>
        public string TestRuleRunUpdateQueueName { get; set; }

        /// <summary>
        /// Gets or sets the upload coordinator queue name.
        /// </summary>
        public string UploadCoordinatorQueueName { get; set; }
    }
}