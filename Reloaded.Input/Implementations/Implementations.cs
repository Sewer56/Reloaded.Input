namespace Reloaded.Input.Implementations;

/// <summary>
/// Implementations of the controller.
/// </summary>
[Flags]
public enum Implementations
{
    /// <summary>
    /// Allow for use of DInput controllers.
    /// </summary>
    DInput = 1 << 0,
    
    /// <summary>
    /// Allow for use of XInput controllers.
    /// </summary>
    XInput = 1 << 1
}