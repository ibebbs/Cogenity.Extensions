using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.Hosting.Composition.Module
{
    public class Loader
    {
        private static readonly string HostDirectory = Path.GetDirectoryName(typeof(ComposableHostBuilderExtensions).Assembly.Location);

        private readonly Configuration.Instance _configuration;
        private readonly ILogger<Loader> _logger;

        public Loader(IOptions<Configuration.Instance> configuration, ILogger<Loader> logger)
        {
            _configuration = configuration.Value;
            _logger = logger;
        }

        private string GetModulePath(Configuration.Module module)
        {
            var fileName = Path.GetFileName(module.Assembly);
            var directory = Path.GetDirectoryName(module.Assembly);
            var path = string.IsNullOrWhiteSpace(directory) ? HostDirectory : directory;

            return Path.Combine(path, fileName);
        }

        private IEnumerable<IModule> LoadModules(Configuration.Module module, string modulePath)
        {
            try
            {
                using (_logger.BeginScope($"Attempting to load '{module.Name}' from '{modulePath}'"))
                {
                    var loadContext = new LoadContext($"{modulePath}.dll", module.Name, _logger);

                    var assembly = loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileName(module.Assembly)));

                    return assembly
                        .GetTypes()
                        .Where(type => typeof(IModule).IsAssignableFrom(type))
                        .Select(Activator.CreateInstance)
                        .Cast<IModule>()
                        .ToArray();
                }
            }
            catch (Exception exception)
            {
                if (module.Optional)
                {
                    _logger.LogWarning($"The module named '{module.Name}' could not be loaded as the assembly '{module.Assembly}' could not be found");

                    return Enumerable.Empty<IModule>();
                }
                else
                {
                    throw new NotFoundException(module.Name, modulePath, exception);
                }
            }
        }

        public IHostBuilder Load(IHostBuilder hostbuilder)
        {
            return (_configuration.Modules ?? Enumerable.Empty<Configuration.Module>())
                .Select(module => (Module: module, Path: GetModulePath(module)))
                .SelectMany(tuple => LoadModules(tuple.Module, tuple.Path).Select(module => (Module: module, tuple.Module.ConfigurationSection)))
                .Aggregate(hostbuilder, (hb, tuple) => tuple.Module.Configure(hb, tuple.ConfigurationSection));
        }
    }
}
