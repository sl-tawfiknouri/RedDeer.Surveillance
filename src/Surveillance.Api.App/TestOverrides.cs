using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Surveillance.Api.App
{
    public class TestOverrides
    {
        public IEnumerable<KeyValuePair<string, string>> Config { get; set; }
        public Action<IServiceCollection> ConfigureServices { get; set; }
    }
}
