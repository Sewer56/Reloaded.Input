using System;
using System.Collections.Generic;
using EnumsNET;
using Reloaded.Input.Interfaces;
using Reloaded.Input.Structs;
using Reloaded.Input.Utilities.Hotplug;
using SharpDX.DirectInput;

namespace Reloaded.Input.Implementations.DInput
{
    public class DInputManager : IControllerManager, IDisposable
    {
        public VirtualController VirtualController { get; private set; }
        public IController[] Controllers { get; private set; }
        private DirectInput DirectInput { get; }
        private Hotplugger Hotplugger { get; }

        public DInputManager(VirtualController virtualController)
        {
            VirtualController = virtualController;
            DirectInput = new DirectInput();
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
            var controller = new Joystick(DirectInput, device.InstanceGuid);
            if (controller.Information.Type == DeviceType.Mouse)
                controller.Properties.AxisMode = DeviceAxisMode.Relative;
            
            // Clamp axis to our values.
            foreach (var deviceObject in controller.GetObjects())
                if (deviceObject.ObjectId.Flags.HasAllFlags(DeviceObjectTypeFlags.AbsoluteAxis))
                    controller.GetObjectPropertiesById(deviceObject.ObjectId).Range = new InputRange((int) AxisSet.MinValue, (int) AxisSet.MaxValue);

            // Acquire the DInput Device
            controller.Acquire();
            return new DInputController(controller, $"DInput {controllerIndex}");
        }

        // Interface Implementation
        public IController[] GetControllers() => Controllers;
        public VirtualController GetRemapper() => VirtualController;

        private void DisposeControllers()
        {
            if (Controllers == null) 
                return;

            foreach (var controller in Controllers)
                ((DInputController) controller).Dispose();
        }
    }
}
