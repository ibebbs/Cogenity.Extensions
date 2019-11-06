namespace Microsoft.Extensions.Hosting.Composition.Configuration
{
    public class Module
    {
        /// <summary>
        /// The unique name of this module instance
        /// </summary>
        /// <remarks>
        /// Multiple plugins using the same assembly can
        /// be loaded by using a different name here
        /// </remarks>
        public string Name { get; set; }

        /// <summary>
        /// The assembly containing one of more modules to load
        /// </summary>
        /// <remarks>
        /// Must not include a file extension (i.e. dll)
        /// </remarks>
        public string Assembly { get; set; }

        /// <summary>
        /// The configuration section to bind to
        /// </summary>
        public string ConfigurationSection { get; set; }

        /// <summary>
        /// True if errors loading this plugin should be ignored
        /// </summary>
        public bool Optional { get; set; }
    }
}
