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

        private readonly string _workingDirectory;

        public Loader(IOptions<Configuration.Instance> configuration)
        {
            _configuration = configuration.Value;

            _workingDirectory = string.IsNullOrWhiteSpace(configuration.Value.WorkingDirectory)
                ? HostDirectory
                : configuration.Value.WorkingDirectory;
        }

        private string GetModulePath(Configuration.Module module)
        {
            var fileName = Path.GetFileName(module.Assembly);
            var directory = Path.GetDirectoryName(module.Assembly);
            var path = string.IsNullOrWhiteSpace(directory) ? _workingDirectory : directory;

            return Path.Combine(path, fileName);
        }

        private IEnumerable<IModule> LoadModules(Configuration.Module module, string modulePath)
        {
            Trace.Event.LoadModuleStart(module.Name, modulePath);

            try
            {
                var loadContext = new LoadContext($"{modulePath}.dll", module.Name);

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
                    Trace.Event.UnableToLocateAssemblyForModule(module.Name, module.Assembly);
                    return Enumerable.Empty<IModule>();
                }
                else
                {
                    throw new NotFoundException(module.Name, modulePath, exception);
                }
            }
            finally
            {
                Trace.Event.LoadModuleStop(module.Name, modulePath);
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
