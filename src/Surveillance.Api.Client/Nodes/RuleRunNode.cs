using RedDeer.Surveillance.Api.Client.Infrastructure;

namespace RedDeer.Surveillance.Api.Client.Nodes
{
    public class RuleRunNode : Node<RuleRunNode>
    {
        public RuleRunNode(Parent parent) : base(parent) { }

        public RuleRunNode FieldId() => AddField("id");
        public RuleRunNode FieldCorrelationId() => AddField("correlationId");
        public RuleRunNode FieldRuleDescription() => AddField("ruleDescription");
        public RuleRunNode FieldRuleVersion() => AddField("ruleVersion");
        public RuleRunNode FieldScheduleRuleStart() => AddField("scheduleRuleStart");
        public RuleRunNode FieldScheduleRuleEnd() => AddField("scheduleRuleEnd");
        public ProcessOperationNode FieldProcessOperation() => AddChild("processOperation", new ProcessOperationNode(this));

    }
}
