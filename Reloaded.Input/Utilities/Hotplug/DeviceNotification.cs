using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Reloaded.Input.Utilities.Hotplug.Structs;

namespace Reloaded.Input.Utilities.Hotplug;

/// <summary>
/// Allows for listening to individual device changes such as the change in connected controllers.
/// </summary>
public class DeviceNotification : IDisposable
{
    private readonly IntPtr _notificationHandle;
    private readonly IntPtr _buffer;
    private bool _disposed = false;

    /// <param name="windowHandle">Handle to the window receiving notifications.</param>
    /// <param name="usbOnly">true to filter to USB devices only, false to be notified for all devices.</param>
    public unsafe DeviceNotification(IntPtr windowHandle, bool usbOnly)
    {
        // Define filter.
        var deviceBroadcastInterface = new DEV_BROADCAST_DEVICEINTERFACE
        {
            deviceType = Native.DBT_DEVTYP_DEVICEINTERFACE,
            reserved = 0,
            classGuid = Native.GUID_DEVINTERFACE_USB_DEVICE,
            name = 0,
            size = Unsafe.SizeOf<DEV_BROADCAST_DEVICEINTERFACE>()
        };

        // Manually marshal filter to unmanaged memory.
        _buffer = Marshal.AllocHGlobal(deviceBroadcastInterface.size);
        Unsafe.Write((void*)_buffer, deviceBroadcastInterface);

        // Register for device notifications.
        _notificationHandle = Native.RegisterDeviceNotification(windowHandle, _buffer, usbOnly ? 0 : Native.DEVICE_NOTIFY_ALL_INTERFACE_CLASSES);
    }

    /// <summary/>
    ~DeviceNotification() => Dispose(false);

    /// <summary/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary/>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        ReleaseUnmanagedResources();
        _disposed = true;
    }

    private void ReleaseUnmanagedResources()
    {
        Marshal.FreeHGlobal(_buffer);
        Native.UnregisterDeviceNotification(_notificationHandle);
    }
}