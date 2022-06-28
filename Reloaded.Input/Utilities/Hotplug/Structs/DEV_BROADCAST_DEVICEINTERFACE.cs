using System;
using System.Runtime.InteropServices;

namespace Reloaded.Input.Utilities.Hotplug.Structs;

/// <summary>
/// Struct which represents a filter for device notifications which contains information about a class of devices.
/// </summary>
/// <a href="https://msdn.microsoft.com/en-us/library/windows/desktop/aa363244(v=vs.85).aspx"/>
[StructLayout(LayoutKind.Sequential)]
internal struct DEV_BROADCAST_DEVICEINTERFACE
{
    /// <summary>
    /// The size of this structure, in bytes. This is the size of the members plus the actual length of the dbcc_name string (null terminator accounted).
    /// </summary>
    internal int size;

    /// <summary>
    /// Set to DBT_DEVTYPE_DEVICEINTERFACE. The device type, which determines the event-specific information that follows the first three members.
    /// </summary>
    internal int deviceType;

    /// <summary>
    /// Do not use.
    /// </summary>
    internal int reserved;

    /// <summary>
    /// The GUID for the interface device class.
    /// </summary>
    internal Guid classGuid;

    /// <summary>
    /// Device name (ignore).
    /// </summary>
    internal short name;
}