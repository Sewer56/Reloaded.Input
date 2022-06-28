using System.Collections.ObjectModel;
using Reloaded.Input.Configurator.WPF;
using Reloaded.Input.Structs;

namespace Reloaded.Input.Configurator.Test;

class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        var mappingEntries = new MappingEntry[]
        {
            new("Jump", 0, MappingType.Button),
            new("Boost", 1, MappingType.Button),
            new("Drift", 2, MappingType.Button),

            new("Up/Down", 3, MappingType.Axis),
            new("Left/Right", 4, MappingType.Axis),
        };

        var configurator = new WPF.Configurator();
        configurator.Run(new ConfiguratorWindow(new ConfiguratorInput[]
        {
            new("Controller 1", "Controller1.json", new ObservableCollection<MappingEntry>(mappingEntries)),
            new("Controller 2", "Controller2.json", new ObservableCollection<MappingEntry>(mappingEntries)),
            new("Controller 3", "Controller3.json", new ObservableCollection<MappingEntry>(mappingEntries)),
            new("Controller 4", "Controller4.json", new ObservableCollection<MappingEntry>(mappingEntries)),
        }));
    }
}