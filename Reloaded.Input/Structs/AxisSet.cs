namespace Reloaded.Input.Structs
{
    public unsafe struct AxisSet
    {
        /// <summary>
        /// Represents the maximum positive value an axis can hold.
        /// </summary>
        public const float MaxValue = 10000F;

        /// <summary>
        /// Represents the maximum negative value an axis can hold.
        /// </summary>
        public const float MinValue = -10000F;

        public const int NumberOfAxis = 64;
        private fixed float _axis[NumberOfAxis];

        /// <summary>
        /// Gets the value of an axis.
        /// </summary>
        /// <param name="index">Index between 0 inclusive and <see cref="NumberOfAxis"/> exclusive</param>
        /// <returns>True if button pressed, else false.</returns>
        public float GetAxis(int index) => _axis[index];

        /// <summary>
        /// Sets the value of an axis.
        /// </summary>
        /// <param name="index">Index between 0 inclusive and <see cref="NumberOfAxis"/> exclusive</param>
        /// <param name="value">The value.</param>
        /// <returns>True if button pressed, else false.</returns>
        public float SetAxis(int index, float value) => _axis[index] = value;
    }
}
