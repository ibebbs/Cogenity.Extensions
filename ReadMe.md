# Microsoft.Extensions.Hosting.Composition

A library providing runtime composition of services and configuration from disparate assemblies into the [.NET Generic Host](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-3.0).

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

Usage is very straight-forward and can be accomplished in a few steps:

### Step 1 - UseComposition

In your generic host project, add a reference to `Microsoft.Extensions.Hosting.Composition` and add the line `.UseComposition()` as shown below:

```c#
private static async Task Main(string[] args)
{
    var builder = Host.CreateDefaultBuilder(args)
        .ConfigureHostConfiguration(configurationBuilder => configurationBuilder.AddCommandLine(args))
        .UseConsoleLifetime()
        .UseComposition(config => config.AddYamlFile(args[0])) // <- add this line
        .ConfigureLogging((hostingContext, logging) => logging.AddConsole());

    await builder
        .Build()
        .RunAsync();
}
```

As you can see, the `.UseComposition()` function requires configuration information but we'll get back to this in step 3.

### Step 2 - Implement IModule

For any assembly you'd like to compose into your Generic Framework host, add a reference to `Microsoft.Extensions.Hosting.Composition.Abstractions` and add a new class that implements `IModule` as shown below:

```c#
public class Module : IModule
{
    public void Configure(IHostBuilder hostbuilder, string configurationSection)
    {
        hostbuilder.ConfigureServices(
            (hostBuilderContext, serviceCollection) =>
            {
                serviceCollection.AddOptions<Configuration>().Bind(hostBuilderContext.Configuration.GetSection(configurationSection));
                serviceCollection.AddSingleton<IHostedService, Service>();
            }
        );
    }
}
```

As you can see, when this module is loaded, it will register a `IHostedService` with the Generic Host such that, when the host is started, the `IHostedService` is started too.

### Step 3 - Configuration


### Step 4 - Profit!


