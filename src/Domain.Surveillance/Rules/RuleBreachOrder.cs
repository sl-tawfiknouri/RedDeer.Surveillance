namespace Domain.Surveillance.Rules
{
    public class RuleBreachOrder
    {
        public RuleBreachOrder(string ruleBreachId, string orderId)
        {
            this.RuleBreachId = ruleBreachId ?? string.Empty;
            this.OrderId = orderId ?? string.Empty;
        }

        public string OrderId { get; }

        public string RuleBreachId { get; }
    }
}