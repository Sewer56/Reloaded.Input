using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Reloaded.Input.Implementations.DInput;
using Reloaded.Input.Implementations.XInput;
using Reloaded.Input.Interfaces;
using Reloaded.Input.Structs;

namespace Reloaded.Input;

/// <summary>
/// Represents an individual abstracted controller.
/// </summary>
public class VirtualController : IDisposable
{
    const int MapSleepTime = 32;

    /// <summary>
    /// Called after the list of controllers is refreshed.
    /// </summary>
    public event Action OnRefresh;

    /// <summary>
    /// The mappings used in this controller.
    /// </summary>
    public MappingSet Mappings { get; private set; }

    /// <summary>
    /// Path of the file where the mappings are stored.
    /// </summary>
    public string FilePath { get; private set; }

    /// <summary>
    /// Set of controller managers.
    /// </summary>
    public IControllerManager[] Managers { get; private set; }

    /// <summary>
    /// Stores a list of controllers by ID.
    /// </summary>
    public Dictionary<string, IController> Controllers { get; private set; }

    /// <summary/>
    public VirtualController(string filePath)
    {
        FilePath = filePath;
        Mappings = MappingSet.ReadOrCreateFrom(filePath);
        Managers = new IControllerManager[]
        {
            new XInputManager(this),
            new DInputManager(this) 
        };
        Refresh();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        foreach (var manager in Managers)
        {
            if (manager is IDisposable disposable)
                disposable.Dispose();
        }
    }

    /// <summary>
    /// Refreshes the list of all controllers recognized by this virtual controller.
    /// </summary>
    internal void Refresh()
    {
        var controllerMap = new Dictionary<string, IController>();
        foreach (var manager in Managers)
        {
            foreach (var controller in manager.GetControllers())
            {
                controllerMap.Add(controller.GetId(), controller);
            }
        }

        Controllers = controllerMap;
        OnRefresh?.Invoke();
    }

    /// <summary>
    /// Saves the mappings behind this controller.
    /// </summary>
    public void Save() => Mappings.SaveTo(FilePath);

    /// <summary>
    /// Unmaps a mapping with a specified index.
    /// </summary>
    /// <param name="index">The index to unmap.</param>
    public void UnMap(int index)
    {
        Mappings.Mappings.Remove(index);
    }

    /// <summary>
    /// Polls controllers for a change in inputs and maps a specified button. 
    /// </summary>
    /// <param name="index">Unique mapping index for the mapping.</param>
    /// <param name="type">The type of mapping.</param>
    /// <param name="token">Allows for cancelling the mapping process.</param>
    /// <param name="callback">Executed after every poll attempt for a key or axis.</param>
    public async Task<bool> Map(int index, MappingType type, CancellationToken token = default, Action callback = null)
    {
        var controllerCaches = GetControllerCaches();

        try
        {
            if (type == MappingType.Button)
                return await Map_Button(index, type, token, callback, controllerCaches);

            if (type == MappingType.Axis)
                return await Map_Axis(index, type, token, callback, controllerCaches);
        }
        catch (TaskCanceledException)
        {
            return false;
        }

        return false;
    }

    private async Task<bool> Map_Axis(int index, MappingType type, CancellationToken token, Action callback, List<ControllerCache> controllerCaches)
    {
        var halfMaxAxis = AxisSet.MaxValue / 2;
        while (!token.IsCancellationRequested)
        {
            foreach (var cache in controllerCaches)
            {
                var newAxis = cache.Controller.GetAxis();
                for (int x = 0; x < AxisSet.NumberOfAxis; x++)
                {
                    var difference = MathF.Abs(cache.Axis.GetAxis(x) - newAxis.GetAxis(x));
                    if (difference < halfMaxAxis)
                        continue;

                    Mappings.Mappings[index] = new Mapping(cache.Controller.GetId(), type, x);
                    return true;
                }
            }

            callback?.Invoke();
            await Task.Delay(MapSleepTime, token);
        }

        return false;
    }

    private async Task<bool> Map_Button(int index, MappingType type, CancellationToken token, Action callback, List<ControllerCache> controllerCaches)
    {
        while (!token.IsCancellationRequested)
        {
            foreach (var cache in controllerCaches)
            {
                var newButtons = cache.Controller.GetButtons();
                for (int x = 0; x < ButtonSet.NumberOfButtons; x++)
                {
                    if (newButtons.GetButton(x) == cache.Buttons.GetButton(x))
                        continue;

                    Mappings.Mappings[index] = new Mapping(cache.Controller.GetId(), type, x);
                    return true;
                }
            }

            callback?.Invoke();
            await Task.Delay(MapSleepTime, token);
        }

        return false;
    }

    /// <summary>
    /// Gets a friendly name for a given mapping.
    /// </summary>
    /// <param name="index">The index to test.</param>
    public string GetFriendlyMappingName(int index)
    {
        if (!Mappings.Mappings.TryGetValue(index, out Mapping value)) 
            return "Unmapped";

        if (Controllers.TryGetValue(value.ControllerId, out var controller))
            return value.GetFriendlyName(controller);

        return "Unmapped";
    }

    /// <summary>
    /// Gets a mapping for a given index.
    /// </summary>
    /// <param name="index">The index to test.</param>
    public Mapping GetMapping(int index)
    {
        if (Mappings.Mappings.TryGetValue(index, out Mapping value))
            return value;

        return null;
    }

    /// <summary>
    /// Gets a bool value for a given mapping index.
    /// </summary>
    /// <param name="mappingIndex"></param>
    /// <returns>Value for the index, else default (false)</returns>
    public bool GetButton(int mappingIndex)
    {
        if (!Mappings.Mappings.TryGetValue(mappingIndex, out Mapping value)) 
            return false;

        if (!Controllers.TryGetValue(value.ControllerId, out var controller)) 
            return false;

        var buttons = controller.GetButtons();
        value.GetValue(ref buttons, out bool result);
        return result;
    }

    /// <summary>
    /// Gets a float value for a given mapping index.
    /// </summary>
    /// <param name="mappingIndex">The index the key is mapped to.</param>
    /// <returns>Value for the index, else default (0.0)</returns>
    public float GetAxis(int mappingIndex)
    {
        if (!Mappings.Mappings.TryGetValue(mappingIndex, out Mapping value)) 
            return 0.0f;

        if (!Controllers.TryGetValue(value.ControllerId, out var controller)) 
            return 0.0f;

        var axis = controller.GetAxis();
        value.GetValue(ref axis, out float result);
        return result;
    }

    private List<ControllerCache> GetControllerCaches()
    {
        var caches = new List<ControllerCache>(Controllers.Values.Count);
        foreach (var values in Controllers.Values) 
            caches.Add(new ControllerCache(values));

        return caches;
    }
}