using Reloaded.Input.Interfaces;
using System.Text;

namespace Reloaded.Input.Structs;

/// <summary/>
public class MultiMapping
{
    /// <summary>
    /// List of mappings assigned to this multiple-mapping wrapper.
    /// </summary>
    public Dictionary<int, Mapping> Mappings { get; set; } = new();

    /// <summary>
    /// The mapping type, axis or button.
    /// </summary>
    public MappingType MappingType { get; private set; }

    private StringBuilder _builder = new StringBuilder();

    // Serialization
    public MultiMapping() { }

    /// <summary>
    /// Creates a Multimapping, which encapsulates multiple individual mappings.
    /// </summary>
    /// <param name="mappingType">Type of mapping performed.</param>
    /// <param name="mapping">Initial mapping to instance with.</param>
    public MultiMapping(MappingType mappingType, Mapping? mapping = default)
    {
        MappingType = mappingType;
        if (mapping != null)
            Mappings[0] = mapping;
    }

    /// <summary>
    /// Gets a friendly name for the mapping.
    /// </summary>
    /// <param name="controllerIdToController">A mapping between controller IDs and IController instances.</param>
    public string GetFriendlyName(Dictionary<string, IController> controllerIdToController)
    {
        var builder = new StringBuilder(100);
        var numMappings = Mappings.Count;
        for (var x = 0; x < numMappings; x++)
        {
            var mapping = Mappings[x];
            builder.Append(mapping.GetFriendlyName(controllerIdToController, MappingType));

            if (x < numMappings - 1)
                builder.Append(" | ");
        }

        return builder.ToString();
    }

    /// <summary>
    /// Gets a friendly name for the mapping.
    /// </summary>
    /// <param name="controllerIdToController">A mapping between controller IDs and IController instances.</param>
    /// <param name="mappingNo">Dictionary mapping of the index.</param>
    public string GetFriendlyName(Dictionary<string, IController> controllerIdToController, int mappingNo)
    {
        if (Mappings.TryGetValue(mappingNo, out var mapping))
            return mapping.GetFriendlyName(controllerIdToController, MappingType);

        return "";
    }

    /// <summary>
    /// Gets the mapped value from the passed in controller instance.
    /// </summary>
    public float GetAxis(Dictionary<string, IController> controllerIdToController)
    {
        if (MappingType != MappingType.Axis)
            return 0.0f;

        var value = 0.0f;
        foreach (var mapping in Mappings.Values)
            value += mapping.GetAxis(controllerIdToController);

        return value;
    }

    /// <summary>
    /// Gets the mapped value from the passed in controller instance.
    /// </summary>
    public bool GetButton(Dictionary<string, IController> controllerIdToController)
    {
        if (MappingType != MappingType.Button)
            return false;

        bool value = false;
        foreach (var mapping in Mappings.Values)
            value |= mapping.GetButton(controllerIdToController);

        return value;
    }

    /// <summary>
    /// Sets the mapping with a specified number.
    /// If the given number does not exist, the number will be appended to the end of the mapping list.
    /// </summary>
    /// <param name="mappingNo">Number of the mapping.</param>
    /// <param name="mapping">The individual mapping value.</param>
    /// <exception cref="System.NotImplementedException"></exception>
    public void SetMapping(int mappingNo, Mapping mapping) => Mappings[mappingNo] = mapping;

    /// <summary>
    /// Gets the next index in the dictionary that is not mapped.
    /// i.e. Max Index + 1;
    /// </summary>
    public int? GetNextUnmappedIndex()
    {
        int? highestIndex = default;

        foreach (var mapKey in Mappings.Keys)
        {
            if (highestIndex == null)
            {
                highestIndex = mapKey;
                continue;
            }

            if (mapKey > highestIndex.Value)
                highestIndex = mapKey;
        }

        return highestIndex + 1;
    }
}