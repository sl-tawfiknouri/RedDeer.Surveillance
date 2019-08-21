namespace Surveillance.Api.App
{
    using System;

    using Microsoft.Extensions.DependencyInjection;

    public class StartupConfig : IStartupConfig
    {
        public Action<IServiceCollection> ConfigureTestServices { get; set; } = null;

        public bool IsTest { get; set; } = false;
    }
}