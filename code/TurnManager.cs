using Sandbox;
using System;

namespace TacticsRPG;

public sealed class TurnManager : Component
{
	[Property] public Unit ActiveUnit {get; set;}
	[Property] public TeamType CurrentTeam {get; set;}

	public bool HasMoved = false;
	public bool HasActed = false;

	public bool EnsureAITurn = false;

	public TurnState CurrentTurnState = TurnState.NA;

	public event Action<TurnEventArgs> TurnEvent;

	public void StartTurn(Unit unit)
	{
		if(!unit.IsValid()) return;

		ActiveUnit = unit;
		ActiveUnit.IsTurn = true;
		CurrentTeam = ActiveUnit.Team;
		CurrentTurnState = TurnState.Active;
		ActiveUnit.Turn.StartTurn();
		BattleEvents.OnTurnStart(ActiveUnit);
		TurnEvent?.Invoke(new TurnEventArgs(ActiveUnit, CurrentTeam, CurrentTurnState));
	}

	//Decide Turn Check End Condition Logic
	//WinCondition Should CheckEndConditions
	//I think DecideTurnState Should be 
	//Either End Of CombatManager, In CommandHandler Or SubState For ActionExecution

	
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




	/*public bool CheckEndConditions()
	{
		if(AlphaTeam.CheckAllDead())
		{
			Log.Info("All Alpha Team Units Killed. Game Over");
			WinningTeam = TeamType.Alpha;
			ChangeCurrentState(BattleState.BattleEnd);
			return true;
		}
		else if(OmegaTeam.CheckAllDead())
		{
			ChangeCurrentState(BattleState.BattleEnd);
			WinningTeam = TeamType.Omega;
			Log.Info("All Omega Team Units Killed. Game Over");
			return true;
		}
		return false;
	}*/

	public void EndTurn(Unit unit)
	{
		CurrentTurnState = TurnState.Finished;
		TurnEvent?.Invoke(new TurnEventArgs(ActiveUnit, CurrentTeam, CurrentTurnState));
		BattleEvents.OnTurnStart(ActiveUnit);
	}

	public void ChangeTurn(Unit unit)
	{

	}
}

public class TurnEventArgs : EventArgs
{
	public Unit unit;
	public TeamType team;
	public TurnState state;

	public TurnEventArgs(Unit u, TeamType t, TurnState s)
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
