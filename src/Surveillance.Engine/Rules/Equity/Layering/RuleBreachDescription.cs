namespace Surveillance.Engine.Rules.Rules.Equity.Layering
{
    public class RuleBreachDescription
    {
        public bool RuleBreached { get; set; }
        public string Description { get; set; }

        public static RuleBreachDescription False()
        {
            return new RuleBreachDescription { RuleBreached = false, Description = string.Empty };
        }
    }
}
