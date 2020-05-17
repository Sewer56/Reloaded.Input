using System.Collections.ObjectModel;
using Reloaded.WPF.MVVM;

namespace Reloaded.Input.Configurator.Model
{
    public class Configuration : ObservableObject
    {
        public ObservableCollection<Mapping> Mappings   { get; private set; } = new ObservableCollection<Mapping>();
        public VirtualController Controller             { get; private set; } 
        public string ConfigurationName                 { get; private set; }

        public Configuration(ConfiguratorInput input)
        {
            ConfigurationName = input.ConfigurationName;
            Controller = new VirtualController(input.ConfigurationPath);
            foreach (var entry in input.Entries)
            {
                Mappings.Add(new Mapping(Controller, entry.Name, entry.MappingIndex, entry.Type));
            }
        }
    }
}
