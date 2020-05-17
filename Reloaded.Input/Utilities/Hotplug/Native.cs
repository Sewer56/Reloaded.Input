using System;
using System.Runtime.InteropServices;

namespace Reloaded.Input.Utilities.Hotplug
{
    internal static class Native
    {
        /// <summary>
        /// The GUID_DEVINTERFACE_USB_DEVICE device interface class is defined for USB devices that are attached to a USB hub.
        /// </summary>
        internal static readonly Guid GUID_DEVINTERFACE_USB_DEVICE = new Guid("A5DCBF10-6530-11D2-901F-00C04FB951ED");

        /// <summary>
        /// Message type which is sent to a window (accessible in C# via Message.Msg).
        /// Notifies an application of a change to the hardware configuration of a device or the computer.
        /// </summary>
        internal const int WM_DEVICECHANGE = 0x0219;

        /// <summary>
        /// The system broadcasts the DBT_DEVICEARRIVAL device event when a device or piece of media has been inserted and becomes available. (Controller Connect)
        /// </summary>
        internal const int DBT_DEVICEARRIVAL = 0x8000;

        /// <summary>
        /// The system broadcasts the DBT_DEVICEREMOVECOMPLETE device event when a device or piece of media has been physically removed.
        /// </summary>
        internal const int DBT_DEVICEREMOVECOMPLETE = 0x8004;

        /// <summary>
        /// The device type, which determines the event-specific information that follows the first three members.
        /// </summary>
        internal const int DBT_DEVTYP_DEVICEINTERFACE = 5;

        //https://msdn.microsoft.com/en-us/library/aa363431(v=vs.85).aspx
        /// <summary>
        /// Notifies the recipient of device interface events for all device interface classes (classGuid is ignored).
        /// Setting this triggers events and reloads for non-USB devices.
        /// </summary>
        internal const int DEVICE_NOTIFY_ALL_INTERFACE_CLASSES = 4;

        /// <summary>
        /// The constant for HWND_MESSAGE, specify this is a parent to a window to make it a message-only window.
        /// </summary>
        internal const long HWND_MESSAGE = -3;

        /// <summary>
        /// Registers the device or type of device for which a window will receive notifications.
        /// </summary>
        /// <param name="recipient">Sets the handle to the window or service to receive the notification/</param>
        /// <param name="notificationFilter">A pointer to a block of data that specifies the type of device for which notifications should be sent. </param>
        /// <param name="flags">See MSDN.</param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern IntPtr RegisterDeviceNotification(IntPtr recipient, IntPtr notificationFilter, int flags);

        /// <summary>
        /// Closes the specified device notification handle.
        /// </summary>
        /// <param name="handle">Handle for the notification returned from RegisterDeviceNotification</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        internal static extern bool UnregisterDeviceNotification(IntPtr handle);
    }
}
