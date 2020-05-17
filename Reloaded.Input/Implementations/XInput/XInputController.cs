using EnumsNET;
using Reloaded.Input.Interfaces;
using Reloaded.Input.Structs;
using SharpDX.XInput;

namespace Reloaded.Input.Implementations.XInput
{
    public class XInputController : IController
    {
        public SharpDX.XInput.Controller Controller { get; private set; }

        public XInputController(SharpDX.XInput.Controller controller)
        {
            Controller = controller;
        }

        /* Interface Implementation */
        public ButtonSet GetButtons()
        {
            var buttonSet = new ButtonSet();
            Controller.GetState(out var state);
            
            var buttons = state.Gamepad.Buttons;
            var flags = Enums.GetMembers<GamepadButtonFlags>();
            for (int x = 0; x < flags.Count; x++) 
                buttonSet.SetButton(x, buttons.HasAllFlags(flags[x].Value));

            return buttonSet;
        }

        public AxisSet GetAxis()
        {
            var axisSet = new AxisSet();
            Controller.GetState(out var state);

            axisSet.SetAxis(0, ScaleAxis(state.Gamepad.LeftThumbX));
            axisSet.SetAxis(1, ScaleAxis(state.Gamepad.LeftThumbY));
            axisSet.SetAxis(2, ScaleAxis(state.Gamepad.RightThumbX));
            axisSet.SetAxis(3, ScaleAxis(state.Gamepad.RightThumbY));
            axisSet.SetAxis(4, ScaleTrigger(state.Gamepad.LeftTrigger));
            axisSet.SetAxis(5, ScaleTrigger(state.Gamepad.RightTrigger));
            
            return axisSet;
        }

        public string GetId() => GetFriendlyName();
        public string GetFriendlyName() => $"XInput {(int)Controller.UserIndex}";

        private float ScaleAxis(float value) => (value / short.MaxValue) * AxisSet.MaxValue;
        private float ScaleTrigger(float value)
        {
            return ((value / byte.MaxValue) * (AxisSet.MaxValue * 2)) - AxisSet.MaxValue;
        }
    }
}
