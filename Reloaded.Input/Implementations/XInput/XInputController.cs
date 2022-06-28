using System;
using Reloaded.Input.Interfaces;
using Reloaded.Input.Structs;
using Vortice.XInput;

namespace Reloaded.Input.Implementations.XInput;

public struct XInputController : IController
{
    private static GamepadButtons[] _buttons = Enum.GetValues<GamepadButtons>();

    public int ControllerIndex { get; set; }

    public XInputController(int index)
    {
        ControllerIndex = index;
    }

    /* Interface Implementation */
    public ButtonSet GetButtons()
    {
        var buttonSet = new ButtonSet();
        Vortice.XInput.XInput.GetState(ControllerIndex, out var state);

        var buttons = state.Gamepad.Buttons;
        for (int x = 0; x < _buttons.Length; x++) 
            buttonSet.SetButton(x, (buttons & _buttons[x]) == _buttons[x]);

        return buttonSet;
    }

    public AxisSet GetAxis()
    {
        var axisSet = new AxisSet();
        Vortice.XInput.XInput.GetState(ControllerIndex, out var state);

        axisSet.SetAxis(0, ScaleAxis(state.Gamepad.LeftThumbX));
        axisSet.SetAxis(1, ScaleAxis(state.Gamepad.LeftThumbY));
        axisSet.SetAxis(2, ScaleAxis(state.Gamepad.RightThumbX));
        axisSet.SetAxis(3, ScaleAxis(state.Gamepad.RightThumbY));
        axisSet.SetAxis(4, ScaleTrigger(state.Gamepad.LeftTrigger));
        axisSet.SetAxis(5, ScaleTrigger(state.Gamepad.RightTrigger));
            
        return axisSet;
    }

    public string GetId() => GetFriendlyName();
    public string GetFriendlyName() => $"XInput {ControllerIndex}";

    private float ScaleAxis(float value) => (value / short.MaxValue) * AxisSet.MaxValue;
    private float ScaleTrigger(float value)
    {
        return ((value / byte.MaxValue) * (AxisSet.MaxValue * 2)) - AxisSet.MaxValue;
    }
}