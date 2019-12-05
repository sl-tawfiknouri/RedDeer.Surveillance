using Microsoft.Extensions.DependencyInjection;
using StructureMap;
using System;

namespace DataSynchroniser.App
{
    public class StructureMapServiceProviderFactory
        : IServiceProviderFactory<Container>, IDisposable
    {
        private readonly Container _container;

        public StructureMapServiceProviderFactory(Container container)
        {
            _container = container;
        }

        public Container CreateBuilder(IServiceCollection services)
        {
            _container.Configure(config => config.Populate(services));
            return _container;
        }

        public IServiceProvider CreateServiceProvider(Container container)
        {
            return container.GetInstance<IServiceProvider>();
        }

        /// <summary>
        /// Container must be dispose, if not dispose, Host service can't release the process.
        /// </summary>
        public void Dispose()
            => _container?.Dispose();
    }
}
