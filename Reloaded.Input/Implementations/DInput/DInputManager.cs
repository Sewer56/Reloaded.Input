using Reloaded.Input.Interfaces;
using Reloaded.Input.Structs;
using Reloaded.Input.Utilities.Hotplug;
using Vortice.DirectInput;

namespace Reloaded.Input.Implementations.DInput;

/// <summary/>
public class DInputManager : IControllerManager, IDisposable
{
    /// <summary/>
    public VirtualController VirtualController { get; private set; }

    /// <summary/>
    public IController[] Controllers { get; private set; } = Array.Empty<IController>();
    
    private IDirectInput8 DirectInput { get; }
    private Hotplugger Hotplugger { get; }

    /// <summary/>
    public DInputManager(VirtualController virtualController)
    {
        VirtualController = virtualController;
        DirectInput = Vortice.DirectInput.DInput.DirectInput8Create();
        Hotplugger = new Hotplugger();
        Refresh();

        Hotplugger.OnConnectedDevicesChanged += () =>
        {
            Refresh();
            VirtualController.Refresh();
        };
    }

    /// <inheritdoc />
    public void Dispose()
    {
        DisposeControllers();
        DirectInput?.Dispose();
        Hotplugger?.Dispose();
    }

    /// <summary/>
    public void Refresh()
    {
        DisposeControllers();
        var devices     = DirectInput.GetDevices(DeviceClass.All, DeviceEnumerationFlags.AttachedOnly);
        var controllers = new List<IController>(devices.Count);

        for (var x = 0; x < devices.Count; x++)
            controllers.Add(AcquireController(devices[x], x));

        Controllers = controllers.ToArray();
    }

    private IController AcquireController(DeviceInstance device, int controllerIndex)
    {
        // Initialize Joystick/Controller
        var controller = DirectInput.CreateDevice(device.InstanceGuid);
        if (controller.DeviceInfo.Type == DeviceType.Mouse)
            controller.Properties.AxisMode = DeviceAxisMode.Relative;

        // Set buffer size
        controller.SetDataFormat<RawJoystickState>();

        // Clamp axis to our values.
        foreach (var deviceObject in controller.GetObjects())
            if ((deviceObject.ObjectId.Flags & DeviceObjectTypeFlags.AbsoluteAxis) == DeviceObjectTypeFlags.AbsoluteAxis)
                controller.Properties.Range = new InputRange((int) AxisSet.MinValue, (int) AxisSet.MaxValue);

        // Acquire the DInput Device
        controller.Acquire();
        return new DInputController(controller, $"DInput {controllerIndex}");
    }

    // Interface Implementation
    /// <inheritdoc />
    public IController[] GetControllers() => Controllers;

    /// <inheritdoc />
    public VirtualController GetRemapper() => VirtualController;

    private void DisposeControllers()
    {
        if (Controllers == null) 
            return;

        foreach (var controller in Controllers)
            ((DInputController) controller).Dispose();
    }
}