namespace Domain.Surveillance.Scheduling
{
    using System.Linq;

    public class RuleIdentifier
    {
        /// <summary>
        ///     Rule instance identifiers
        /// </summary>
        public string[] Ids { get; set; }

        /// <summary>
        ///     Rule
        /// </summary>
        public Rules Rule { get; set; }

        public override string ToString()
        {
            var ids = !(this.Ids?.Any() ?? false) ? string.Empty : this.Ids.Aggregate((x, y) => $"{x} {y}");
            return $"RuleIdentifier Rule: {this.Rule} Ids {ids}";
        }
    }
}