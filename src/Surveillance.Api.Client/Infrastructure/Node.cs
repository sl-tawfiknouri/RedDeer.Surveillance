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

        protected T AddArgument<T>(string name, object value, T self)
        {
            var variable = Guid.NewGuid().ToString();
            _actions.Add(graph => graph.Argument(name, "", variable));
            _arguments.Add(new GraphQLQueryArgument(variable, value));
            return self;
        }

        protected T AddField<T>(string name, T self)
        {
            _actions.Add(x => x.Field(name));
            return self;
        }
    }
}
