namespace Microsoft.Extensions.Hosting.PlugIns.Configuration
{
    public class PlugIn
    {
        /// <summary>
        /// The unique name of this plug in instance
        /// </summary>
        /// <remarks>
        /// Multiple plugins using the same assembly can
        /// be loaded by using a different name here
        /// </remarks>
        public string Name { get; set; }

        /// <summary>
        /// The assembly containing one of more plugins to load
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
        /// True if errors loading this plugin should
        /// be ignored
        /// </summary>
        public bool Optional { get; set; }
    }
}
