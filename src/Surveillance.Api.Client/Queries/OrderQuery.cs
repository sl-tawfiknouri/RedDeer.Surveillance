﻿using Surveillance.Api.Client.Dtos;
using Surveillance.Api.Client.Infrastructure;
using Surveillance.Api.Client.Nodes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Surveillance.Api.Client.Queries
{
    using Response = List<OrderDto>;

    public class OrderQuery : Query<Response>
    {
        public OrderNode OrderNode { get; }

        public OrderQuery()
        {
            OrderNode = new OrderNode(this);
        }

        internal override async Task<Response> HandleAsync(IRequest request, CancellationToken ctx)
        {
            return await BuildAndPost<Response>("orders", OrderNode, request, ctx);
        }
    }
}
