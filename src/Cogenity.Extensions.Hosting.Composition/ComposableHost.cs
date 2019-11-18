namespace Microsoft.Extensions.Hosting.Composition
{
    public static class ComposableHost
    {
        public static IHostBuilder CreateDefaultBuilder() => CreateDefaultBuilder(args: null);
        
        public static IHostBuilder CreateDefaultBuilder(string[] args)
        {
            var hostBuilder = Host
                .CreateDefaultBuilder(args);

            return new ComposableHostBuilder(hostBuilder)
                .UseComposition();
        }
    }
}
