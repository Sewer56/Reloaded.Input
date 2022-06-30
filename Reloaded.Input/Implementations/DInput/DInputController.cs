using Reloaded.Input.Implementations.DInput.Enums;
using Reloaded.Input.Interfaces;
using Reloaded.Input.Structs;
using SharpGen.Runtime;
using Vortice.DirectInput;

namespace Reloaded.Input.Implementations.DInput;

/// <summary/>
public class DInputController : IController, IDisposable
{
    private static readonly DpadDirection[] _directions = DpadDirectionExtensions.GetValues();

    /// <summary/>
    public IDirectInputDevice8 Joystick   { get; private set; }

    /// <summary/>
    public string FriendlyName { get; private set; }

    private ButtonSet _buttons;
    private AxisSet _axis;

    /// <summary/>
    public DInputController(IDirectInputDevice8 controller, string friendlyName)
    {
        Joystick = controller;
        FriendlyName = friendlyName;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Joystick.Unacquire();
        Joystick.Dispose();
    }

    /* Interface Implementation */

    public void Poll()
    {
        try
        {
            Joystick.Poll();
            var state = Joystick.GetCurrentJoystickState();

            // Buttons
            _buttons = new ButtonSet();

            int currentButtonIndex = 0;
            foreach (var button in state.Buttons)
            {
                _buttons.SetButton(currentButtonIndex, button);
                currentButtonIndex += 1;
            }

            // Buttons: DPad
            foreach (var povController in state.PointOfViewControllers)
            {
                foreach (var direction in _directions)
                {
                    _buttons.SetButton(currentButtonIndex, povController == (int)direction);
                    currentButtonIndex += 1;
                }
            }

            // Axis
            int currentAxisIndex = 0;
            _axis = new AxisSet();

            AddAxisFromArray(ref _axis, ref currentAxisIndex, state.AccelerationSliders);
            AddAxisFromArray(ref _axis, ref currentAxisIndex, state.ForceSliders);
            AddAxisFromArray(ref _axis, ref currentAxisIndex, state.Sliders);
            AddAxisFromArray(ref _axis, ref currentAxisIndex, state.VelocitySliders);
            AddAxis(ref _axis, ref currentAxisIndex, state.X);
            AddAxis(ref _axis, ref currentAxisIndex, state.Y);
            AddAxis(ref _axis, ref currentAxisIndex, state.Z);
            AddAxis(ref _axis, ref currentAxisIndex, state.RotationX);
            AddAxis(ref _axis, ref currentAxisIndex, state.RotationY);
            AddAxis(ref _axis, ref currentAxisIndex, state.RotationZ);
            AddAxis(ref _axis, ref currentAxisIndex, state.VelocityX);
            AddAxis(ref _axis, ref currentAxisIndex, state.VelocityY);
            AddAxis(ref _axis, ref currentAxisIndex, state.VelocityZ);
            AddAxis(ref _axis, ref currentAxisIndex, state.AngularVelocityX);
            AddAxis(ref _axis, ref currentAxisIndex, state.AngularVelocityY);
            AddAxis(ref _axis, ref currentAxisIndex, state.AngularVelocityZ);
            AddAxis(ref _axis, ref currentAxisIndex, state.AccelerationX);
            AddAxis(ref _axis, ref currentAxisIndex, state.AccelerationY);
            AddAxis(ref _axis, ref currentAxisIndex, state.AccelerationZ);
            AddAxis(ref _axis, ref currentAxisIndex, state.AngularAccelerationX);
            AddAxis(ref _axis, ref currentAxisIndex, state.AngularAccelerationY);
            AddAxis(ref _axis, ref currentAxisIndex, state.AngularAccelerationZ);
            AddAxis(ref _axis, ref currentAxisIndex, state.ForceX);
            AddAxis(ref _axis, ref currentAxisIndex, state.ForceY);
            AddAxis(ref _axis, ref currentAxisIndex, state.ForceZ);
            AddAxis(ref _axis, ref currentAxisIndex, state.TorqueX);
            AddAxis(ref _axis, ref currentAxisIndex, state.TorqueY);
            AddAxis(ref _axis, ref currentAxisIndex, state.TorqueZ);
        }
        catch (SharpGenException e)
        {
            ExceptionHandler(e);
        }
        catch (Exception) { /* Ignored */ }
    }

    /// <inheritdoc />
    public ButtonSet GetButtons() => _buttons;

    /// <inheritdoc />
    public AxisSet GetAxis() => _axis;

    private void ExceptionHandler(SharpGenException ex)
    {
        if (ex.ResultCode == ResultCode.NotAcquired || ex.ResultCode == ResultCode.InputLost)
        {
            try { Joystick.Acquire(); }
            catch (Exception) { /* Ignored */ }
        }
    }

    /// <inheritdoc />
    public string GetId() => Joystick.DeviceInfo.InstanceGuid.ToString();

    /// <inheritdoc />
    public string GetFriendlyName() => FriendlyName;

    static void AddAxis(ref AxisSet set, ref int axisIndex, int value)
    {
        set.SetAxis(axisIndex, value);
        axisIndex += 1;
    }

    static void AddAxisFromArray(ref AxisSet set, ref int axisIndex, int[] axis)
    {
        foreach (var ax in axis)
        {
            set.SetAxis(axisIndex, ax);
            axisIndex += 1;
        }
    }
}