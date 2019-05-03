using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Surveillance.Api.App
{
    public interface IStartupConfig
    {
        bool IsTest { get; set; }
        Action<IServiceCollection> ConfigureTestServices { get; set; }
    }
}
