using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using Reloaded.Input.Configurator.Model;
using Reloaded.Input.Configurator.ViewModel;
using Reloaded.WPF.Theme.Default;

namespace Reloaded.Input.Configurator.WPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class ConfiguratorWindow : ReloadedWindow
{
    public new MainWindowViewModel ViewModel { get; set; }

    public ConfiguratorWindow(ConfiguratorInput[] input)
    {
        InitializeComponent();
        ViewModel = new MainWindowViewModel(input);
        this.Closing += SaveOnExit;
    }

    private void SaveOnExit(object sender, CancelEventArgs e)
    {
        ViewModel.Save();
        ViewModel.Close();
    }

    private void OnRightclick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        var mapping = GetMappingFromButton(sender);
        if (e.RightButton == MouseButtonState.Pressed)
            mapping.UnMap();
    }

    private async void Click(object sender, System.Windows.RoutedEventArgs e)
    {
        var mapping = GetMappingFromButton(sender);
        await mapping.Map();
    }

    private static Mapping GetMappingFromButton(object sender)
    {
        var senderBtn = (Button)sender;
        var mapping = (Mapping)(senderBtn.DataContext);
        return mapping;
    }
}