using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.Hosting.Composition
{
    public class ComposableHostBuilder : IComposableHostBuilder
    {
        private readonly DefaultServiceProviderFactory _compositionServiceProviderFactory = new DefaultServiceProviderFactory();
        private readonly List<Action<IConfigurationBuilder>> _configureHostConfigActions = new List<Action<IConfigurationBuilder>>();
        private readonly List<Action<HostBuilderContext, IConfigurationBuilder>> _configureAppConfigActions = new List<Action<HostBuilderContext, IConfigurationBuilder>>();
        private readonly List<Action<HostBuilderContext, IServiceCollection>> _configureServicesActions = new List<Action<HostBuilderContext, IServiceCollection>>()
        {
            (hostBuilderContext, serviceCollection) => serviceCollection.AddLogging(),
            (hostBuilderContext, serviceCollection) => serviceCollection.AddTransient<Module.Loader>()
        };


        private IHostBuilder _hostBuilder;

        public ComposableHostBuilder(IHostBuilder hostBuilder)
        {
            _hostBuilder = hostBuilder;
        }

        private IServiceProvider BuildCompositionServiceProvider(HostBuilderContext hostBuilderContext)
        {
            var services = new ServiceCollection();

            foreach (var configureServicesAction in _configureServicesActions)
            {
                configureServicesAction(hostBuilderContext, services);
            }

            var serviceContainerBuilder = _compositionServiceProviderFactory.CreateBuilder(services);
            return _compositionServiceProviderFactory.CreateServiceProvider(serviceContainerBuilder);
        }

        private IConfigurationRoot BuildHostCompositionConfiguration()
        {
            var configBuilder = new ConfigurationBuilder()
                .AddInMemoryCollection(); // Make sure there's some default storage since there are no default providers

            foreach (var buildAction in _configureHostConfigActions)
            {
                buildAction(configBuilder);
            }

            return configBuilder.Build();
        }

        private IConfigurationRoot BuildCompositionConfiguration(HostBuilderContext hostBuilderContext)
        {
            var configBuilder = new ConfigurationBuilder()
                .AddConfiguration(BuildHostCompositionConfiguration(), shouldDisposeConfiguration: true);

            foreach (var buildAction in _configureAppConfigActions)
            {
                buildAction(hostBuilderContext, configBuilder);
            }

            return configBuilder.Build();
        }


        private IHostBuilder Compose()
        {
            var compositionBuilderContext = new HostBuilderContext(Properties);
            compositionBuilderContext.Configuration = BuildCompositionConfiguration(compositionBuilderContext);

            var compositionServiceProvider = BuildCompositionServiceProvider(compositionBuilderContext);

            var loader = compositionServiceProvider.GetService<Module.Loader>();

            return loader.Load(_hostBuilder);
        }

        public IHost Build()
        {
            Compose();

            return _hostBuilder.Build();
        }

        public IHostBuilder ConfigureAppConfiguration(Action<HostBuilderContext, IConfigurationBuilder> configureDelegate)
        {
            _configureAppConfigActions.Add(configureDelegate ?? throw new ArgumentNullException(nameof(configureDelegate)));

            _hostBuilder = _hostBuilder.ConfigureAppConfiguration(configureDelegate);

            return this;
        }

        public IHostBuilder ConfigureContainer<TContainerBuilder>(Action<HostBuilderContext, TContainerBuilder> configureDelegate)
        {
            _hostBuilder = _hostBuilder.ConfigureContainer(configureDelegate);

            return this;
        }

        public IHostBuilder ConfigureHostConfiguration(Action<IConfigurationBuilder> configureDelegate)
        {
            _configureHostConfigActions.Add(configureDelegate ?? throw new ArgumentNullException(nameof(configureDelegate)));
            _hostBuilder = _hostBuilder.ConfigureHostConfiguration(configureDelegate);

            return this;
        }

        public IHostBuilder ConfigureServices(Action<HostBuilderContext, IServiceCollection> configureDelegate)
        {
            _configureServicesActions.Add(configureDelegate);

            _hostBuilder = _hostBuilder.ConfigureServices(configureDelegate);

            return this;
        }

        public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory)
        {
            _hostBuilder = _hostBuilder.UseServiceProviderFactory(factory);

            return this;
        }

        public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(Func<HostBuilderContext, IServiceProviderFactory<TContainerBuilder>> factory)
        {
            _hostBuilder = _hostBuilder.UseServiceProviderFactory(factory);

            return this;
        }

        public IDictionary<object, object> Properties => _hostBuilder.Properties;
    }
}
