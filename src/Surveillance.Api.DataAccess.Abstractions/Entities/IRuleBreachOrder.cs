namespace Surveillance.Api.DataAccess.Abstractions.Entities
{
    public interface IRuleBreachOrder
    {
        int OrderId { get; set; }
        int RuleBreachId { get; set; }
    }
}