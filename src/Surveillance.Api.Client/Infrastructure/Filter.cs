using System;
using System.Collections.Generic;

namespace RedDeer.Surveillance.Api.Client.Infrastructure
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

        protected TSelf AddArgument<TValues>(string name, HashSet<TValues> value) 
        {
            if (value == null)
            {
                return SetArgument(name, value);
            }

            if (!_arguments.ContainsKey(name))
            {
                return SetArgument(name, value);
            }

            if (!(_arguments[name] is IEnumerable<TValues> existingValue))
            {
                return SetArgument(name, value);
            }

            var newValue = new HashSet<TValues>(value);
            newValue.UnionWith(existingValue);

            return SetArgument(name, value);
        }

        protected TSelf AddArgument(string name, object value)
            => SetArgument(name, value);

        private TSelf SetArgument(string name, object value)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            _arguments[name] = value;
            return this as TSelf;
        }

        internal override string Build(string name, Dictionary<string, object> arguments)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            return Node.Build(name, _arguments);
        }
    }
}
