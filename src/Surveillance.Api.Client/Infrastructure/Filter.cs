using System;
using System.Collections.Generic;
using System.Text;

namespace Surveillance.Api.Client.Infrastructure
{
    public class Filter<TSelf, TNode> : Parent
        where TSelf : class
        where TNode : Parent
    {
        public TNode Node { get; }

        private readonly Dictionary<string, object> _arguments = new Dictionary<string, object>();

        public Filter(TNode node)
        {
            Node = node;
        }

        protected TSelf AddArgument(string name, object value)
        {
            _arguments[name] = value;
            return this as TSelf;
        }

        internal override string Build(string name, Dictionary<string, object> arguments)
        {
            return Node.Build(name, _arguments);
        }
    }
}
