namespace Reloaded.Input.Configurator.Localization;

internal static class LocalizedStrings
{
    public static string New = "New";
    public static string BindButtonTooltip = "Left click assigns a new binding.\n" +
                                             "Middle click inverts the value.\n" +
                                             "Right click clears the mapping!\n" +
                                             "! indicates the value is inverted.";

    public static string Timeout = "Timeout";

    public static void Init(ILocalizationProvider? provider)
    {
        if (provider == null)
            return;

        Assign(ref New, provider.GetText(CustomStrings.New));
        Assign(ref BindButtonTooltip, provider.GetText(CustomStrings.BindButtonTooltip));
        Assign(ref Timeout, provider.GetText(CustomStrings.Timeout));

        static void Assign(ref string target, string? value)
        {
            if (value != null)
                target = value;
        }
    }
}