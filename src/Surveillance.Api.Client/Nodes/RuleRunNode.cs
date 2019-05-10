using Surveillance.Api.Client.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Surveillance.Api.Client.Nodes
{
    public class RuleRunNode : Node
    {
        public RuleRunNode(NodeParent parent) : base(parent) { }

        public RuleRunNode FieldId() => AddField("id", this);
        public RuleRunNode FieldCorrelationId() => AddField("correlationId", this);
        public RuleRunNode FieldRuleDescription() => AddField("ruleDescription", this);
        public RuleRunNode FieldRuleVersion() => AddField("ruleVersion", this);
        public RuleRunNode FieldScheduleRuleStart() => AddField("start", this);
        public RuleRunNode FieldScheduleRuleEnd() => AddField("end", this);
        public ProcessOperationNode FieldProcessOperation() => AddChild("processOperation", new ProcessOperationNode(this));

    }
}
