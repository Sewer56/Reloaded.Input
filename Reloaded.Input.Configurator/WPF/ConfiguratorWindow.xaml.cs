using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
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

        foreach (var config in ViewModel.Configurations)
        foreach (var mapping in config.Mappings)
            mapping.Slots.CollectionChanged += OnSlotsChanged;

        this.Closing += SaveOnExit;
        this.MappingGrid.LoadingRow += LoadedRow;
    }

    private void SaveOnExit(object sender, CancelEventArgs e)
    {
        ViewModel.Save();
        ViewModel.Close();
    }

    private void OnRightclick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        var mappingSlot = GetMappingFromButton(sender);
        if (e.RightButton == MouseButtonState.Pressed)
            mappingSlot.Parent.UnMap(mappingSlot.MappingNo);
    }

    private async void Click(object sender, System.Windows.RoutedEventArgs e)
    {
        var mappingSlot = GetMappingFromButton(sender);
        await mappingSlot.Parent.Map(mappingSlot.MappingNo);
    }

    private static MappingSlot GetMappingFromButton(object sender)
    {
        var senderBtn = (Button)sender;
        var mapping = (MappingSlot)(senderBtn.DataContext);
        return mapping;
    }

    private void LoadedRow(object? sender, DataGridRowEventArgs e)
    {
        var description = ViewModel.SelectedConfiguration.Mappings[e.Row.GetIndex()].Description;
        if (!string.IsNullOrEmpty(description))
        {
            e.Row.ToolTip = description;
            ToolTipService.SetInitialShowDelay(e.Row, 0);
        }

        OnSlotsChanged(null, null);
    }

    private async void OnSlotsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        // TODO: This is a terrible hack.
        await Task.Delay(16);

        // First reset all auto columns
        foreach (var column in MappingGrid.Columns)
        {
            if (!column.Width.IsAuto)
                continue;

            column.Width = 0;
            column.Width = DataGridLength.Auto;
        }

        try { MappingGrid.UpdateLayout(); }
        catch (Exception) { }

        // Then update all star columns
        foreach (var column in MappingGrid.Columns)
        {
            if (!column.Width.IsStar)
                continue;

            var oldWidth = column.Width;
            column.Width = 0;
            column.Width = oldWidth;
        }

        try { MappingGrid.UpdateLayout(); }
        catch (Exception) { }
    }
}