# Samples

The following sections describes the process of how the samples were implemented.

## GenericHostConsole

1. Start a new Console App (`.netcore3.0`) and ensure you're targeting .NET Core 3.0
2. Add the following packages:
    * [Microsoft.Extensions.Hosting](https://www.nuget.org/packages/Microsoft.Extensions.Hosting)
    * [Cogenity.Extensions.Hosting.Compsition](https://www.nuget.org/packages/Cogenity.Extensions.Hosting.Composition)
    * [NetEscapades.Configuration.Yaml](https://www.nuget.org/packages/NetEscapades.Configuration.Yaml)
3. Add the following to the `Main` method:

```c#
private static async Task Main(string[] args)
{
    var builder = Host.CreateDefaultBuilder(args)
        .ConfigureHostConfiguration(configurationBuilder => configurationBuilder.AddCommandLine(args))
        .UseComposition(config => config.AddYamlFile(args[0]));

    await builder
        .Build()
        .RunAsync();
}
```
4. Add a new yaml file named 'config.yml' and ensure it's `Copy To Output Directory` setting to `Copy If Newer`. Add the following content to the file:

```yml
composition:
  modules:
    - name: ConsoleWriter
      assembly: GenericHostConsole.Writer
      configurationSection: consolewriterConfiguration
      optional: true

consolewriterConfiguration:
  writeIntervalInSeconds: 2
```

## GenericHostConsole.Writer

1. Start a new Library project (`.netstandard 2.0') and add references to the following packages:
    * [Microsoft.Extensions.Hosting](https://www.nuget.org/packages/Microsoft.Extensions.Hosting)
    * [Cogenity.Extensions.Hosting.Compsition.Abstractions](https://www.nuget.org/packages/Cogenity.Extensions.Hosting.Composition.Abstractions)
    * [Microsoft.Extensions.Logging.Console](https://www.nuget.org/packages/Microsoft.Extensions.Logging.Console)
    * [Microsoft.Extensions.Options.ConfigurationExtensions](https://www.nuget.org/packages/Microsoft.Extensions.Options.ConfigurationExtensions)
2. Rename `Class1.cs` to `Module.cd` (ensuring to rename the class too) and implement the `IModule` interface in this class as follows:
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
3. Add the Configuration and Service classes which can be found [here](https://github.com/ibebbs/Cogenity.Extensions.Hosting.Composition/blob/master/samples/GenericHostConsole.Writer/Configuration.cs) and [here](https://github.com/ibebbs/Cogenity.Extensions.Hosting.Composition/blob/master/samples/GenericHostConsole.Writer/Service.cs) respectively.

## Build and composition

Build both projects then copy the build artifacts from `GenericHostConsole.Writer` (`.\samples\GenericHostConsole.Writer\bin\debug\netstandard2.0`) to the build artifacts directory in `GenericHostConsole` (`.\samples\GenericHostConsole\bin\Debug\netcoreapp3.0`).

## Run!

Run the GenericHostConsole application and you should see the `GenericHostConsole.Writer.Service` write `Here!` to the console every two seconds per it's configuration.


