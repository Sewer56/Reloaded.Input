using System;
using System.Collections.ObjectModel;
using System.Windows;
using Reloaded.Input.Configurator.WPF;
using Reloaded.Input.Structs;

namespace Reloaded.Input.Configurator.Test
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var mappingEntries = new MappingEntry[]
            {
                new MappingEntry("Jump", 0, MappingType.Button),
                new MappingEntry("Boost", 1, MappingType.Button),
                new MappingEntry("Drift", 2, MappingType.Button),

                new MappingEntry("Up/Down", 3, MappingType.Axis),
                new MappingEntry("Left/Right", 4, MappingType.Axis),
            };

            var configurator = new WPF.Configurator();
            configurator.Run(new ConfiguratorWindow(new ConfiguratorInput[]
            {
                new ConfiguratorInput("Controller 1", "Controller1.json", new ObservableCollection<MappingEntry>(mappingEntries)),
                new ConfiguratorInput("Controller 2", "Controller2.json", new ObservableCollection<MappingEntry>(mappingEntries)),
                new ConfiguratorInput("Controller 3", "Controller3.json", new ObservableCollection<MappingEntry>(mappingEntries)),
                new ConfiguratorInput("Controller 4", "Controller4.json", new ObservableCollection<MappingEntry>(mappingEntries)),
            }));
        }
    }
}
