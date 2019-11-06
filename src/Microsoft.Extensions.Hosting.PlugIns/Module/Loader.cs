using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.Hosting.Composition.Module
{
    public class Loader
    {
        private static readonly string HostDirectory = Path.GetDirectoryName(typeof(HostBuilderExtensions).Assembly.Location);

        private Configuration.Instance _configuration;

        public Loader(Configuration.Instance configuration)
        {
            _configuration = configuration;
        }

        public void Load(IHostBuilder hostbuilder)
        {
            foreach (Configuration.Module configuredModule in _configuration.Modules)
            {
                var fileName = Path.GetFileName(configuredModule.Assembly);
                var path = Path.GetDirectoryName(configuredModule.Assembly);
                
                path = string.IsNullOrWhiteSpace(path) ? HostDirectory : path;

                var assemblyPath = Path.Combine(path, fileName);

                try
                {
                    var loadContext = new LoadContext($"{assemblyPath}.dll", configuredModule.Name);

                    var assembly = loadContext.LoadFromAssemblyName(new AssemblyName(fileName));

                    var modules = assembly
                        .GetTypes()
                        .Where(type => typeof(IModule).IsAssignableFrom(type))
                        .Select(Activator.CreateInstance)
                        .Cast<IModule>()
                        .ToArray();

                    foreach (var module in modules)
                    {
                        module.Configure(hostbuilder, configuredModule.ConfigurationSection);
                    }

                }
                catch (Exception exception)
                {
                    if (!configuredModule.Optional)
                    {
                        throw new NotFoundException(configuredModule.Name, assemblyPath, exception);
                    }
                }
            }
        }
    }
}
