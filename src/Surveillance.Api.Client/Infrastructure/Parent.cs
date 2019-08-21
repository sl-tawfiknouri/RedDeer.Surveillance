namespace RedDeer.Surveillance.Api.Client.Infrastructure
{
    using System.Collections.Generic;

    public abstract class Parent
    {
        internal abstract string Build(string name, Dictionary<string, object> arguments);
    }
}