using NetEscapades.EnumGenerators;
using System;

namespace Reloaded.Input.Implementations.DInput.Enums
{
    /// <summary>
    /// Values for the individual directions of the directional pad. 
    /// These are the common values for a 8 directional DPad.
    /// In reality, this value is analog, with a range of 0-36000, albeit this is practically never used.
    /// </summary>
    [Flags]
    [EnumExtensions]
    public enum DpadDirection : int
    {
        Up = 0,
        UpRight = 4500,
        UpLeft = 31500,

        Right = 9000,
        Left = 27000,

        Down = 18000,
        DownRight = 13500,
        DownLeft = 22500,
    }
}
