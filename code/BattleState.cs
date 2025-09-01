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

	//Visual & Audio
	[Property] public CameraManager Camera;
	[Property] public MapManager Map;
	[Property] public MusicPlayer Music;
	//Player
	[Property] public PlayerMaster Player;
	[Property] public SelectorManager Selector {get; set;}
	
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
	[Property] public bool HasHandledCommandStart {get; set;} = false;

	public override void Enter()
	{
		base.Enter();
		Log.Info("ActionCommandSelectState Entered");

	}
	public void HandlePlayerTurnCommands()
	{
		if(Machine.Turn.ActiveUnit.Team == TeamType.Alpha)
		{
			BattleEvents.OnActionSelectStart(Machine.Turn.ActiveUnit);
			HasHandledCommandStart = true;
		}
	}
	public void HandleAITurnCommands()
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
		if(!HasHandledCommandStart)
		{
			HandlePlayerTurnCommands();
			HandleAITurnCommands();	
		}

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

	protected override void RemoveListeners()
	{
		Machine.commandHandler.ProcessComplete -= OnProcessFinished;		
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
	
	//Check If Team Won Else Continue Turn
	public void CheckIfWinCondition()
	{
		switch(Machine.WinCondition.CheckEndConditions())
		{
			case true:
				Machine.ChangeState<BattleEndState>();
				break;
			case false:
				DecideTurnState();
				break;
		}
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
		CheckIfWinCondition();
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
	public bool HasBegunEndTurn = false;
	public override void Enter()
	{
		base.Enter();
		Log.Info("TurnEndState Entered");
	}

	public override void Update()
	{
		if(HasBegunEndTurn) return;
		EndTurnAndContinue();
	}

	public async void EndTurnAndContinue()
	{
		HasBegunEndTurn = true;
		await Task.DelayRealtimeSeconds(1.5f);
		Machine.Turn.EndTurn();
		Machine.TurnQueue.AddToQueue(Machine.Turn.LastUnit);
		Machine.ChangeState<TurnStartState>();
	}

	public override void Exit()
	{
		base.Exit();
		HasBegunEndTurn = false;
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
		EndSequence();
	}

	private async void EndSequence()
	{
		Machine.Music.StopBattleTheme();

		await Task.DelayRealtimeSeconds(2.5f);

		var seq = new EffectSequence();

		seq.AddStep(new CameraOrbitStep{
			Target = CameraManager.Instance.FocusPoint,
			StartTime = 0,
			Revolutions = 4f,
			Duration = 50f,
			ZoomDistance = 10f,

		});

		EffectManager.Instance.AddSequence(seq);

		//Sound.Play(BattleEndTheme);

		await Task.DelayRealtimeSeconds(1f);
		
		Machine.Music.PlayEndTheme();
		Machine.EndUI.IsActive = true;

		Log.Info("Game Over");		
	}

	protected override void HandleInput(InputKey key)
	{
		base.HandleInput(key);
	}

}
