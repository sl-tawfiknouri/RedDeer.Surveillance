namespace RedDeer.Surveillance.Api.Client.Nodes
{
    using RedDeer.Surveillance.Api.Client.Infrastructure;

    public class OrderDatesNode : Node<OrderDatesNode>
    {
        public OrderDatesNode(Parent parent)
            : base(parent)
        {
        }

        public OrderDatesNode FieldAmended()
        {
            return this.AddField("amendedDate");
        }

        public OrderDatesNode FieldBooked()
        {
            return this.AddField("bookedDate");
        }

        public OrderDatesNode FieldCancelled()
        {
            return this.AddField("cancelledDate");
        }

        public OrderDatesNode FieldFilled()
        {
            return this.AddField("filledDate");
        }

        public OrderDatesNode FieldPlaced()
        {
            return this.AddField("placedDate");
        }

        public OrderDatesNode FieldRejected()
        {
            return this.AddField("rejectedDate");
        }

        public OrderDatesNode FieldStatusChanged()
        {
            return this.AddField("statusChangedDate");
        }
    }
}