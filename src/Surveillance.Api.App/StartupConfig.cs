using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Surveillance.Api.App
{
    public class StartupConfig : IStartupConfig
    {
        public bool IsTest { get; set; } = false;
        public Action<IServiceCollection> ConfigureTestServices { get; set; } = null;
    }
}
