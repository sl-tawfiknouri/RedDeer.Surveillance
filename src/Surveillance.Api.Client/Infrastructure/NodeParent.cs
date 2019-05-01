using SAHB.GraphQLClient.QueryGenerator;
using System;
using System.Collections.Generic;
using System.Text;

namespace Surveillance.Api.Client.Infrastructure
{
    public abstract class NodeParent
    {
        internal List<GraphQLQueryArgument> _arguments;

        public NodeParent(NodeParent parent)
        {
            _arguments = parent == null ? new List<GraphQLQueryArgument>() : parent._arguments;
        }
    }
}
