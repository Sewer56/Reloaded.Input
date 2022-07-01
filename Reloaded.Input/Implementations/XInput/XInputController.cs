using Reloaded.Input.Interfaces;
using Reloaded.Input.Structs;
using Vortice.XInput;

namespace Reloaded.Input.Implementations.XInput;

/// <inheritdoc />
public class XInputController : IController
{
    private static readonly GamepadButtons[] _allButtons = Enum.GetValues<GamepadButtons>();
    private static readonly string[] _buttonNames = Enum.GetNames<GamepadButtons>();
    
    /// <summary/>
    public int ControllerIndex { get; set; }

    private ButtonSet _buttons;
    private AxisSet _axis;

    /// <summary/>
    public XInputController(int index)
    {
        ControllerIndex = index;
    }

    /* Interface Implementation */
    public void Poll()
    {
        Vortice.XInput.XInput.GetState(ControllerIndex, out var state);

        // Buttons
        _buttons = new ButtonSet();

        var buttons = state.Gamepad.Buttons;
        for (int x = 0; x < _allButtons.Length; x++)
            _buttons.SetButton(x, (buttons & _allButtons[x]) == _allButtons[x]);

        // Axis
        _axis = new AxisSet();
        _axis.SetAxis(0, ScaleAxis(state.Gamepad.LeftThumbX));
        _axis.SetAxis(1, ScaleAxis(state.Gamepad.LeftThumbY));
        _axis.SetAxis(2, ScaleAxis(state.Gamepad.RightThumbX));
        _axis.SetAxis(3, ScaleAxis(state.Gamepad.RightThumbY));
        _axis.SetAxis(4, ScaleTrigger(state.Gamepad.LeftTrigger));
        _axis.SetAxis(5, ScaleTrigger(state.Gamepad.RightTrigger));
    }

    /// <inheritdoc />
    public ButtonSet GetButtons() => _buttons;

    /// <inheritdoc />
    public AxisSet GetAxis() => _axis;

    /// <inheritdoc />
    public string GetFriendlyButtonName(int index) => _buttonNames[index];

    /// <inheritdoc />
    public string GetFriendlyAxisName(int index)
    {
        return index switch
        {
            0 => "Left Stick X",
            1 => "Left Stick Y",
            2 => "Right Stick X",
            3 => "Right Stick Y",
            4 => "Left Trigger",
            5 => "Right Trigger",
            _ => "Unknown"
        };
    }

    /// <inheritdoc />
    public string GetId() => GetFriendlyName();

    /// <inheritdoc />
    public string GetFriendlyName() => $"XInput {ControllerIndex}";

    private float ScaleAxis(float value) => (value / short.MaxValue) * AxisSet.MaxValue;
    private float ScaleTrigger(float value)
    {
        return ((value / byte.MaxValue) * (AxisSet.MaxValue * 2)) - AxisSet.MaxValue;
    }
}