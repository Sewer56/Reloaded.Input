using Reloaded.Input.Structs;

namespace Reloaded.Input.Interfaces;

/// <summary>
/// Abstracts an individual controller from an API like XInput or DInput.
/// </summary>
public interface IController
{
    /// <summary>
    /// Obtains a string that uniquely identifies the controller.
    /// </summary>
    string GetId();

    /// <summary>
    /// Obtains the friendly name for a controller.
    /// </summary>
    string GetFriendlyName();

    /// <summary>
    /// Updates the values returned by <see cref="GetButtons"/> and <see cref="GetAxis"/>.
    /// </summary>
    void Poll();

    /// <summary>
    /// Retrieves all of the controller buttons.
    /// </summary>
    ButtonSet GetButtons();

    /// <summary>
    /// Retrieves all of the controller axis.
    /// </summary>
    AxisSet GetAxis ();
}