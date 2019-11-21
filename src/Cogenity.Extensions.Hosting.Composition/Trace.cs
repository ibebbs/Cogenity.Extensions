using System;
using System.Diagnostics.Tracing;

namespace Microsoft.Extensions.Hosting.Composition
{
    [EventSource(Name = "Microsoft-Extensions-Hosting-Composition")]
    public sealed class Trace : EventSource
    {
        public static readonly Trace Event = new Trace();

        public class Keywords
        {
            public const EventKeywords Loader = (EventKeywords)1;
            public const EventKeywords LoadContext = (EventKeywords)2;
        }

        #region Loader

        [Event(1, Level = EventLevel.Verbose, ActivityOptions = EventActivityOptions.Recursive | EventActivityOptions.Detachable, Opcode = EventOpcode.Start, Keywords = Keywords.Loader, Message = "Starting to load module '{0}' from path '{1}'")]
        public void LoadModuleStart(string module, string path)
        {
            if (IsEnabled(EventLevel.Verbose, Keywords.Loader))
            {
                WriteEvent(1, module, path);
            }
        }

        [Event(2, Level = EventLevel.Verbose, ActivityOptions = EventActivityOptions.Recursive | EventActivityOptions.Detachable, Opcode = EventOpcode.Start, Keywords = Keywords.Loader, Message = "Completed loading module '{0}' from path '{1}'")]
        public void LoadModuleStop(string module, string path)
        {
            if (IsEnabled(EventLevel.Verbose, Keywords.Loader))
            {
                WriteEvent(2, module, path);
            }
        }

        [Event(3, Level = EventLevel.Warning, Keywords = Keywords.Loader, Message = "The module named '{0}' could not be loaded as the assembly '{1}' could not be found")]
        public void UnableToLocateAssemblyForModule(string module, string assembly)
        {
            if (IsEnabled(EventLevel.Warning, Keywords.Loader))
            {
                WriteEvent(3, module, assembly);
            }
        }

        #endregion Loader

        #region LoadContext

        [Event(11, Level = EventLevel.Verbose, ActivityOptions = EventActivityOptions.Recursive | EventActivityOptions.Detachable, Opcode = EventOpcode.Start, Keywords = Keywords.LoadContext, Message = "Starting to load assembly '{0}'")]
        public void AssemblyLoadStart(string assembly)
        {
            if (IsEnabled(EventLevel.Verbose, Keywords.LoadContext))
            {
                WriteEvent(11, assembly);
            }
        }

        [Event(12, Level = EventLevel.Verbose, ActivityOptions = EventActivityOptions.Recursive | EventActivityOptions.Detachable, Opcode = EventOpcode.Stop, Keywords = Keywords.LoadContext, Message = "Completed loading of assembly '{0}'")]
        public void AssemblyLoadStop(string assembly)
        {
            if (IsEnabled(EventLevel.Verbose, Keywords.LoadContext))
            {
                WriteEvent(12, assembly);
            }
        }

        [Event(13, Level = EventLevel.Verbose, Keywords = Keywords.LoadContext, Message = "Using default context for assembly '{0}'")]
        public void UsingDefaultAssembly(string assembly)
        {
            if (IsEnabled(EventLevel.Verbose, Keywords.LoadContext))
            {
                WriteEvent(13, assembly);
            }
        }

        [Event(14, Level = EventLevel.Warning, Keywords = Keywords.LoadContext, Message = "Unable to resolve assembly path for '{0}'")]
        public void UnableToResolveAssembly(string assembly)
        {
            if (IsEnabled(EventLevel.Warning, Keywords.LoadContext))
            {
                WriteEvent(14, assembly);
            }
        }

        [Event(15, Level = EventLevel.Verbose, Keywords = Keywords.LoadContext, Message = "Loading assembly '{0}' from path '{1}'")]
        public void LoadingAssemblyFromPath(string assembly, string path)
        {
            if (IsEnabled(EventLevel.Verbose, Keywords.LoadContext))
            {
                WriteEvent(15, assembly, path);
            }
        }

        [Event(16, Level = EventLevel.Warning, Keywords = Keywords.LoadContext, Message = "Unable to resolve unmanaged DLL path for '{0}'")]
        public void UnableToResolveUnmanagedDll(string unmanagedDllName)
        {
            if (IsEnabled(EventLevel.Warning, Keywords.LoadContext))
            {
                WriteEvent(16, unmanagedDllName);
            }
        }

        [Event(17, Level = EventLevel.Verbose, Keywords = Keywords.LoadContext, Message = "Loading unmanaged DLL '{0}' from path '{1}'")]
        public void LoadingUnmanagedDllFromPath(string unmanagedDllName, string path)
        {
            if (IsEnabled(EventLevel.Verbose, Keywords.LoadContext))
            {
                WriteEvent(17, unmanagedDllName, path);
            }
        }

        #endregion LoadContext
    }
}
