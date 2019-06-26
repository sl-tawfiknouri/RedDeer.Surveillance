using System.Collections.Generic;

namespace RedDeer.Surveillance.Api.Client.Infrastructure
{
    public abstract class Parent
    {
        internal abstract string Build(string name, Dictionary<string, object> arguments);
    }
}
