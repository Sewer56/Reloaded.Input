namespace Reloaded.Input.Configurator.Localization;

/// <summary>
/// This class provides localization support for a given language. 
/// </summary>
public interface ILocalizationProvider
{
    /// <summary>
    /// Tries to get the localized name for the given string.
    /// </summary>
    /// <param name="button">The button to get name for.</param>
    /// <returns>The name, or null if not available.</returns>
    public string? GetText(CustomStrings button) => null;
}

/// <summary>
/// Custom strings specific to this package.
/// </summary>
public enum CustomStrings
{
    /// <summary>
    /// "New" Used for button that adds values.
    /// </summary>
    New,

    /// <summary>
    /// "The description of the tooltip that appears on buttons". Default text:
    ///
    /// Left click assigns a new binding.
    /// Middle click inverts the value.
    /// Right click clears the mapping!
    /// ! indicates the value is inverted.
    /// </summary>
    BindButtonTooltip,

    /// <summary>
    /// The string "timeout"
    /// </summary>
    Timeout
}