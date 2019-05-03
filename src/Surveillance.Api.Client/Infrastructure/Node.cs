using SAHB.GraphQLClient.Builder;
using SAHB.GraphQLClient.QueryGenerator;
using System;
using System.Collections.Generic;
using System.Text;

namespace Surveillance.Api.Client.Infrastructure
{
    public class Node : NodeParent
    {
        public NodeParent _parent { get; private set; }
        internal List<Action<IGraphQLQueryFieldBuilder>> _actions = new List<Action<IGraphQLQueryFieldBuilder>>();

        public Node(NodeParent parent) : base(parent)
        {
            _parent = parent;
        }

        public T Parent<T>() where T : NodeParent
        {
            return _parent as T;
        }

        protected T AddArgument<T>(string name, object value, T child)
        {
            var variable = Guid.NewGuid().ToString();
            _actions.Add(graph => graph.Argument(name, "", variable));
            _arguments.Add(new GraphQLQueryArgument(variable, value));
            return child;
        }

        protected T AddField<T>(string name, T child)
        {
            _actions.Add(x => x.Field(name));
            return child;
        }
    }
}
