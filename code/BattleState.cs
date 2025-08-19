using Sandbox;
using System;

namespace TacticsRPG;

public class Battlestate : State
{
	protected BattleMachine Owner;

	public Battlestate() {}
}

[Category("Battle State")]
public class BattleMachine : StateMachine
{
	public BattleConfig Config;
	public BattleData Data;

	public CameraManager Camera;
	public MapManager Map;

	public TurnQueueController TurnQueue;
	public TurnManager Turn;
	public WinConditionController WinCondition;
	public CommandHandler commandHandler;

	public BattleEndUI EndUI;

	protected override void OnStart()
	{
		Log.Info("Init State Set");
		ChangeState<BattleStartState>();
	}
}

[Category("Battle State")]
public class BattleStartState : Battlestate
{
	public override void Enter()
	{
		base.Enter();
		Log.Info("BattleStartState Entered");
	}

	public override void Update()
	{

	}

	public override void Exit()
	{
		base.Exit();
	}

	protected override void AddListeners()
	{

	}

	protected override void RemoveListeners()
	{

	}
}

[Category("Battle State")]
public class TurnStartState : Battlestate
{

}

[Category("Battle State")]
public class ActionCommandSelectState : Battlestate
{

}

[Category("Battle State")]
public class ExecuteActionState : Battlestate
{

}

[Category("Battle State")]
public class FacingDirectionState : Battlestate
{

}

[Category("Battle State")]
public class TurnEndState : Battlestate
{

}

[Category("Battle State")]
public class BattleEndState : Battlestate
{

}
