using System.Linq;

namespace Domain.Surveillance.Scheduling
{
    public class RuleIdentifier
    {
        /// <summary>
        /// Rule
        /// </summary>
        public Rules Rule { get; set; }

        /// <summary>
        /// Rule instance identifiers
        /// </summary>
        public string[] Ids { get; set; }

        public override string ToString()
        {
            var ids = !(Ids?.Any() ?? false) ? "" : Ids.Aggregate((x, y) => $"{x} {y}");
            return $"RuleIdentifier Rule: {Rule} Ids {ids}";
        }
    }
}
