using RedDeer.Surveillance.Api.Client.Infrastructure;

namespace RedDeer.Surveillance.Api.Client.Nodes
{
    public class OrderAllocationNode : Node<OrderAllocationNode>
    {
        public OrderAllocationNode(Parent parent) : base(parent) { }

        public OrderAllocationNode FieldId() => AddField("id");

        public OrderAllocationNode FieldOrderId() => AddField("orderId");

        public OrderAllocationNode FieldFund() => AddField("fund");

        public OrderAllocationNode FieldStrategy() => AddField("strategy");

        public OrderAllocationNode FieldClientAccountId() => AddField("clientAccountId");

        public OrderAllocationNode FieldOrderFilledVolume() => AddField("orderFilledVolume");

        public OrderAllocationNode FieldLive() => AddField("live");

        public OrderAllocationNode FieldAutoScheduled() => AddField("autoScheduled");

        public OrderAllocationNode FieldCreatedDate() => AddField("createdDate");
    }
}
