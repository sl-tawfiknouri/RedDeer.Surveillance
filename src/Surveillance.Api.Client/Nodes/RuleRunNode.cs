namespace RedDeer.Surveillance.Api.Client.Nodes
{
    using RedDeer.Surveillance.Api.Client.Infrastructure;

    public class RuleRunNode : Node<RuleRunNode>
    {
        public RuleRunNode(Parent parent)
            : base(parent)
        {
        }

        public RuleRunNode FieldCorrelationId()
        {
            return this.AddField("correlationId");
        }

        public RuleRunNode FieldId()
        {
            return this.AddField("id");
        }

        public ProcessOperationNode FieldProcessOperation()
        {
            return this.AddChild("processOperation", new ProcessOperationNode(this));
        }

        public RuleRunNode FieldRuleDescription()
        {
            return this.AddField("ruleDescription");
        }

        public RuleRunNode FieldRuleVersion()
        {
            return this.AddField("ruleVersion");
        }

        public RuleRunNode FieldScheduleRuleEnd()
        {
            return this.AddField("scheduleRuleEnd");
        }

        public RuleRunNode FieldScheduleRuleStart()
        {
            return this.AddField("scheduleRuleStart");
        }
    }
}