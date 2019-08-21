namespace Surveillance.Api.DataAccess.Entities
{
    using Surveillance.Api.DataAccess.Abstractions.Entities;

    public class RuleBreachOrder : IRuleBreachOrder
    {
        public int OrderId { get; set; }

        public int RuleBreachId { get; set; }
    }
}