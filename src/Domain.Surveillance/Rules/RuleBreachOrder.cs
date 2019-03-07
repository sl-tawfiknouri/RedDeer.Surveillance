namespace Domain.Surveillance.Rules
{
    public class RuleBreachOrder
    {
        public RuleBreachOrder(string ruleBreachId, string orderId)
        {
            RuleBreachId = ruleBreachId ?? string.Empty;
            OrderId = orderId ?? string.Empty;
        }

        public string RuleBreachId { get; }
        public string OrderId { get; }
    }
}
