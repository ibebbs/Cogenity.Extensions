using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace Microsoft.Extensions.Hosting.Composition.Module
{
    internal class LoadContext : AssemblyLoadContext
    {
        private readonly ILogger<Loader> _logger;
        private readonly AssemblyDependencyResolver _resolver;

        public LoadContext(string pluginPath, string name, ILogger<Loader> logger) : base(name)
        {
            _logger = logger;
            _resolver = new AssemblyDependencyResolver(pluginPath);
        }

        /// <summary>
        /// Returns the path where the specified assembly can be found
        /// </summary>
        /// <param name="assemblyName">AssemblyName</param>
        /// <returns>string with the path</returns>
        public string ResolveAssemblyPath(AssemblyName assemblyName)
        {
            return _resolver.ResolveAssemblyToPath(assemblyName);
        }

        /// <inheritdoc />
        protected override Assembly Load(AssemblyName assemblyName)
        {
            using (_logger.BeginScope($"Attempting to resolve assembly '{assemblyName.FullName}'"))
            {
                // Try to get the assembly from the AssemblyLoadContext.Default, when it is already loaded
                if (Default.TryGetAssembly(assemblyName, out var alreadyLoadedAssembly))
                {
                    _logger.LogDebug($"Using already loaded assembly '{alreadyLoadedAssembly.FullName}'");

                    return alreadyLoadedAssembly;
                }
                else
                {
                    var assemblyPath = ResolveAssemblyPath(assemblyName);
                    if (assemblyPath == null)
                    {
                        _logger.LogWarning($"Unable to resolve assembly path for '{assemblyName.FullName}'");
                        return null;
                    }
                    else
                    {
                        _logger.LogDebug($"Loading '{assemblyName.FullName}' from '{assemblyPath}'");

                        var resultAssembly = LoadFromAssemblyPath(assemblyPath);
                        return resultAssembly;
                    }
                }
            }
        }

        /// <inheritdoc />
        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            var libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
            if (libraryPath == null)
            {
                return IntPtr.Zero;
            }

            return LoadUnmanagedDllFromPath(libraryPath);
        }
    }

    public static class AssemblyLoadContextExtensions
    {
        /// <summary>
        /// Try to get an assembly from the specified AssemblyLoadContext
        /// </summary>
        /// <param name="assemblyLoadContext">AssemblyLoadContext</param>
        /// <param name="assemblyName">AssemblyName to look for</param>
        /// <param name="foundAssembly">Assembly out</param>
        /// <returns>bool</returns>
        public static bool TryGetAssembly(this AssemblyLoadContext assemblyLoadContext, AssemblyName assemblyName, out Assembly foundAssembly)
        {
            foundAssembly = assemblyLoadContext.Assemblies
                .Where(assembly => assembly.GetName().Name.Equals(assemblyName.Name))
                .FirstOrDefault();

            return foundAssembly != default;
        }
    }
}
