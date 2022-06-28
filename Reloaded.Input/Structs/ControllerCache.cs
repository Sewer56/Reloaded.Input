using Reloaded.Input.Interfaces;

namespace Reloaded.Input.Structs;

public class ControllerCache
{
    public IController Controller { get; set; }
    public ButtonSet Buttons { get; set; }
    public AxisSet Axis { get; set; }

    public ControllerCache(IController controller)
    {
        Controller = controller;
        Buttons = controller.GetButtons();
        Axis = controller.GetAxis();
    }
}