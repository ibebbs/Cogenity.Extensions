using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.Hosting.PlugIns.PlugIn
{
    public class Loader
    {
        private static readonly string HostDirectory = Path.GetDirectoryName(typeof(HostBuilderExtensions).Assembly.Location); // hostContext.HostingEnvironment.ContentRootPath

        private Configuration.Instance _configuration;

        public Loader(Configuration.Instance configuration)
        {
            _configuration = configuration;
        }

        public void Load(HostBuilderContext hostContext, IServiceCollection serviceCollection)
        {
            foreach (Configuration.PlugIn configuredPlugIn in _configuration.PlugIns)
            {
                var fileName = Path.GetFileName(configuredPlugIn.Assembly);
                var path = Path.GetDirectoryName(configuredPlugIn.Assembly);
                
                path = string.IsNullOrWhiteSpace(path) ? HostDirectory : path;

                var assemblyPath = Path.Combine(path, fileName);

                try
                {
                    var loadContext = new PlugIn.PluginLoadContext($"{assemblyPath}.dll", configuredPlugIn.Name);

                    var assembly = loadContext.LoadFromAssemblyName(new AssemblyName(fileName));

                    var plugIns = assembly
                        .GetTypes()
                        .Where(type => typeof(IPlugIn).IsAssignableFrom(type))
                        .Select(Activator.CreateInstance)
                        .Cast<IPlugIn>()
                        .ToArray();

                    foreach (var plugIn in plugIns)
                    {
                        plugIn.ConfigureHost(hostContext, serviceCollection, configuredPlugIn.ConfigurationSection);
                    }

                }
                catch (FileNotFoundException fileNotFoundException)
                {
                    if (!configuredPlugIn.Optional)
                    {
                        throw new NotFoundException(configuredPlugIn.Name, assemblyPath, fileNotFoundException);
                    }
                }
            }
        }
    }
}
