namespace Domain.Scheduling
{
    public class RuleIdentifier
    {
        /// <summary>
        /// Rule
        /// </summary>
        public Domain.Scheduling.Rules Rule { get; set; }

        /// <summary>
        /// Rule instance identifiers
        /// </summary>
        public string[] Ids { get; set; }
    }
}
