using Sandbox;
using System;

namespace TacticsRPG;

public sealed class WinController : Component
{
	[Property] public TeamManager AlphaTeam {get; set;}
	[Property] public TeamManager OmegaTeam {get; set;}
	[Property] public TeamType WinningTeam {get; set;}

	public bool CheckEndConditions()
	{
		if(AlphaTeam.CheckAllDead())
		{
			Log.Info("All Alpha Team Units Killed. Game Over");
			WinningTeam = TeamType.Alpha;
			//ChangeCurrentState(BattleState.BattleEnd);
			return true;
		}
		else if(OmegaTeam.CheckAllDead())
		{
			//ChangeCurrentState(BattleState.BattleEnd);
			//WinningTeam = TeamType.Omega;
			Log.Info("All Omega Team Units Killed. Game Over");
			return true;
		}
		return false;
	}
}
