using Sandbox;
using System;

namespace TacticsRPG;

public sealed class TurnManager : Component
{
	[Property] public Unit ActiveUnit {get; set;}
	[Property] public TeamType CurrentTeam {get; set;}

	public bool HasMoved = false;
	public bool HasActed = false;

	public event Action<TurnEventArgs> TurnStarted;

	public void StartTurn(Unit unit)
	{
		if(!unit.IsValid()) return;

		ActiveUnit = unit;
		ActiveUnit.IsTurn = true;
		CurrentTeam = ActiveUnit.Team;

		ActiveUnit.Turn.StartTurn();
		TurnStarted?.Invoke(new TurnEventArgs(ActiveUnit, CurrentTeam));
	}

	public void EndTurn(Unit unit)
	{

	}

	public void ChangeTurn(Unit unit)
	{

	}
}

public class TurnEventArgs : EventArgs
{
	public Unit unit;
	public TeamType team;

	public TurnEventArgs(Unit u, TeamType t)
	{
		unit = u;
		team = t;
	}
}
