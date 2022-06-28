using System.Collections.ObjectModel;

namespace Reloaded.Input.Configurator;

public class ConfiguratorInput
{
    public string ConfigurationName { get; set; }
    public string ConfigurationPath { get; set; }
    public ObservableCollection<MappingEntry> Entries { get; set; }

    public ConfiguratorInput(string configurationName, string configurationPath, ObservableCollection<MappingEntry> entries)
    {
        ConfigurationName = configurationName;
        ConfigurationPath = configurationPath;
        Entries = entries;
    }
}