namespace RedDeer.Surveillance.Api.Client.Infrastructure
{
    using System;
    using System.Collections.Generic;

    public class Filter<TSelf, TNode> : Parent
        where TSelf : class where TNode : Parent
    {
        private readonly Dictionary<string, object> _arguments = new Dictionary<string, object>();

        public Filter(TNode node)
        {
            this.Node = node;
        }

        public TNode Node { get; }

        internal override string Build(string name, Dictionary<string, object> arguments)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            return this.Node.Build(name, this._arguments);
        }

        protected TSelf AddArgument<TValues>(string name, HashSet<TValues> value)
        {
            if (value == null) return this.SetArgument(name, value);

            if (!this._arguments.ContainsKey(name)) return this.SetArgument(name, value);

            if (!(this._arguments[name] is IEnumerable<TValues> existingValue)) return this.SetArgument(name, value);

            var newValue = new HashSet<TValues>(value);
            newValue.UnionWith(existingValue);

            return this.SetArgument(name, value);
        }

        protected TSelf AddArgument(string name, object value)
        {
            return this.SetArgument(name, value);
        }

        private TSelf SetArgument(string name, object value)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            this._arguments[name] = value;
            return this as TSelf;
        }
    }
}