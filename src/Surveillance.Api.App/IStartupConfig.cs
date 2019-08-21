namespace Surveillance.Api.App
{
    using System;

    using Microsoft.Extensions.DependencyInjection;

    public interface IStartupConfig
    {
        Action<IServiceCollection> ConfigureTestServices { get; set; }

        bool IsTest { get; set; }
    }
}