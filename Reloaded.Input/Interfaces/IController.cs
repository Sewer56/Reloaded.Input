﻿using Reloaded.Input.Structs;

namespace Reloaded.Input.Interfaces
{
    public interface IController
    {
        /// <summary>
        /// Obtains a string that uniquely identifies the controller.
        /// </summary>
        string GetId();

        /// <summary>
        /// Obtains the friendly name for a controller.
        /// </summary>
        string GetFriendlyName();

        /// <summary>
        /// Retrieves all of the controller buttons.
        /// </summary>
        ButtonSet GetButtons();

        /// <summary>
        /// Retrieves all of the controller axis.
        /// </summary>
        AxisSet GetAxis ();
    }
}
