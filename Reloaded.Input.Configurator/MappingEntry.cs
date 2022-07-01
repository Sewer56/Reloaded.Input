using Reloaded.Input.Structs;
using Reloaded.WPF.MVVM;

namespace Reloaded.Input.Configurator;

/// <summary>
/// Represents an individual entry the user can supply to be mapped by the configurator.
/// </summary>
public class MappingEntry : ObservableObject
{
    public string Name                  { get; private set; }
    public string Description           { get; private set; }
    public int MappingIndex             { get; private set; }
    public MappingType Type             { get; private set; }

    public MappingEntry(string name, int mappingIndex, MappingType type, string description = "")
    {
        Name = name;
        MappingIndex = mappingIndex;
        Type = type;
        Description = description;
    }
}