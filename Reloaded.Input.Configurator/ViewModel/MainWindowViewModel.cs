using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Reloaded.Input.Configurator.Model;
using Reloaded.WPF.MVVM;

namespace Reloaded.Input.Configurator.ViewModel;

public class MainWindowViewModel : ObservableObject
{
    public ObservableCollection<Configuration> Configurations { get; set; } = new ObservableCollection<Configuration>();
    public Configuration SelectedConfiguration { get; set; } = null;
    private Task UpdateValuesTask { get; set; }
    private CancellationTokenSource CancelTask { get; set; }

    public MainWindowViewModel(ConfiguratorInput[] input)
    {
        foreach (var inputs in input) 
            Configurations.Add(new Configuration(inputs));

        SelectedConfiguration = Configurations[0];

        CancelTask = new CancellationTokenSource();
        UpdateValuesTask = new Task(UpdateValues, CancelTask.Token);
        UpdateValuesTask.Start();
    }

    private async void UpdateValues()
    {
        while (! CancelTask.IsCancellationRequested)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (SelectedConfiguration != null)
                {
                    foreach (var mapping in SelectedConfiguration.Mappings)
                    {
                        mapping.UpdateValue();
                    }
                }
            });

            await Task.Delay(32);
        }
    }

    public void Save()
    {
        foreach (var configuration in Configurations) 
            configuration.Controller.Save();
    }

    public void Close()
    {
        CancelTask.Cancel();
        CancelTask.Dispose();
    }
}