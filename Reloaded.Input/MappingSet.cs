using System;
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
    public Dictionary<int, MultiMapping> Mappings { get; set; } = new();

    // Serialization Methods

    /// <summary>
    /// Gets an existing mapping or creates a new one with a given type.
    /// </summary>
    /// <param name="index">The index of the mapping.</param>
    /// <param name="type">Type of the mapping [in case one needs to be created].</param>
    public MultiMapping GetOrCreateMapping(int index, MappingType type)
    {
        if (!Mappings.TryGetValue(index, out var value))
        {
            value = new MultiMapping(type);
            Mappings[index] = value;
        }

        return value;
    }

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
        try
        {
            var result = File.Exists(filePath)
                ? JsonSerializer.Deserialize<MappingSet>(File.ReadAllText(filePath), _serializerSettings)
                : new MappingSet();

            if (result == null)
                return new MappingSet();

            return result;
        }
        catch (Exception)
        {
            return new MappingSet();
        }
    }

    private static JsonSerializerOptions _serializerSettings = new()
    {
        WriteIndented = true
    };
}