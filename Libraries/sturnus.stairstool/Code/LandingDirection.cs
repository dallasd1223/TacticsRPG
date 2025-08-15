using Sandbox;
using System;

namespace StairsTool;

/// <summary>
/// Defines the direction of the second flight of stairs after the landing,
/// from the perspective of someone standing at the bottom of the staircase looking up.
/// </summary>
public enum LandingDirection
{
    /// <summary>
    /// Second flight continues in the same direction as the first flight
    /// </summary>
    Straight,
    
    /// <summary>
    /// Second flight turns left from the landing (when viewed from bottom of stairs)
    /// </summary>
    Left,
    
    /// <summary>
    /// Second flight turns right from the landing (when viewed from bottom of stairs)
    /// </summary>
    Right/*,
    
    /// <summary>
    /// Second flight goes back in the opposite direction as the first flight (U-shaped stairs)
    /// </summary>
    U*/
}
