using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace RedDeer.Surveillance.Api.Client.Infrastructure
{
    public abstract class NodeBase : Parent
    {
        protected readonly Dictionary<string, Parent> _fields = new Dictionary<string, Parent>();

        internal override string Build(string name, Dictionary<string, object> argumentsDictionary)
        {
            var arguments = new List<string>();
            if (argumentsDictionary != null)
            {
                foreach (var argument in argumentsDictionary)
                {
                    var value = argument.Value;
                    if (argument.Value is DateTime dateTime)
                    {
                        value = dateTime.ToString(CultureInfo.GetCultureInfo("en-GB"));
                    }
                    arguments.Add($"{argument.Key}: {JsonConvert.SerializeObject(value)}");
                }
            }

            var fields = new List<string>();
            foreach (var field in _fields)
            {
                if (field.Value != null)
                {
                    fields.Add(field.Value.Build(field.Key, null));
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
    }
}
