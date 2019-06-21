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
                _arguments[name] = value;
                return this as TSelf;
            }

            if (!_arguments.ContainsKey(name))
            {
                _arguments[name] = value;
                return this as TSelf;
            }

            if (!(_arguments[name] is IEnumerable<TValues> existingValue))
            {
                _arguments[name] = value;
                return this as TSelf;
            }

            var newValue = new HashSet<TValues>(value);
            newValue.UnionWith(existingValue);

            _arguments[name] = value;
            return this as TSelf;
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
