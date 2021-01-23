using System;
using Reloaded.Input.Implementations.DInput.Enums;
using Reloaded.Input.Interfaces;
using Reloaded.Input.Structs;
using SharpDX;
using SharpDX.DirectInput;

namespace Reloaded.Input.Implementations.DInput
{
    public class DInputController : IController
    {
        public Joystick Joystick   { get; private set; }
        public string FriendlyName { get; private set; }

        public DInputController(Joystick controller, string friendlyName)
        {
            Joystick = controller;
            FriendlyName = friendlyName;
        }

        /* Interface Implementation */
        public ButtonSet GetButtons()
        {
            try
            {
                var buttonSet = new ButtonSet();
                var state = Joystick.GetCurrentState();

                int currentButtonIndex = 0;
                foreach (var button in state.Buttons)
                {
                    buttonSet.SetButton(currentButtonIndex, button);
                    currentButtonIndex += 1;
                }

                // Handle the DPad
                var povDirections = EnumsNET.Enums.GetMembers<DpadDirection>();
                foreach (var povController in state.PointOfViewControllers)
                {
                    foreach (var direction in povDirections)
                    {
                        buttonSet.SetButton(currentButtonIndex, povController == (int)direction.Value);
                        currentButtonIndex += 1;
                    }
                }

                return buttonSet;
            }
            catch (SharpDXException e)
            {
                return new ButtonSet();
            }
        }

        public AxisSet GetAxis()
        {
            try
            {
                var axisSet = new AxisSet();
                var state = Joystick.GetCurrentState();
                int currentButtonIndex = 0;

                AddAxisFromArray(ref axisSet, ref currentButtonIndex, state.AccelerationSliders);
                AddAxisFromArray(ref axisSet, ref currentButtonIndex, state.ForceSliders);
                AddAxisFromArray(ref axisSet, ref currentButtonIndex, state.Sliders);
                AddAxisFromArray(ref axisSet, ref currentButtonIndex, state.VelocitySliders);
                AddAxis(ref axisSet, ref currentButtonIndex, state.X);
                AddAxis(ref axisSet, ref currentButtonIndex, state.Y);
                AddAxis(ref axisSet, ref currentButtonIndex, state.Z);
                AddAxis(ref axisSet, ref currentButtonIndex, state.RotationX);
                AddAxis(ref axisSet, ref currentButtonIndex, state.RotationY);
                AddAxis(ref axisSet, ref currentButtonIndex, state.RotationZ);
                AddAxis(ref axisSet, ref currentButtonIndex, state.VelocityX);
                AddAxis(ref axisSet, ref currentButtonIndex, state.VelocityY);
                AddAxis(ref axisSet, ref currentButtonIndex, state.VelocityZ);
                AddAxis(ref axisSet, ref currentButtonIndex, state.AngularVelocityX);
                AddAxis(ref axisSet, ref currentButtonIndex, state.AngularVelocityY);
                AddAxis(ref axisSet, ref currentButtonIndex, state.AngularVelocityZ);
                AddAxis(ref axisSet, ref currentButtonIndex, state.AccelerationX);
                AddAxis(ref axisSet, ref currentButtonIndex, state.AccelerationY);
                AddAxis(ref axisSet, ref currentButtonIndex, state.AccelerationZ);
                AddAxis(ref axisSet, ref currentButtonIndex, state.AngularAccelerationX);
                AddAxis(ref axisSet, ref currentButtonIndex, state.AngularAccelerationY);
                AddAxis(ref axisSet, ref currentButtonIndex, state.AngularAccelerationZ);
                AddAxis(ref axisSet, ref currentButtonIndex, state.ForceX);
                AddAxis(ref axisSet, ref currentButtonIndex, state.ForceY);
                AddAxis(ref axisSet, ref currentButtonIndex, state.ForceZ);
                AddAxis(ref axisSet, ref currentButtonIndex, state.TorqueX);
                AddAxis(ref axisSet, ref currentButtonIndex, state.TorqueY);
                AddAxis(ref axisSet, ref currentButtonIndex, state.TorqueZ);

                return axisSet;

                // Local Utility Functions
                void AddAxis(ref AxisSet set, ref int buttonIndex, int value)
                {
                    set.SetAxis(currentButtonIndex, value);
                    buttonIndex += 1;
                }

                void AddAxisFromArray(ref AxisSet set, ref int buttonIndex, int[] axis)
                {
                    foreach (var ax in axis)
                    {
                        set.SetAxis(currentButtonIndex, ax);
                        buttonIndex += 1;
                    }
                }
            }
            catch (SharpDXException e)
            {
                return new AxisSet();
            }
        }

        public string GetId() => Joystick.Information.InstanceGuid.ToString();
        public string GetFriendlyName() => FriendlyName;
    }
}
