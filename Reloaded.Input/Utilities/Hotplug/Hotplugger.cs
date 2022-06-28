namespace Reloaded.Input.Utilities.Hotplug;

/// <summary/>
public class Hotplugger : NativeWindow, IDisposable
{
    /// <summary>
    /// Executed when the connected devices changed.
    /// </summary>
    public event Action? OnConnectedDevicesChanged;

    /// <summary>
    /// Used for receiving notifications.
    /// </summary>
    private DeviceNotification NotificationReceiver { get; set; }

    /// <summary>
    /// Allows for receiving of device removal / addition events.
    /// </summary>
    public Hotplugger()
    {
        // Specify HWND_MESSAGE in the hwndParent parameter such that the window only receives messages, no rendering, etc.
        var cp = new CreateParams { Parent = (IntPtr)(Native.HWND_MESSAGE) };
        CreateHandle(cp);
        NotificationReceiver = new DeviceNotification(Handle, false);
    }

    /// <summary/>
    ~Hotplugger()
    {
        Dispose();
    }

    /// <summary/>
    public void Dispose()
    {
        NotificationReceiver?.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Handles window messages. Sends out events on device disconnect or connect.
    /// </summary>
    protected override void WndProc(ref Message m)
    {
        if (m.Msg == Native.WM_DEVICECHANGE)
        {
            switch ((int)m.WParam)
            {
                case Native.DBT_DEVICEREMOVECOMPLETE:
                case Native.DBT_DEVICEARRIVAL:
                    OnConnectedDevicesChanged?.Invoke();
                    break;
            }
        }

        base.WndProc(ref m);
    }
}