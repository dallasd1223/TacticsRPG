using Sandbox;
using System;

namespace TacticsRPG;

public sealed class WinController : Component
{
	[Property] public TeamManager AlphaTeam {get; set;}
	[Property] public TeamManager OmegaTeam {get; set;}
}
