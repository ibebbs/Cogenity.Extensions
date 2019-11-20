using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;

namespace Cogenity.Extensions.Logging.EventSource
{
    public class EventLoggerFilterConfigureOptions : IConfigureOptions<EventLoggerFilterOptions>
    {
        private const string LogLevelKey = "LogLevel";
        private const string DefaultCategory = "Default";
        private readonly IConfigurationSection _configuration;

        public EventLoggerFilterConfigureOptions(IConfigurationSection configurationSection)
        {
            _configuration = configurationSection;
        }

        public void Configure(EventLoggerFilterOptions options)
        {
            LoadDefaultConfigValues(options);
        }

        private void LoadDefaultConfigValues(EventLoggerFilterOptions options)
        {
            if (_configuration == null)
            {
                return;
            }

            options.CaptureScopes = _configuration.GetValue(nameof(options.CaptureScopes), options.CaptureScopes);

            foreach (var configurationSection in _configuration.GetChildren())
            {
                if (configurationSection.Key.Equals(LogLevelKey, StringComparison.OrdinalIgnoreCase))
                {
                    // Load global category defaults
                    LoadRules(options, configurationSection, null);
                }
                else
                {
                    var logLevelSection = configurationSection.GetSection(LogLevelKey);
                    if (logLevelSection != null)
                    {
                        // Load logger specific rules
                        var logger = configurationSection.Key;
                        LoadRules(options, logLevelSection, logger);
                    }
                }
            }
        }

        private void LoadRules(EventLoggerFilterOptions options, IConfigurationSection configurationSection, string logger)
        {
            foreach (var section in configurationSection.AsEnumerable(true))
            {
                if (TryGetSwitch(section.Value, out var level))
                {
                    var category = section.Key;
                    if (category.Equals(DefaultCategory, StringComparison.OrdinalIgnoreCase))
                    {
                        category = null;
                    }
                    var newRule = new LoggerFilterRule(logger, category, level, null);
                    options.Rules.Add(newRule);
                }
            }
        }

        private static bool TryGetSwitch(string value, out LogLevel level)
        {
            if (string.IsNullOrEmpty(value))
            {
                level = LogLevel.None;
                return false;
            }
            else if (Enum.TryParse(value, true, out level))
            {
                return true;
            }
            else
            {
                throw new InvalidOperationException($"Configuration value '{value}' is not supported.");
            }
        }

    }
}
