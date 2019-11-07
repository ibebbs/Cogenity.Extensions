# Microsoft.Extensions.Hosting.Composition

A library for .NET Core 3.0 providing runtime composition of services and configuration from disparate assemblies into the [.NET Generic Host](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-3.0).

## Background

The .NET Generic Host provides scaffolding for implementing software that runs acrosss a variety of environments and platforms. It provides common resources - and patterns to supplement these resources - such as:

  * Dependency injection (DI)
  * Logging
  * Configuration

Furthermore, by encapsulating/decorating your software as an `IHostedService`, the Generic Host provides graceful startup and shutdown in accordance with the lifetime underwhich it is currently running (i.e. console application, windows service, web host, etc).

In short, it's great! Since it's introduction in .NET Core 2.1, the Generic Host has quickly become the go-to pattern for implementing long-running services in .NET Core.

Unfortunately, the Generic Host doesn't provide any means of composing services from non-referenced assemblies at runtime. While [libraries for accomplishing runtime composition are available](https://www.nuget.org/packages/System.Composition.Runtime/), they tend to be quite heavy weight and there is little to no guidance on how to integrate these libraries in a way that works reliably with the Generic Host.

This library aims to address this issue by providing a simple mechanisms to quickly and reliably load and register services from non-referenced assemblies into the Generic Host.

NOTE: This library does not intend to be a generic, zero-knowledge plugin system. Microsoft.Extensions.Hosting.Composition uses configuration to specify and configure modules providing increased reliability and flexibility while decreasing start-up times compared to the directory / assembly scanning approaches typically used by plug-in systems. If you feel you require a plug-in system, you can find a good example of one [here](https://github.com/dapplo/Dapplo.Microsoft.Extensions.Hosting).

## Usage

Usage is very straight-forward and can be accomplished in a few steps. Here are the steps I used to implement the [GenericHostConsole](https://github.com/ibebbs/Microsoft.Extensions.Hosting.PlugIns/tree/master/samples) sample:

### Step 1 - UseComposition

In the GenericHostConsole project, add a reference to `Microsoft.Extensions.Hosting.Composition` and add the line `.UseComposition()` as shown below:

```c#
private static async Task Main(string[] args)
{
    var builder = Host.CreateDefaultBuilder(args)
        .ConfigureHostConfiguration(configurationBuilder => configurationBuilder.AddCommandLine(args))
        .UseComposition(config => config.AddYamlFile(args[0])); // <- Add this line

    await builder
        .Build()
        .RunAsync();
}
```

As you can see, the `.UseComposition()` function requires configuration information but we'll get back to this in step 3.

### Step 2 - Implement IModule

For any assembly you'd like to compose into your Generic Framework host, add a reference to `Microsoft.Extensions.Hosting.Composition.Abstractions` and add a new class that implements `IModule`. Within the `Configure` method of the interface, compose your services as you would from a normal generic host. Here we're registering configuration, services and logging within the [GenericHostConsole.Writer](https://github.com/ibebbs/Microsoft.Extensions.Hosting.PlugIns/tree/master/samples/GenericHostConsole.Writer) sample:

```c#
public class Module : IModule
{
    public IHostBuilder Configure(IHostBuilder hostbuilder, string configurationSection)
    {
        return hostbuilder
            .ConfigureServices(
                (hostBuilderContext, serviceCollection) =>
                {
                    serviceCollection.AddOptions<Configuration>().Bind(hostBuilderContext.Configuration.GetSection(configurationSection));
                    serviceCollection.AddSingleton<IHostedService, Service>();
                })
            .ConfigureLogging((hostingContext, logging) => logging.AddConsole());
    }
}
```

### Step 3 - Configuration

Back in the GenericHostConsole project, we need to supply configuration information to the `.UseComposition()` call. I like using yaml for this kind of configuration so I first install the [NetEscapades.Configuration.Yaml](https://www.nuget.org/packages/NetEscapades.Configuration.Yaml/) package then add a new yaml file to the project named 'config.yml' (remembering to set it's `Copy To Output Directory` setting to `Copy If Newer`).

Then I populate the config.yaml file with the following:

```yaml
composition:
  modules:
    - name: ConsoleWriter
      assembly: GenericHostConsole.Writer
      configurationSection: consolewriterConfiguration
      optional: true

consolewriterConfiguration:
  writeIntervalInSeconds: 2
```

When loaded, this configuration will do the following:

1. Instruct the module loader to load the `GenericHostConsole.Writer` assembly. If no path is supplied, it looks in the directory containing the currently executing loading assembly.
2. Give the loaded module a distinct name which allows multiple modules of the same type to be loaded concurrently.
3. Pass the specified `configurationSection` to the module from which to load it's configuration
4. State that loading this module is optional - no exception will be thrown if the module could not be located.

### Step 4 - Run

If you run the GenericHostConsole app now.... you'll see the following:

```
Warning: The module named 'ConsoleWriter' could not be loaded as the assembly 'GenericHostConsole.Writer' could not be found
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
info: Microsoft.Hosting.Lifetime[0]
      Hosting environment: Production
info: Microsoft.Hosting.Lifetime[0]
      Content root path: C:\Source\Repositories\Microsoft.Extensions.Hosting.PlugIns\samples\GenericHostConsole\bin\Debug\netcoreapp3.0
```

Yup, it starts with a warning that it couldn't locate a named module then does nothing. Now, if you copy the build artifacts from `GenericHostConsole.Writer` (`.\samples\GenericHostConsole.Writer\bin\debug\netstandard2.0`) to the build directory for `GenericHostConsole` (`.\samples\GenericHostConsole\bin\Debug\netcoreapp3.0`) then re-run the GenericHostConsole app you should now see the following:

```
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
info: Microsoft.Hosting.Lifetime[0]
      Hosting environment: Production
info: Microsoft.Hosting.Lifetime[0]
      Content root path: C:\Source\Repositories\Microsoft.Extensions.Hosting.PlugIns\samples\GenericHostConsole\bin\Debug\netcoreapp3.0
info: GenericHostConsole.Writer.Service[0]
      Here!
info: GenericHostConsole.Writer.Service[0]
      Here!
```

Yup, no warning and the the text `Here!` written to the console every two seconds. This is the GenericHostConsole.Writer.Service following it's configuration and logging settings.

Done, you've composed functionality into your generic host!

## Contributing

Any suggestions/contributions of enhancements/bug fixes gratefully received.