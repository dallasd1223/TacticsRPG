using Sandbox;
using System;

namespace TacticsRPG;

public sealed class TurnManager : Component
{
	public BattleUnit LastUnit;
	[Property] public BattleUnit ActiveUnit {get; set;}
	[Property] public TeamType CurrentTeam {get; set;}

	public bool HasMoved = false;
	public bool HasActed = false;

	public bool EnsureAITurn = false;

	public TurnState CurrentTurnState = TurnState.NA;

	public event Action<TurnEventArgs> TurnEvent;

	public void StartTurn(BattleUnit unit)
	{
		if(!unit.IsValid()) return;

		ActiveUnit = unit;
		ActiveUnit.IsTurn = true;
		ActiveUnit.Turn.StartTurn();

		CurrentTeam = ActiveUnit.Team;
		CurrentTurnState = TurnState.Active;

		BattleEvents.OnTurnStart(ActiveUnit);
		BattleEvents.OnTurnEvent(new TurnEventArgs(ActiveUnit, CurrentTeam, CurrentTurnState));
		TurnEvent?.Invoke(new TurnEventArgs(ActiveUnit, CurrentTeam, CurrentTurnState));
	}

	public bool CheckUnitTurnEnded()
	{
		if(ActiveUnit.Turn.HasMoved && !ActiveUnit.Turn.HasActed)
		{
			Log.Info("Unit Must Act");
			CurrentTurnState = TurnState.MustAct;
			//PlayerMaster.Instance.Mode = FocusMode.Menu;
			return false;

		}
		else if(!ActiveUnit.Turn.HasMoved && ActiveUnit.Turn.HasActed)
		{
			//PlayerMaster.Instance.Mode = FocusMode.Menu;
			Log.Info("Unit Must Move");
			CurrentTurnState = TurnState.MustAct;
			return false;
		}
		else if(ActiveUnit.Turn.HasMoved && ActiveUnit.Turn.HasActed)
		{
			Log.Info("Unit Turn Over");
			CurrentTurnState = TurnState.Finished;
			return true;
		}
		return false;
	}

	public void EndTurn()
	{
		ActiveUnit.IsTurn = false;
		ActiveUnit.Turn.EndTurn();

		LastUnit = ActiveUnit;

		if(ActiveUnit.isAIControlled)
		{
			ActiveUnit.AI.EndTurn();
			EnsureAITurn = false;			
		}


		CurrentTurnState = TurnState.Finished;
		TurnEvent?.Invoke(new TurnEventArgs(ActiveUnit, CurrentTeam, CurrentTurnState));
		BattleEvents.OnTurnEnd(ActiveUnit);
	}

}

public class TurnEventArgs : EventArgs
{
	public BattleUnit unit;
	public TeamType team;
	public TurnState state;

	public TurnEventArgs(BattleUnit u, TeamType t, TurnState s)
	{
		unit = u;
		team = t;
		state = s;
	}
}

public enum TurnState
{
	NA,
	Active,
	MustMove,
	MustAct,
	Finished,
}
