using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Reloaded.Input.Configurator.Model;

namespace Reloaded.Input.Configurator.WPF.Converter;

internal class ConfigurationsToVisibilityConverter : IValueConverter
{
    public static ConfigurationsToVisibilityConverter Instance = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is ObservableCollection<Configuration> { Count: > 1 })
            return Visibility.Visible;

        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}