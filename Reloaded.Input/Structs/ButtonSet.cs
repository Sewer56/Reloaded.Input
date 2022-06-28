namespace Reloaded.Input.Structs;

/// <summary>
/// Abstracts a number of buttons returned from a controller.
/// </summary>
public unsafe struct ButtonSet
{
    /// <summary>
    /// Maximum number of buttons supported by this structure.
    /// </summary>
    public const int NumberOfButtons = 256;

    private fixed bool _buttons[NumberOfButtons];

    /// <summary>
    /// Gets the value of a button.
    /// </summary>
    /// <param name="index">Index of the button between 0 inclusive and <see cref="NumberOfButtons"/> exclusive</param>
    /// <returns>True if button pressed, else false.</returns>
    public bool GetButton(int index) => _buttons[index];

    /// <summary>
    /// Sets the value of a button.
    /// </summary>
    /// <param name="index">Index of the button between 0 inclusive and <see cref="NumberOfButtons"/> exclusive</param>
    /// <param name="value">The value.</param>
    /// <returns>True if button pressed, else false.</returns>
    public bool SetButton(int index, bool value) => _buttons[index] = value;
}