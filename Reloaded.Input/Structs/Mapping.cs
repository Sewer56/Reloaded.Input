using Reloaded.Input.Interfaces;

namespace Reloaded.Input.Structs
{
    public class Mapping
    {
        /// <summary>
        /// The ID of the controller this mapping is assigned to.
        /// </summary>
        public string ControllerId { get; private set; }

        /// <summary>
        /// The mapping type, axis or button.
        /// </summary>
        public MappingType MappingType { get; private set; }

        /// <summary>
        /// The index of <see cref="MappingType"/> the button was mapped to.
        /// </summary>
        public int Index { get; private set; }

        public Mapping(string controllerId, MappingType mappingType, int index)
        {
            ControllerId = controllerId;
            MappingType = mappingType;
            Index = index;
        }

        /// <summary>
        /// Gets a friendly name for the mapping.
        /// </summary>
        /// <param name="controller"></param>
        public string GetFriendlyName(IController controller)
        {
            if (MappingType == MappingType.Button)
                return $"{controller.GetFriendlyName()}/B{Index}";

            return $"{controller.GetFriendlyName()}/A{Index}";
        }

        /// <summary>
        /// Gets the mapped value from the passed in controller instance.
        /// </summary>
        public void GetValue(ref AxisSet axis, out float value)
        {
            value = 0.0f;
            switch (MappingType)
            {
                case MappingType.Axis:
                    value = axis.GetAxis(Index);
                    break;
            }
        }

        /// <summary>
        /// Gets the mapped value from the passed in controller instance.
        /// </summary>
        public void GetValue(ref ButtonSet buttons, out bool value)
        {
            switch (MappingType)
            {
                case MappingType.Button:
                    value = buttons.GetButton(Index);
                    break;
                default:
                    value = false;
                    break;
            }
        }
    }

    /// <summary>
    /// Declares the types of mappings supported.
    /// </summary>
    public enum MappingType
    {
        Button,
        Axis
    }
}
