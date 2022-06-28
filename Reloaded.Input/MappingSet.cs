using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Reloaded.Input.Structs;

namespace Reloaded.Input;

public class MappingSet
{
    public Dictionary<int, Mapping> Mappings { get; private set; } = new();

    // Serialization Methods
    public void SaveTo(string filePath)
    {
        File.WriteAllText(filePath, JsonConvert.SerializeObject(this, _serializerSettings));
    }

    public static MappingSet ReadOrCreateFrom(string filePath)
    {
        var result = File.Exists(filePath)
            ? JsonConvert.DeserializeObject<MappingSet>(File.ReadAllText(filePath), _serializerSettings)
            : new MappingSet();

        return result;
    }

    private static JsonSerializerSettings _serializerSettings = new()
    {
        Formatting = Formatting.Indented
    };
}