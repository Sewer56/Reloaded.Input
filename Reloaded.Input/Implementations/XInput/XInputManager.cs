using Reloaded.Input.Interfaces;
using SharpDX.XInput;

namespace Reloaded.Input.Implementations.XInput
{
    public class XInputManager : IControllerManager
    {
        public VirtualController VirtualController { get; private set; }
        public IController[] Controllers { get; private set; }

        public XInputManager(VirtualController virtualController)
        {
            VirtualController = virtualController;
            Controllers = new[]
            {
                new XInputController(new SharpDX.XInput.Controller(UserIndex.One)),
                new XInputController(new SharpDX.XInput.Controller(UserIndex.Two)),
                new XInputController(new SharpDX.XInput.Controller(UserIndex.Three)),
                new XInputController(new SharpDX.XInput.Controller(UserIndex.Four)),
            };
        }

        // Interface Implementation
        public IController[] GetControllers() => Controllers;
        public VirtualController GetRemapper() => VirtualController;
    }
}
