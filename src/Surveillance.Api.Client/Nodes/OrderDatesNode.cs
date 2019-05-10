using Surveillance.Api.Client.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Surveillance.Api.Client.Nodes
{
    public class OrderDatesNode : Node<OrderDatesNode>
    {
        public OrderDatesNode(NodeParent parent) : base(parent) { }

        public OrderDatesNode FieldAmended() => AddField("amendedDate");
        public OrderDatesNode FieldBooked() => AddField("bookedDate");
        public OrderDatesNode FieldCancelled() => AddField("cancelledDate");
        public OrderDatesNode FieldFilled() => AddField("filledDate");
        public OrderDatesNode FieldPlaced() => AddField("placedDate");
        public OrderDatesNode FieldRejected() => AddField("rejectedDate");
        public OrderDatesNode FieldStatusChanged() => AddField("statusChangedDate");
    }
}
