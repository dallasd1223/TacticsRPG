using Sandbox;
using System;

namespace StairsTool;

/// <summary>
/// Defines different types of stair shapes that can be generated.
/// </summary>
public enum StairShape
{
    /// <summary>
    /// A straight staircase with no turns
    /// </summary>
    Straight,
    
    /// <summary>
    /// A curved staircase that turns while ascending
    /// </summary>
    Curved,
    
    /// <summary>
    /// A staircase with a landing/platform between two flights
    /// </summary>
    Landing
}
