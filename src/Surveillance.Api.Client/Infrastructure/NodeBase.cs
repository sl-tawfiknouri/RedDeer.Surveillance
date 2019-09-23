namespace RedDeer.Surveillance.Api.Client.Infrastructure
{
    using System.Collections.Generic;
    using System.Linq;

    using Newtonsoft.Json;

    public abstract class NodeBase : Parent
    {
        protected readonly Dictionary<string, Parent> _fields = new Dictionary<string, Parent>();

        internal override string Build(string name, Dictionary<string, object> argumentsDictionary)
        {
            var arguments = argumentsDictionary
                                ?.Select(argument => $"{argument.Key}: {JsonConvert.SerializeObject(argument.Value)}")
                                ?.ToList() ?? new List<string>();

            var fields = new List<string>();
            foreach (var field in this._fields)
                if (field.Value != null)
                    fields.Add(field.Value.Build(field.Key, null));
                else
                    fields.Add(field.Key);

            var s = name;
            if (arguments.Count > 0) s += "(" + string.Join(", ", arguments) + ")";
            if (fields.Count > 0) s += " { " + string.Join(", ", fields) + " }";
            return s;
        }
    }
}