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

        value.MappingType = type;
        return value;
    }

    /// <summary>
    /// Saves the mapping file to a given file path.
    /// </summary>
    /// <param name="filePath">Path of where to save the file.</param>
    /// <param name="cleanup">If true, cleans up the mapping set to prevent unused mappings from being written.</param>
    public void SaveTo(string filePath, bool cleanup = true)
    {
        if (cleanup)
            Cleanup();

        File.WriteAllText(filePath, JsonSerializer.Serialize(this, _serializerSettings));
    }

    /// <summary>
    /// Cleans up the mapping set to remove any empty mappings.
    /// </summary>
    public void Cleanup()
    {
        foreach (var mappingKey in Mappings.Keys.ToArray())
        {
            if (Mappings[mappingKey].Mappings.Count <= 0)
                Mappings.Remove(mappingKey);
        }
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