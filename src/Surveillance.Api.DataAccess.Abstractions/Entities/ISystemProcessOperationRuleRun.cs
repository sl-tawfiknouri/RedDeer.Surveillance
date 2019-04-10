namespace Surveillance.Api.DataAccess.Abstractions.Entities
{
    public interface ISystemProcessOperationRuleRun
    {
        string CorrelationId { get; set; }
        int Id { get; set; }
        bool IsBackTest { get; set; }
        bool IsForceRun { get; set; }
        string RuleDescription { get; set; }
        string RuleParameterId { get; set; }
        int RuleTypeId { get; set; } // there is a rule type but we need to extract abstractions from domain.surveillance for this
        string RuleVersion { get; set; }
        int SystemProcessOperationId { get; set; }
        string End { get; }
        string Start { get; }
    }
}