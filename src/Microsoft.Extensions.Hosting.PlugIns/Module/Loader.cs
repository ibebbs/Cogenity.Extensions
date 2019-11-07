using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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

        private string GetModulePath(Configuration.Module module)
        {
            var fileName = Path.GetFileName(module.Assembly);
            var path = Path.GetDirectoryName(module.Assembly);

            path = string.IsNullOrWhiteSpace(path) ? HostDirectory : path;

            return Path.Combine(path, fileName);
        }

        private IEnumerable<IModule> LoadModules(Configuration.Module module, string assemblyPath)
        {
            try
            {
                var loadContext = new LoadContext($"{assemblyPath}.dll", module.Name);

                var assembly = loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileName(module.Assembly)));

                return assembly
                    .GetTypes()
                    .Where(type => typeof(IModule).IsAssignableFrom(type))
                    .Select(Activator.CreateInstance)
                    .Cast<IModule>()
                    .ToArray();
            }
            catch (Exception exception)
            {
                if (module.Optional)
                {
                    Console.WriteLine($"Warning: The module named '{module.Name}' could not be loaded as the assembly '{module.Assembly}' could not be found");

                    return Enumerable.Empty<IModule>();
                }
                else
                {
                    throw new NotFoundException(module.Name, assemblyPath, exception);
                }
            }
        }

        public IHostBuilder Load(IHostBuilder hostbuilder)
        {
            return _configuration.Modules
                .Select(module => (Module: module, Path: GetModulePath(module)))
                .SelectMany(tuple => LoadModules(tuple.Module, tuple.Path).Select(module => (Module: module, tuple.Module.ConfigurationSection)))
                .Aggregate(hostbuilder, (hb, tuple) => tuple.Module.Configure(hb, tuple.ConfigurationSection));
        }
    }
}
