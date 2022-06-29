using Reloaded.Input.Interfaces;

namespace Reloaded.Input.Structs;

/// <summary/>
public class Mapping
{
    /// <summary>
    /// The ID of the controller this mapping is assigned to.
    /// </summary>
    public string ControllerId { get; set; }

    /// <summary>
    /// The index of <see cref="MappingType"/> the button was mapped to.
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// Default constructor.
    /// </summary>
    public Mapping()
    {
        ControllerId = "";
        Index = 0;
    }

    /// <summary>
    /// Creates a mapping given a unique controller ID, mapping type and index.
    /// </summary>
    /// <param name="controllerId">Unique ID of the controller.</param>
    /// <param name="index">Index into the <see cref="ButtonSet"/> or <see cref="AxisSet"/> arrays.</param>
    public Mapping(string controllerId, int index)
    {
        ControllerId = controllerId;
        Index = index;
    }

    /// <summary>
    /// Gets a friendly name for the mapping.
    /// </summary>
    public string GetFriendlyName(Dictionary<string, IController> controllerIdToController, MappingType type)
    {
        if (!controllerIdToController.TryGetValue(ControllerId, out var controller))
            return "";

        if (type == MappingType.Button)
            return $"{controller.GetFriendlyName()}/B{Index}";

        return $"{controller.GetFriendlyName()}/A{Index}";
    }

    /// <summary>
    /// Gets the mapped value from the passed in controller instance.
    /// </summary>
    public float GetAxis(Dictionary<string, IController> controllerIdToController)
    {
        if (!controllerIdToController.TryGetValue(ControllerId, out var controller))
            return 0.0f;

        return controller.GetAxis().GetAxis(Index);
    }

    /// <summary>
    /// Gets the mapped value from the passed in controller instance.
    /// </summary>
    public bool GetButton(Dictionary<string, IController> controllerIdToController)
    {
        if (!controllerIdToController.TryGetValue(ControllerId, out var controller))
            return false;

        return controller.GetButtons().GetButton(Index);
    }
}

/// <summary>
/// Declares the types of mappings supported.
/// </summary>
public enum MappingType
{
    /// <summary/>
    Button,
    /// <summary/>
    Axis
}