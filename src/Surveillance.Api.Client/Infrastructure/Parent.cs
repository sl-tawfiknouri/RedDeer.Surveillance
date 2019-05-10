using System;
using System.Collections.Generic;
using System.Text;

namespace Surveillance.Api.Client.Infrastructure
{
    public abstract class Parent
    {
        internal abstract string Build(string name, Dictionary<string, object> arguments);
    }
}
