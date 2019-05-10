using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Surveillance.Api.Client.Infrastructure
{
    public class Node : NodeParent
    {
        private readonly NodeParent _parent;
        private readonly Dictionary<string, object> _arguments = new Dictionary<string, object>();
        private readonly Dictionary<string, Node> _fields = new Dictionary<string, Node>();

        public Node(NodeParent parent)
        {
            _parent = parent;
        }

        public T Parent<T>() where T : NodeParent
        {
            return _parent as T;
        }

        protected T AddArgument<T>(string name, object value, T self) where T : Node
        {
            if (self != this)
            {
                throw new Exception("Self must be set to this");
            }
            _arguments[name] = value;
            return self;
        }

        protected T AddField<T>(string name, T self) where T : Node
        {
            if (self != this)
            {
                throw new Exception("Self must be set to this");
            }
            _fields[name] = null;
            return self;
        }

        protected T AddChild<T>(string name, T node) where T : Node
        {
            _fields[name] = node;
            return node;
        }

        internal string Build(string name, Dictionary<string, object> additionalArguments = null)
        {
            var arguments = ExpandArguments(_arguments);
            if (additionalArguments != null)
            {
                arguments.AddRange(ExpandArguments(additionalArguments));
            }

            var fields = new List<string>();
            foreach (var field in _fields)
            {
                if (field.Value != null)
                {
                    fields.Add(field.Value.Build(field.Key));
                }
                else
                {
                    fields.Add(field.Key);
                }
            }

            var s = name;
            if (arguments.Count > 0)
            {
                s += "(" + string.Join(", ", arguments) + ")";
            }
            if (fields.Count > 0)
            {
                s += " { " + string.Join(", ", fields) + " }";
            }
            return s;
        }

        private List<string> ExpandArguments(Dictionary<string, object> argumentsDictionary)
        {
            var arguments = new List<string>();
            foreach (var argument in argumentsDictionary)
            {
                var value = argument.Value;
                if (argument.Value is DateTime dateTime)
                {
                    value = dateTime.ToString(CultureInfo.GetCultureInfo("en-GB"));
                }
                arguments.Add($"{argument.Key}: {JsonConvert.SerializeObject(value)}");
            }
            return arguments;
        }
    }
}
