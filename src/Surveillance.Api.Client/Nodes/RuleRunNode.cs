using Surveillance.Api.Client.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Surveillance.Api.Client.Nodes
{
    public class RuleRunNode : Node
    {
        public RuleRunNode(NodeParent parent) : base(parent) { }

        public RuleRunNode ArgumentCorrelationIds(List<string> correlationIds) => AddArgument("correlationIds", correlationIds, this);

        public RuleRunNode FieldId() => AddField("id", this);
        public RuleRunNode FieldCorrelationId() => AddField("correlationId", this);

    }
}
