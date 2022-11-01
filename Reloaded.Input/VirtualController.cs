using Reloaded.Input.Implementations.DInput;
using Reloaded.Input.Implementations.XInput;
using Reloaded.Input.Interfaces;
using Reloaded.Input.Structs;
using static Reloaded.Input.Implementations.Implementations;

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
    public event Action? OnRefresh;

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
    public Dictionary<string, IController> Controllers { get; private set; } = new();

    /// <summary/>
    public VirtualController(string filePath, Implementations.Implementations implementations = DInput | XInput)
    {
        FilePath = filePath;
        Mappings = MappingSet.ReadOrCreateFrom(filePath);

        var managers = new List<IControllerManager>();
        if ((implementations & DInput) == DInput)
            managers.Add(new DInputManager(this));
        if ((implementations & XInput) == XInput)
            managers.Add(new XInputManager(this));

        Managers = managers.ToArray();
        Refresh();
    }
    
    /// <summary/>
    public VirtualController(string filePath) : this(filePath, XInput | DInput) { }

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
    /// <param name="mappingId">The index to unmap.</param>
    public void UnMap(int mappingId)
    {
        Mappings.Mappings.Remove(mappingId);
    }

    /// <summary>
    /// Unmaps a mapping with a specified index and mapping no.
    /// </summary>
    /// <param name="mappingId">The index to unmap.</param>
    /// <param name="mappingNo">The </param>
    public void UnMap(int mappingId, int mappingNo)
    {
        if (Mappings.Mappings.TryGetValue(mappingId, out var multiMap))
            multiMap.Mappings.Remove(mappingNo);
    }

    /// <summary>
    /// Polls all the controllers managed by this instance.
    /// </summary>
    public void PollAll()
    {
        foreach (var value in Controllers.Values)
            value.Poll();
    }

    /// <summary>
    /// Polls controllers for a change in inputs and maps a specified button. 
    /// </summary>
    /// <param name="mappingId">Unique ID for the mapping.</param>
    /// <param name="type">The type of mapping.</param>
    /// <param name="mappingNo">The unique number. e.g. 0 for button 1, 1 for button 2 corresponding to same action.</param>
    /// <param name="token">Allows for cancelling the mapping process.</param>
    /// <param name="callback">Executed after every poll attempt for a key or axis.</param>
    public async Task<bool> Map(int mappingId, MappingType type, int mappingNo, CancellationToken token = default, Action? callback = null)
    {
        PollAll();
        var controllerCaches = GetControllerCaches();

        try
        {
            if (type == MappingType.Button)
                return await Map_Button(mappingId, type, mappingNo, token, callback, controllerCaches);

            if (type == MappingType.Axis)
                return await Map_Axis(mappingId, type, mappingNo, token, callback, controllerCaches);
        }
        catch (TaskCanceledException)
        {
            return false;
        }

        return false;
    }

    private async Task<bool> Map_Axis(int index, MappingType type, int mappingNo, CancellationToken token, Action? callback, List<ControllerCache> controllerCaches)
    {
        var halfMaxAxis = AxisSet.MaxValue / 2;
        while (!token.IsCancellationRequested)
        {
            foreach (var cache in controllerCaches)
            {
                var controller = cache.Controller;
                controller.Poll();
                var newAxis = controller.GetAxis();
                for (int x = 0; x < AxisSet.NumberOfAxis; x++)
                {
                    var difference = MathF.Abs(cache.Axis.GetAxis(x) - newAxis.GetAxis(x));
                    if (difference < halfMaxAxis)
                        continue;

                    var mapping = Mappings.GetOrCreateMapping(index, type);
                    mapping.SetMapping(mappingNo, new Mapping(cache.Controller.GetId(), x));
                    return true;
                }
            }

            callback?.Invoke();
            await Task.Delay(MapSleepTime);
        }

        return false;
    }

    private async Task<bool> Map_Button(int index, MappingType type, int mappingNo, CancellationToken token, Action? callback, List<ControllerCache> controllerCaches)
    {
        while (!token.IsCancellationRequested)
        {
            foreach (var cache in controllerCaches)
            {
                var controller = cache.Controller;
                controller.Poll();
                var newButtons = controller.GetButtons();
                for (int x = 0; x < ButtonSet.NumberOfButtons; x++)
                {
                    if (newButtons.GetButton(x) == cache.Buttons.GetButton(x))
                        continue;

                    var mapping = Mappings.GetOrCreateMapping(index, type);
                    mapping.SetMapping(mappingNo, new Mapping(cache.Controller.GetId(), x));
                    return true;
                }
            }

            callback?.Invoke();
            await Task.Delay(MapSleepTime);
        }

        return false;
    }

    /// <summary>
    /// Gets a friendly name for a given mapping.
    /// </summary>
    /// <param name="mappingId">The index to test.</param>
    public string GetFriendlyMappingName(int mappingId)
    {
        if (!Mappings.Mappings.TryGetValue(mappingId, out MultiMapping? value)) 
            return "Unmapped";
        
        return value.GetFriendlyName(Controllers);
    }

    /// <summary>
    /// Gets a friendly name for a given mapping.
    /// </summary>
    /// <param name="mappingId">The index to test.</param>
    /// <param name="mappingNo">The unique number. e.g. 0 for button 1, 1 for button 2 corresponding to same action.</param>
    public string GetFriendlyMappingName(int mappingId, int mappingNo)
    {
        if (!Mappings.Mappings.TryGetValue(mappingId, out MultiMapping? value))
            return "Unmapped";

        return value.GetFriendlyName(Controllers, mappingNo);
    }

    /// <summary>
    /// Gets a mapping for a given index.
    /// </summary>
    /// <param name="mappingId">The index to test.</param>
    public MultiMapping? GetMapping(int mappingId)
    {
        if (Mappings.Mappings.TryGetValue(mappingId, out MultiMapping? value))
            return value;

        return null;
    }

    /// <summary>
    /// Gets a mapping for a given index.
    /// </summary>
    /// <param name="mappingId">The index to test.</param>
    /// <param name="mappingNo">Number of the mapping for this index.</param>
    public Mapping? GetMapping(int mappingId, int mappingNo)
    {
        if (!Mappings.Mappings.TryGetValue(mappingId, out MultiMapping? multiMapping))
            return null;

        if (multiMapping.Mappings.TryGetValue(mappingNo, out var mapping))
            return mapping;

        return null;
    }

    /// <summary>
    /// Gets a bool value for a given mapping index.
    /// </summary>
    /// <param name="mappingIndex"></param>
    /// <returns>Value for the index, else default (false)</returns>
    public bool GetButton(int mappingIndex)
    {
        if (!Mappings.Mappings.TryGetValue(mappingIndex, out MultiMapping? value)) 
            return false;
        
        return value.GetButton(Controllers);
    }

    /// <summary>
    /// Gets a float value for a given mapping index.
    /// </summary>
    /// <param name="mappingIndex">The index the key is mapped to.</param>
    /// <returns>Value for the index, else default (0.0)</returns>
    public float GetAxis(int mappingIndex)
    {
        if (!Mappings.Mappings.TryGetValue(mappingIndex, out MultiMapping? value)) 
            return 0.0f;
        
        return value.GetAxis(Controllers);
    }

    private List<ControllerCache> GetControllerCaches()
    {
        var caches = new List<ControllerCache>(Controllers.Values.Count);
        foreach (var values in Controllers.Values) 
            caches.Add(new ControllerCache(values));

        return caches;
    }
}