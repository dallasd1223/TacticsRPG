using Sandbox;

namespace TacticsRPG;

public sealed class BattleManager : Component
{
	public static BattleManager Instance {get; set;}

	[Property] public BattleConfig Config {get; set;}
 	[Property] public MusicPlayer Music {get; set;}
	[Property] public bool Initialized {get; set;} = false;
	[Property] public List<Unit> AllUnits {get; set;}
	[Property] public Queue<Unit> UnitTurnQueue {get; set;}
	[Property] public CommandHandler commandHandler {get; set;}
	[Property] public TeamManager AlphaTeam {get; set;}
	[Property] public TeamManager OmegaTeam {get; set;}
	[Property] public Unit ActiveUnit {get; set;}
	[Property] public TeamType TeamTurn {get; set;} = TeamType.Alpha;
	[Property] public BattleState CurrentBattleState = BattleState.BattleStart;
	[Property] public TeamType? WinningTeam {get; set;} = null;
	[Property] public SoundEvent BattleEndTheme {get; set;}
	[Property] public bool StateHasStarted {get; set;} = false;
	[Property] public bool EndGameStarted {get; set;} = false;
	[Property] public MapManager Map {get; set;}
	[Property] public GameObject MapNode {get; set;}
	[Property] public BattleIntroUI IntroUI {get; set;}
	[Property] public BattleEndUI EndUI {get; set;}
	public BattleReward Reward {get; set;}  = new BattleReward();
	public bool EnsureAITurn = false;

	protected override void OnAwake()
	{
		Instance = this;
		commandHandler = GetComponent<CommandHandler>();
	}

	protected override void OnStart()
	{
		InitializeBattle();
	}
	protected override void OnUpdate()
	{
		switch(CurrentBattleState)
		{
			case BattleState.BattleStart:
				InitializeBattle();
	
				break;
			case BattleState.TurnStart:
				StartTurn();
				ChangeCurrentState(BattleState.WaitForCommands);
				break;
			case BattleState.WaitForCommands:
				HandleTurnCommands();
				break;
			case BattleState.ProcessCommands:
				commandHandler.ProcessCommands();
				break;
			case BattleState.TurnEnd:
				if(!StateHasStarted)
				{
					StateHasStarted = true;
					EndTurn();				
				}
				break;
			case BattleState.BattleEnd:
				if(!EndGameStarted)
				{
					EndGameStarted = true;
					EndBattle();
				}

				break;
		}
	}

	public void InitializeBattle()
	{
		if(Initialized)
		{
			return;
		}
		var units = Scene.GetAll<Unit>();
		AllUnits = units.ToList();
		foreach(Unit unit in AllUnits)
		{
			UnitTurnQueue.Enqueue(unit);
		}
		Initialized = true;

		var seq = new EffectSequence();

		seq.AddStep(new BetterCameraSpiralInStep{
			FocusTarget = MapNode,
			endPosition = new Vector3(-407.852f,-606.5532f,435.9904f),
			Revolutions = 1f,
			HeightStart = 1000f,
			StartTime = 0f,
			Duration = Config.INTRO_SPIRAL_DURATION,
			action = () => Log.Info("YA MAMA"),

		});

		EffectManager.Instance.AddSequence(seq,() => ChangeCurrentState(BattleState.TurnStart));
	}

	public void ChangeCurrentState(BattleState State)
	{
		Log.Info($"Manager {CurrentBattleState} State Changing To {State}");
		CurrentBattleState = State;
	}



	public async void StartTurn()
	{
		StateHasStarted = false;
		if(UnitTurnQueue.Count() == 0)
		{
			Log.Info("No Units Found");
			return;
		}
		ActiveUnit = UnitTurnQueue.Peek();
		ActiveUnit.IsTurn = true;
		ActiveUnit.Turn.StartTurn();
		TeamTurn = ActiveUnit.Team;
		
		if(TeamTurn == TeamType.Alpha)
		{
			PlayerMaster.Instance.CurrentUnit = ActiveUnit;
			PlayerMaster.Instance.Mode = FocusMode.Menu;
			PlayerMaster.Instance.Menu.IsActive = true;
		}

		CameraManager.Instance.DirectCameraFocus(ActiveUnit);
		Log.Info($"Unit {ActiveUnit.Data.Name}'s Turn On Team {ActiveUnit.Team}");
	}

	public void HandleTurnCommands()
	{
		if(ActiveUnit.isAIControlled)
		{
			if(!ActiveUnit.AI.HasTakenTurn && !EnsureAITurn)
			{
				EnsureAITurn = true;
				Log.Info("BattleManager AI Turn");
				ActiveUnit.AI.TakeTurn();
			}

		}
	}

	public void ProcessCommands()
	{

	}

	public bool CheckUnitTurnEnded()
	{
		if(ActiveUnit.Turn.HasMoved && !ActiveUnit.Turn.HasActed)
		{
			Log.Info("Unit Must Act");
			PlayerMaster.Instance.Mode = FocusMode.Menu;
			return false;

		}
		else if(!ActiveUnit.Turn.HasMoved && ActiveUnit.Turn.HasActed)
		{
			PlayerMaster.Instance.Mode = FocusMode.Menu;
			Log.Info("Unit Must Move");
			return false;
		}
		else if(ActiveUnit.Turn.HasMoved && ActiveUnit.Turn.HasActed)
		{
			Log.Info("Unit Turn Over");
			return true;
		}
		return false;
	}

	public void DecideTurnState()
	{
		switch(CheckUnitTurnEnded())
		{
			case true:
				ChangeCurrentState(BattleState.TurnEnd);
				break;
			case false:
				ChangeCurrentState(BattleState.WaitForCommands);
				break;
		}
	}
	public bool CheckEndConditions()
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
	}
	public async void EndTurn()
	{

		Unit Last = ActiveUnit;
		ActiveUnit.IsTurn = false;
		ActiveUnit.Turn.EndTurn();
		UnitTurnQueue.Dequeue();
		if(TeamTurn == TeamType.Alpha)
		{
			PlayerMaster.Instance.CurrentUnit = null;
		}
		else if(ActiveUnit.isAIControlled)
		{
			ActiveUnit.AI.EndTurn();
			EnsureAITurn = false;
		}
		ActiveUnit = null;
		if(CheckEndConditions()) return;
		await Task.DelayRealtimeSeconds(1.5f);
		UnitTurnQueue.Enqueue(Last);
		Log.Info("End Turn Delay");

		ChangeCurrentState(BattleState.TurnStart);
	}

	public async void EndBattle()
	{
		Music.StopTheme();
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


		Sound.Play(BattleEndTheme);
		await Task.DelayRealtimeSeconds(1f);
		EndUI.IsActive = true;
		Log.Info("Game Over");
	}
}



public enum TeamType
{
	Alpha,
	Omega,
}

public enum BattleState
{
	BattleStart,
	TurnStart,
	WaitForCommands,
	ProcessCommands,
	TurnEnd,
	BattleEnd,
}
