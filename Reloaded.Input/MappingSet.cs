using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Reloaded.Input.Structs;

namespace Reloaded.Input;

public class MappingSet
{
    public Dictionary<int, Mapping> Mappings { get; set; } = new();

    // Serialization Methods
    public void SaveTo(string filePath)
    {
        File.WriteAllText(filePath, JsonSerializer.Serialize(this, _serializerSettings));
    }

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