﻿namespace Reloaded.Input.Interfaces;

public interface IControllerManager
{
    /// <summary>
    /// Retrieves all controllers belonging to the manager.
    /// </summary>
    IController[] GetControllers();

    /// <summary>
    /// Gets the original remapper this class was instantiated with.
    /// </summary>
    VirtualController GetRemapper();
}