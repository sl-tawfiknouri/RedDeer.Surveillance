using Surveillance.Api.Client.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Surveillance.Api.Client.Nodes
{
    public class OrderDatesNode : Node
    {
        public OrderDatesNode(NodeParent parent) : base(parent) { }

        public OrderDatesNode FieldAmended() => AddField("amendedDate", this);
        public OrderDatesNode FieldBooked() => AddField("bookedDate", this);
        public OrderDatesNode FieldCancelled() => AddField("cancelledDate", this);
        public OrderDatesNode FieldFilled() => AddField("filledDate", this);
        public OrderDatesNode FieldPlaced() => AddField("placedDate", this);
        public OrderDatesNode FieldRejected() => AddField("rejectedDate", this);
        public OrderDatesNode FieldStatusChanged() => AddField("statusChangedDate", this);
    }
}
