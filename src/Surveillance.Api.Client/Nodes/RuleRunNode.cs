using Surveillance.Api.Client.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Surveillance.Api.Client.Nodes
{
    public class RuleRunNode : Node<RuleRunNode>
    {
        public RuleRunNode(NodeParent parent) : base(parent) { }

        public RuleRunNode FieldId() => AddField("id");
        public RuleRunNode FieldCorrelationId() => AddField("correlationId");
        public RuleRunNode FieldRuleDescription() => AddField("ruleDescription");
        public RuleRunNode FieldRuleVersion() => AddField("ruleVersion");
        public RuleRunNode FieldScheduleRuleStart() => AddField("start");
        public RuleRunNode FieldScheduleRuleEnd() => AddField("end");
        public ProcessOperationNode FieldProcessOperation() => AddChild("processOperation", new ProcessOperationNode(this));

    }
}
