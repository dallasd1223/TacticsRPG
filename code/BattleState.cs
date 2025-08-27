using Sandbox;
using System;

namespace TacticsRPG;

public class Battlestate : State
{
	protected BattleMachine Machine {get; set;}

	protected override void OnAwake()
	{
		BattleEvents.StateHasChanged(this);
		Machine = GetComponent<BattleMachine>();
	}


	protected override void AddListeners()
	{
		Machine.Input.InputPressed += HandleInput;
	}

	protected override void RemoveListeners()
	{
		Machine.Input.InputPressed -= HandleInput;
	}

	protected virtual void HandleInput(InputKey key)
	{
		Log.Info($"Key Pressed: {key}");
	}
	public Battlestate() {}
}

[Category("Battle State")]
public partial class BattleMachine : StateMachine
{
	public static BattleMachine Instance;
	//Core Data (Resource)
	[Property] public BattleConfig Config;
	[Property] public BattleData Data;

	//Input
	[Property] public InputManager Input {get; set;}

	//Visual
	[Property] public CameraManager Camera;
	[Property] public MapManager Map;

	[Property] public PlayerMaster Player;
	//Battle Drivers
	[Property] public TurnManager Turn;
	[Property] public TurnQueueController TurnQueue;
	[Property] public WinController WinCondition;
	[Property] public CommandHandler commandHandler;

	//UI
	[Property] public BattleIntroUI IntroUI;
	[Property] public BattleEndUI EndUI;

	protected override void OnStart()
	{
		Log.Info("Init State Set");
		ActionLog.SetupEventHooks();
		ChangeState<BattleStartState>();
	}

	protected override void OnUpdate()
	{
		if(!_activeState.IsValid()) return;
		_activeState.Update();
	}

	protected override void OnDestroy()
	{
		Instance = null;
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

	protected override void OnStart()
	{
		InitializeBattle();
	}

	public void InitializeBattle()
	{

		Machine.TurnQueue.BuildQueue();
		var seq = new EffectSequence();

		seq.AddStep(new BetterCameraSpiralInStep{
			FocusTarget = Machine.Map.Node,
			endPosition = new Vector3(-407.852f,-606.5532f,435.9904f),
			Revolutions = 1f,
			HeightStart = 1000f,
			StartTime = 0f,
			Duration = Machine.Config.INTRO_SPIRAL_DURATION,
			action = () => Log.Info("YA MAMA"),

		});

		EffectManager.Instance.AddSequence(seq,() => Machine.ChangeState<TurnStartState>());
	}

	protected override void HandleInput(InputKey key)
	{
		base.HandleInput(key);
		switch(key)
		{
			case InputKey.BACKSPACE:
				var b = EffectManager.Instance.TrySkipSequence();
				BattleMachine.Instance.IntroUI.Deactivate();
				Log.Info($"Trying To Skip: {b}");
				break;
		}
	}

	public override void Update()
	{
		
	}

	public override void Exit()
	{
		base.Exit();
	}


}

[Category("Battle State")]
public class TurnStartState : Battlestate
{
	public override void Enter()
	{
		base.Enter();
		Log.Info("TurnStartState Entered");
		InitiateTurn();
	}

	public void InitiateTurn()
	{
		Machine.Turn.StartTurn(Machine.TurnQueue.NextInQueue());

		CameraManager.Instance.DirectCameraFocus(Machine.Turn.ActiveUnit);

	}

	public override void Update()
	{
		Machine.ChangeState<ActionCommandSelectState>();		
	}

	public override void Exit()
	{
		base.Exit();
	}

	protected override void HandleInput(InputKey key)
	{
		base.HandleInput(key);

	
	}
}

[Category("Battle State")]
public class ActionCommandSelectState : Battlestate
{
	public override void Enter()
	{
		base.Enter();
		Log.Info("ActionCommandSelectState Entered");

	}

	public void HandleTurnCommands()
	{
		if(Machine.Turn.ActiveUnit.isAIControlled)
		{
			if(!Machine.Turn.ActiveUnit.AI.HasTakenTurn && !Machine.Turn.EnsureAITurn)
			{
				Machine.Turn.EnsureAITurn = true;
				Log.Info("BattleManager AI Turn");
				Machine.Turn.ActiveUnit.AI.TakeTurn();
			}

		}
	}

	public override void Update()
	{
			HandleTurnCommands();	
	}

	public override void Exit()
	{
		base.Exit();
	}
	protected override void HandleInput(InputKey key)
	{
		base.HandleInput(key);
		InputEvents.OnActionSelectPressed(key);
	}
}

[Category("Battle State")]
public class ExecuteActionState : Battlestate
{
	protected override void AddListeners()
	{
		base.AddListeners();
		Machine.commandHandler.ProcessComplete += OnProcessFinished;
	}

	public void OnProcessFinished()
	{
		Machine.ChangeState<PostActionState>();
	}

	public override void Enter()
	{
		base.Enter();
		Log.Info("ExecuteActionState Entered");
	}

	public override void Update()
	{
		Machine.commandHandler.ProcessCommands();
	}

	public override void Exit()
	{
		base.Exit();
	}
	protected override void HandleInput(InputKey key)
	{
		base.HandleInput(key);
	}
}

[Category("Battle State")]
public class PostActionState : Battlestate
{


	public override void Enter()
	{
		base.Enter();
		Log.Info("PostActionState Entered");
		
	}

	public void DecideTurnState()
	{
		switch(Machine.Turn.CheckUnitTurnEnded())
		{
			case true:
				Machine.ChangeState<TurnEndState>();
				break;
			case false:
				Machine.ChangeState<ActionCommandSelectState>();
				break;
		}
	}

	public override void Update()
	{
		stateMachine.ChangeState<TurnEndState>();
	}

	public override void Exit()
	{
		base.Exit();
	}
	protected override void HandleInput(InputKey key)
	{
		base.HandleInput(key);
	}

}

[Category("Battle State")]
public class TurnEndState : Battlestate
{
	public override void Enter()
	{
		base.Enter();
		Log.Info("TurnEndState Entered");
	}

	public override void Update()
	{
		stateMachine.ChangeState<BattleEndState>();
	}

	public override void Exit()
	{
		base.Exit();
	}
	protected override void HandleInput(InputKey key)
	{
		base.HandleInput(key);
	}

}

[Category("Battle State")]
public class BattleEndState : Battlestate
{
	public override void Enter()
	{
		base.Enter();
		Log.Info("BattleEndState Entered");
	}

	public override void Update()
	{

	}

	public override void Exit()
	{
		base.Exit();
	}
	protected override void HandleInput(InputKey key)
	{
		base.HandleInput(key);
	}

}
