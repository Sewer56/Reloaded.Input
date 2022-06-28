using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Reloaded.Input.Structs;

namespace Reloaded.Input;

/// <summary/>
public class MappingSet
{
    /// <summary>
    /// Contains a list of mappings of unique ID to mapping value.
    /// </summary>
    public Dictionary<int, Mapping> Mappings { get; set; } = new();

    // Serialization Methods

    /// <summary>
    /// Saves the mapping file to a given file path.
    /// </summary>
    public void SaveTo(string filePath)
    {
        File.WriteAllText(filePath, JsonSerializer.Serialize(this, _serializerSettings));
    }

    /// <summary>
    /// Reads the mapping file from a given path, or creates the mapping file.
    /// </summary>
    public static MappingSet ReadOrCreateFrom(string filePath)
    {
        var result = File.Exists(filePath)
            ? JsonSerializer.Deserialize<MappingSet>(File.ReadAllText(filePath), _serializerSettings)
            : new MappingSet();

        return result;
    }

    private static JsonSerializerOptions _serializerSettings = new()
    {
        WriteIndented = true
    };
}