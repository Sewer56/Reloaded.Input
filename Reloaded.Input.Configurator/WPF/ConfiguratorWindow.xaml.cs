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
        var senderBtn = (Button)sender;
        var dataContext = (Mapping)(senderBtn.DataContext);
        if (e.RightButton == MouseButtonState.Pressed)
            dataContext.UnMap();
    }

    private async void Click(object sender, System.Windows.RoutedEventArgs e)
    {
        var senderBtn = (Button)sender;
        var dataContext = (Mapping)(senderBtn.DataContext);
        await dataContext.Map();
    }
}