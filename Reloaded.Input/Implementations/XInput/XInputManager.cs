using Reloaded.Input.Interfaces;

namespace Reloaded.Input.Implementations.XInput;

public class XInputManager : IControllerManager
{
    public VirtualController VirtualController { get; private set; }
    public IController[] Controllers { get; private set; }

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
    public IController[] GetControllers() => Controllers;
    public VirtualController GetRemapper() => VirtualController;
}