namespace RedDeer.Surveillance.Api.Client.Nodes
{
    using RedDeer.Surveillance.Api.Client.Infrastructure;

    public class OrderAllocationNode : Node<OrderAllocationNode>
    {
        public OrderAllocationNode(Parent parent)
            : base(parent)
        {
        }

        public OrderAllocationNode FieldAutoScheduled()
        {
            return this.AddField("autoScheduled");
        }

        public OrderAllocationNode FieldClientAccountId()
        {
            return this.AddField("clientAccountId");
        }

        public OrderAllocationNode FieldCreatedDate()
        {
            return this.AddField("createdDate");
        }

        public OrderAllocationNode FieldFund()
        {
            return this.AddField("fund");
        }

        public OrderAllocationNode FieldId()
        {
            return this.AddField("id");
        }

        public OrderAllocationNode FieldLive()
        {
            return this.AddField("live");
        }

        public OrderAllocationNode FieldOrderFilledVolume()
        {
            return this.AddField("orderFilledVolume");
        }

        public OrderAllocationNode FieldOrderId()
        {
            return this.AddField("orderId");
        }

        public OrderAllocationNode FieldStrategy()
        {
            return this.AddField("strategy");
        }
    }
}