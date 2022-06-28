using Reloaded.Input.Interfaces;

namespace Reloaded.Input.Implementations.XInput;

/// <inheritdoc />
public class XInputManager : IControllerManager
{
    /// <summary/>
    public VirtualController VirtualController { get; private set; }

    /// <summary/>
    public IController[] Controllers { get; private set; }

    /// <summary/>
    public XInputManager(VirtualController virtualController)
    {
        VirtualController = virtualController;
        Controllers = new IController[]
        {
            new XInputController(0),
            new XInputController(1),
            new XInputController(2),
            new XInputController(3),
        };
    }

    // Interface Implementation
    /// <inheritdoc />
    public IController[] GetControllers() => Controllers;

    /// <inheritdoc />
    public VirtualController GetRemapper() => VirtualController;
}